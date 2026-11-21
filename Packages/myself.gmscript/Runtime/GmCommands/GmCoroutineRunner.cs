using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

/// <summary>
/// Channels a job may run on. Flags (not a bool) so a job can declare
/// editor-only, runtime-only, or both. The runner checks these bits against
/// the channel that actually delivered the jobstart request.
/// </summary>
[Flags]
public enum GmJobChannel
{
    None = 0,
    Editor = 1 << 0,   // editor socket channel / edit mode (UnityEditor APIs)
    Runtime = 1 << 1,  // runtime socket channel / Play mode / standalone player
    Both = Editor | Runtime,
}

/// <summary>
/// Yield a progress checkpoint. Returning one from a GM command coroutine
/// updates the job's counters and hands the editor back one frame, so a
/// 640-scene bake no longer holds the main thread for hours inside a single
/// EditorApplication.update callback.
/// </summary>
public sealed class GmJobStep
{
    public int Done;
    public int Total;
    public string Note;

    public GmJobStep(int done, int total, string note = "")
    {
        Done = done;
        Total = total;
        Note = note ?? string.Empty;
    }
}

/// <summary>Yield this as the final value to set the job's result string.</summary>
public sealed class GmJobResult
{
    public string Result;
    public GmJobResult(string result) { Result = result ?? string.Empty; }
}

/// <summary>
/// Yield once per failed work item. A batch command may catch per-item
/// exceptions to keep processing the rest of the list, but the job must not
/// then report "done": that would silently hide 12/12 failures as a success.
/// </summary>
public sealed class GmJobFail
{
    public string Message;
    public GmJobFail(string message) { Message = message ?? string.Empty; }
}

/// <summary>
/// One long-running GM command. The body is a plain IEnumerator; no
/// MonoBehaviour / StartCoroutine is needed, so the same runner works in edit
/// mode (EditorApplication.update) and in Play mode / player (Update).
/// </summary>
public sealed class GmJob
{
    public int Id;
    public string Name = string.Empty;
    public string Arg = string.Empty;
    public IEnumerator Body;
    public string State = "running";   // running | done | error | cancelled
    public string Result = string.Empty;
    public string Error = string.Empty;
    public int Done;
    public int Total;
    public string Note = string.Empty;
    public int Failed;
    public string FirstError = string.Empty;
    public long ElapsedMs;
    public bool CancelRequested;
    // Channels this job declared at registration time (flags, not a bool).
    public GmJobChannel Channels = GmJobChannel.None;

    internal readonly Stopwatch Watch = new Stopwatch();
}

/// <summary>
/// Frame-driven coroutine scheduler for GM commands.
///
/// Long batch commands used to run entirely inside one
/// EditorApplication.update -> GmCommandDispatcher.Pump() call. That blocked
/// script compilation, asset refresh, repaint and every subsequent GM request
/// for hours (Unity showed "Hold on (busy for ...)"). Commands now expose an
/// IEnumerator and yield GmJobStep between work items; Tick() advances all
/// active jobs once per frame under a small time budget.
///
/// Requests still use the existing edit/exec channel: an async command returns
/// "JOB_STARTED id=N" immediately, and job_status / job_result / job_cancel /
/// job_list are ordinary synchronous DSL calls. No protocol change is needed.
/// </summary>
public static class GmCoroutineRunner
{
    // Per-frame wall-clock budget for ALL jobs combined. A single work item
    // (e.g. NavMeshBuilder.BuildNavMesh for one scene) may exceed it, but the
    // next item always waits for the following frame.
    public const int c_FrameBudgetMs = 12;

    // Factories take a string[] so a job can expose the full parameter list of
    // the underlying batch command (which, force, dryRun, limit, ...). Keeping
    // one scalar `arg` forced boolean options to be encoded as separate job
    // names (map_nav / map_nav_force, ..._fit / ..._fit_dry).
    private static readonly Dictionary<string, Func<string[], IEnumerator>> s_Factories =
        new Dictionary<string, Func<string[], IEnumerator>>(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, GmJobChannel> s_Channels =
        new Dictionary<string, GmJobChannel>(StringComparer.OrdinalIgnoreCase);
    private static readonly List<GmJob> s_Jobs = new List<GmJob>();
    private static int s_NextId = 1;
    private static long s_LastTickTimestamp = -1;

    static GmCoroutineRunner()
    {
        // Channel-agnostic self-check job. Registered eagerly so it exists even
        // before any DSL calculator is lazily created (e.g. a jobstart request
        // arriving straight through the dispatcher).
        Register("job_channel_probe", args => GmJobDslApi.Probe(args), GmJobChannel.Both);
    }

    /// <summary>
    /// Register a named long command with an EXPLICIT channel mask. There is
    /// deliberately no default: nearly every job belongs to exactly one side,
    /// so omitting the mask must be a compile error rather than silently
    /// granting both channels. Editor means the body may call UnityEditor
    /// APIs; Runtime means it is safe in Play mode / a standalone player.
    /// Both is intentionally rare and must not call UnityEditor-only APIs.
    /// </summary>
    public static void Register(string name, Func<string[], IEnumerator> factory,
                                GmJobChannel channels)
    {
        if (string.IsNullOrEmpty(name) || null == factory) return;
        if (channels == GmJobChannel.None)
            throw new ArgumentException("job '" + name + "' registered with no channel; pass Editor, Runtime or Both");
        lock (s_Factories) {
            s_Factories[name] = factory;
            s_Channels[name] = channels;
        }
    }

    /// <summary>Convenience overload for factories that only need one argument.</summary>
    public static void Register(string name, Func<string, IEnumerator> factory,
                                GmJobChannel channels)
    {
        if (string.IsNullOrEmpty(name) || null == factory) return;
        Register(name, args => factory(args != null && args.Length > 0 ? args[0] : string.Empty), channels);
    }

    /// <summary>Channel mask a job was registered with (None when unknown = fail closed).</summary>
    public static GmJobChannel ChannelsOf(string name)
    {
        lock (s_Factories) {
            GmJobChannel c;
            return s_Channels.TryGetValue(name, out c) ? c : GmJobChannel.None;
        }
    }

    public static string Arg(string[] args, int index)
    {
        return args != null && args.Length > index ? (args[index] ?? string.Empty) : string.Empty;
    }

    public static bool BoolArg(string[] args, int index, bool defVal = false)
    {
        string s = Arg(args, index).Trim().ToLowerInvariant();
        if (s.Length == 0) return defVal;
        return s == "1" || s == "true" || s == "yes" || s == "on" || s == "force" || s == "dry";
    }

    /// <summary>
    /// Registered job names with their channel mask, e.g. "map_nav[Editor]".
    /// Showing the mask makes it easy to audit that Both stays rare.
    /// </summary>
    public static string RegisteredCommands()
    {
        lock (s_Factories) {
            var names = new List<string>(s_Factories.Keys);
            names.Sort(StringComparer.Ordinal);
            var parts = new List<string>(names.Count);
            foreach (string n in names) {
                GmJobChannel c;
                if (!s_Channels.TryGetValue(n, out c)) c = GmJobChannel.None;
                parts.Add(n + "[" + c + "]");
            }
            return string.Join(",", parts.ToArray());
        }
    }

    /// <summary>
    /// Run a coroutine to completion in the current call, preserving the old
    /// synchronous DSL command semantics. Short/single-tile calls still use
    /// this; only the full-batch variants need job_start.
    /// </summary>
    public static string RunSync(IEnumerator body)
    {
        if (null == body) return string.Empty;
        string result = string.Empty;
        while (body.MoveNext()) {
            var res = body.Current as GmJobResult;
            if (null != res) { result = res.Result; continue; }
            var str = body.Current as string;
            if (null != str) result = str;
        }
        return result;
    }

    /// <summary>
    /// Job body used when the delivering channel is not in the job's mask.
    /// Kept here so the guard is a runner concern, not something every
    /// project-side job has to reimplement.
    /// </summary>
    private static IEnumerator ChannelRejected(string reason)
    {
        yield return new GmJobFail(reason);
        yield return new GmJobResult("ABORTED " + reason);
    }

    /// <summary>
    /// Which channel capability is usable right now. The editor socket channel
    /// stays connected during Play mode, but UnityEditor batch APIs
    /// (AssetDatabase / EditorSceneManager / NavMeshBuilder) are NOT usable
    /// there -- so Play mode only has the Runtime capability. Checking
    /// isPlaying is essential: without it an Editor-only job started from the
    /// editor channel while playing would pass the channel mask and then call
    /// EditorSceneManager.OpenScene, failing every work item.
    /// </summary>
    private static GmJobChannel AvailableChannel
    {
        get {
            return Application.isEditor && !Application.isPlaying
                ? GmJobChannel.Editor : GmJobChannel.Runtime;
        }
    }

    /// <summary>
    /// Start a registered command on the channel that delivered the request.
    /// Two bit checks, no hard-coded channel list:
    ///   1. (channels &amp; requested)    != 0 -- the caller's channel may start it.
    ///   2. (channels &amp; available)   != 0 -- the current Unity mode can run it.
    /// "Both" jobs pass in either mode; Editor-only jobs need edit mode;
    /// Runtime-only jobs need Play mode / player. A job declaring Both must
    /// therefore avoid UnityEditor-only APIs.
    /// </summary>
    public static int StartCommand(string name, string[] args, GmJobChannel requested)
    {
        string[] safe = args ?? new string[0];
        Func<string[], IEnumerator> factory;
        GmJobChannel channels;
        lock (s_Factories) {
            if (!s_Factories.TryGetValue(name, out factory))
                throw new InvalidOperationException("unknown gm job: " + name +
                    " (registered: " + RegisteredCommands() + ")");
            if (!s_Channels.TryGetValue(name, out channels)) channels = GmJobChannel.None;
        }
        GmJobChannel available = AvailableChannel;
        IEnumerator body;
        if ((channels & requested) == 0) {
            body = ChannelRejected(string.Format(
                "job '{0}' allows [{1}] but was started on [{2}] channel",
                name, channels, requested));
        }
        else if ((channels & available) == 0) {
            body = ChannelRejected(string.Format(
                "job '{0}' allows [{1}] but Unity currently only supports [{2}] (isPlaying={3}); stop Play or run in edit mode",
                name, channels, available, Application.isPlaying));
        }
        else {
            body = factory(safe);
            if (null == body)
                throw new InvalidOperationException("gm job factory returned null: " + name);
        }
        string argText = safe.Length == 0 ? string.Empty : string.Join(",", safe);
        int id = Start(name, argText, body);
        GmJob job = Find(id);
        if (null != job) {
            job.Channels = channels;
        }
        return id;
    }

    /// <summary>Backwards-compatible entry point: infer the channel from play state.</summary>
    public static int StartCommand(string name, string[] args)
    {
        return StartCommand(name, args,
            Application.isPlaying ? GmJobChannel.Runtime : GmJobChannel.Editor);
    }

    /// <summary>Backwards-compatible single-argument entry point.</summary>
    public static int StartCommand(string name, string arg)
    {
        return StartCommand(name, string.IsNullOrEmpty(arg) ? new string[0] : new[] { arg });
    }

    public static int Start(string name, string arg, IEnumerator body)
    {
        int id;
        lock (s_Jobs) { id = s_NextId++; }
        var job = new GmJob {
            Id = id, Name = name ?? "job", Arg = arg ?? string.Empty, Body = body
        };
        job.Watch.Start();
        lock (s_Jobs) { s_Jobs.Add(job); }
        return id;
    }

    /// <summary>
    /// Advance every active job. Called from GmCommandDispatcher.Pump(), which
    /// is the single main-thread entry point for both the editor and runtime
    /// channels. The timestamp guard makes the two drivers in editor Play mode
    /// count as one frame instead of double-stepping every job.
    /// </summary>
    public static void Tick()
    {
        long now = System.Diagnostics.Stopwatch.GetTimestamp();
        long freq = System.Diagnostics.Stopwatch.Frequency;
        if (s_LastTickTimestamp >= 0 &&
            (now - s_LastTickTimestamp) * 1000.0 / freq < 1.0) {
            return;
        }
        s_LastTickTimestamp = now;

        List<GmJob> snapshot;
        lock (s_Jobs) { snapshot = new List<GmJob>(s_Jobs); }
        if (snapshot.Count == 0) return;

        var budget = System.Diagnostics.Stopwatch.StartNew();
        foreach (GmJob job in snapshot) {
            if (job.State != "running") continue;
            if (budget.ElapsedMilliseconds >= c_FrameBudgetMs) break;

            if (job.CancelRequested) { Finish(job, "cancelled", string.Empty, "cancelled by request"); continue; }
            try {
                // Advance until the coroutine yields a checkpoint, completes,
                // or the shared frame budget is used up.
                while (budget.ElapsedMilliseconds < c_FrameBudgetMs) {
                    bool more;
                    try { more = job.Body.MoveNext(); }
                    catch (Exception ex) {
                        Finish(job, "error", string.Empty, ex.Message + "\n" + ex.StackTrace);
                        break;
                    }
                    if (!more) {
                        Finish(job, "done", job.Result, string.Empty);
                        break;
                    }
                    object cur = job.Body.Current;
                    var fail = cur as GmJobFail;
                    if (null != fail) {
                        ++job.Failed;
                        if (string.IsNullOrEmpty(job.FirstError)) job.FirstError = fail.Message;
                        break;                       // hand this frame back
                    }
                    var step = cur as GmJobStep;
                    if (null != step) {
                        job.Done = step.Done;
                        job.Total = step.Total;
                        job.Note = step.Note;
                        break;                       // hand this frame back
                    }
                    var res = cur as GmJobResult;
                    if (null != res) {
                        Finish(job, "done", res.Result, string.Empty);
                        break;
                    }
                    if (cur is string) {
                        Finish(job, "done", (string)cur, string.Empty);
                        break;
                    }
                    // null (and anything else) is a plain yield-return-null
                    // checkpoint: one step this frame.
                    break;
                }
            }
            catch (Exception ex) {
                Finish(job, "error", string.Empty, ex.Message);
            }
        }
    }

    private static void Finish(GmJob job, string state, string result, string error)
    {
        // A completed batch with any failed work item must not be reported as
        // success. Keep the result text (it shows every per-item error line)
        // but mark the job as error and expose the first cause.
        if (state == "done" && job.Failed > 0) {
            state = "error";
            int total = job.Total > 0 ? job.Total : (job.Done > 0 ? job.Done : job.Failed);
            error = string.Format("{0}/{1} work items failed; first: {2}",
                job.Failed, total, job.FirstError);
        }
        job.State = state;
        job.Result = result ?? string.Empty;
        job.Error = error ?? string.Empty;
        job.Watch.Stop();
        job.ElapsedMs = job.Watch.ElapsedMilliseconds;
        try { if (null != job.Body) job.Body.Reset(); } catch { }
    }

    public static GmJob Find(int id)
    {
        lock (s_Jobs) {
            for (int i = s_Jobs.Count - 1; i >= 0; --i)
                if (s_Jobs[i].Id == id) return s_Jobs[i];
        }
        return null;
    }

    public static string Status(int id)
    {
        GmJob j = Find(id);
        if (null == j) return "JOB id=" + id + " state=missing";
        var sb = new StringBuilder();
        sb.AppendFormat("JOB id={0} name={1} state={2} progress={3}/{4} failed={5} channels={6} note={7} elapsedMs={8}",
            j.Id, j.Name, j.State, j.Done, j.Total, j.Failed, j.Channels,
            j.State == "running" ? j.Note : "", j.ElapsedMs);
        if (!string.IsNullOrEmpty(j.Error)) sb.Append(" error=").Append(j.Error);
        return sb.ToString();
    }

    public static string Result(int id)
    {
        GmJob j = Find(id);
        if (null == j) return "JOB_RESULT id=" + id + " missing";
        if (j.State == "running") return "JOB_RESULT id=" + id + " running " + j.Done + "/" + j.Total;
        return "JOB_RESULT id=" + id + " state=" + j.State + "\n" + j.Result +
               (string.IsNullOrEmpty(j.Error) ? "" : "\n" + j.Error);
    }

    public static bool Cancel(int id)
    {
        GmJob j = Find(id);
        if (null == j || j.State != "running") return false;
        j.CancelRequested = true;
        return true;
    }

    public static string List()
    {
        List<GmJob> snapshot;
        lock (s_Jobs) { snapshot = new List<GmJob>(s_Jobs); }
        var sb = new StringBuilder();
        sb.Append("JOBS count=").Append(snapshot.Count)
          .Append(" registered=").Append(RegisteredCommands()).Append('\n');
        foreach (GmJob j in snapshot)
            sb.AppendFormat("  id={0} {1} {2} {3}/{4} {5}\n",
                j.Id, j.Name, j.State, j.Done, j.Total, j.Note);
        return sb.ToString();
    }
}
