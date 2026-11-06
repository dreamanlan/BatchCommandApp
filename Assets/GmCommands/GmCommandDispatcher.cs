using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

// One GM request: created on a socket thread, executed on the main thread, then signalled back.
internal sealed class GmCommandRequest
{
    internal object Id;
    internal string Command = string.Empty;
    internal string Code = string.Empty;
    internal int TimeoutMs;
    internal long Since = -1;
    internal int Count = 200;
    internal string Filter = string.Empty;

    internal readonly ManualResetEventSlim Done = new ManualResetEventSlim(false);

    internal bool Ok = true;
    internal string Result = string.Empty;
    internal Dictionary<string, object> ResultObject;
    internal string Error = string.Empty;
    internal long ElapsedMs;
    internal List<Dictionary<string, object>> Logs;
    internal long NextLogIndex = -1;
}

// Dispatcher: socket thread enqueues -> main thread Pump executes -> reply.
// Pump is driven by GmRootScript.Update at runtime and by EditorApplication.update in the editor.
internal static class GmCommandDispatcher
{
    internal const int c_DefaultTimeout = 15000;
    internal const int c_MaxQueuedRequests = 256;
    internal const int c_MaxRequestsPerPump = 8;
    internal const int c_MaxLogsPerResponse = 200;

    // The delegates below are always invoked on the main thread; registered by GmCommandApi.EnsureInit / the editor listener.
    internal static Func<string, string> DslExecutor;
    internal static Func<string, string> ConsoleExecutor;
    internal static Func<string, string> EditorExecutor;
    internal static Func<Dictionary<string, object>> InfoProvider;

    // Callable from any thread; returns one line of JSON (no trailing newline).
    internal static string HandleLine(string line)
    {
        if (string.IsNullOrEmpty(line)) {
            return ErrorResponse(null, "empty request");
        }
        line = line.Trim();
        if (line.Length == 0) {
            return ErrorResponse(null, "empty request");
        }
        if (line[0] != '{') {
            // Legacy protocol: a plain text line is treated as a DebugConsole command.
            line = "{\"cmd\":\"console\",\"code\":\"" + GmJson.Escape(line) + "\"}";
        }

        GmCommandRequest req;
        object id = null;
        try {
            var dict = GmJson.ParseObject(line);
            dict.TryGetValue("id", out id);
            req = new GmCommandRequest();
            req.Id = id;
            req.Code = GmJson.GetString(dict, "code", GmJson.GetString(dict, "text", string.Empty));
            string cmd = GmJson.GetString(dict, "cmd", null);
            if (string.IsNullOrEmpty(cmd)) {
                cmd = string.IsNullOrEmpty(req.Code) ? "ping" : "exec";
            }
            req.Command = cmd.Trim().ToLowerInvariant();
            req.TimeoutMs = GmJson.GetInt(dict, "timeout", c_DefaultTimeout);
            req.Since = GmJson.GetLong(dict, "since", -1);
            req.Count = GmJson.GetInt(dict, "count", c_MaxLogsPerResponse);
            req.Filter = GmJson.GetString(dict, "filter", string.Empty);
        }
        catch (Exception ex) {
            return ErrorResponse(id, "bad request: " + ex.Message);
        }

        if (req.TimeoutMs <= 0) {
            req.TimeoutMs = c_DefaultTimeout;
        }
        if (req.TimeoutMs > 300000) {
            req.TimeoutMs = 300000;
        }
        if (req.Count <= 0) {
            req.Count = c_MaxLogsPerResponse;
        }

        lock (s_Lock) {
            if (s_Requests.Count >= c_MaxQueuedRequests) {
                return ErrorResponse(id, "server busy: too many pending requests");
            }
            s_Requests.Enqueue(req);
        }

        if (!req.Done.Wait(req.TimeoutMs)) {
            return ErrorResponse(id, string.Format("timed out after {0}ms: main thread did not process the request (game paused / blocked?)", req.TimeoutMs));
        }
        return BuildResponse(req);
    }

    // Called on the main thread, once per frame.
    internal static void Pump()
    {
        for (int i = 0; i < c_MaxRequestsPerPump; ++i) {
            GmCommandRequest req;
            lock (s_Lock) {
                if (s_Requests.Count <= 0) {
                    return;
                }
                req = s_Requests.Dequeue();
            }
            Process(req);
        }
    }

    internal static int PendingCount
    {
        get {
            lock (s_Lock) {
                return s_Requests.Count;
            }
        }
    }

    private static void Process(GmCommandRequest req)
    {
        var sw = Stopwatch.StartNew();
        long logStart = GmLogBuffer.CurrentIndex;
        try {
            GmCommandApi.EnsureInit();
            switch (req.Command) {
                case "ping":
                    req.ResultObject = null != InfoProvider ? InfoProvider() : new Dictionary<string, object>();
                    req.Result = "pong";
                    break;
                case "exec":
                    req.Result = Invoke(DslExecutor, req.Code, "dsl executor not registered");
                    break;
                case "console":
                    req.Result = Invoke(ConsoleExecutor, req.Code, "console executor not registered");
                    break;
                case "edit":
                    req.Result = Invoke(EditorExecutor, req.Code, "editor executor not registered (only available inside Unity Editor)");
                    break;
                case "logs": {
                        long next;
                        req.Logs = GmLogBuffer.Collect(req.Since, req.Count, req.Filter, out next);
                        req.NextLogIndex = next;
                        req.Result = "ok";
                    }
                    break;
                case "clearlogs":
                    GmLogBuffer.Clear();
                    req.Result = "cleared";
                    break;
                default:
                    req.Ok = false;
                    req.Error = "unknown cmd: " + req.Command + " (support: ping/exec/console/edit/logs/clearlogs)";
                    break;
            }
        }
        catch (Exception ex) {
            req.Ok = false;
            req.Error = ex.Message;
            if (!string.IsNullOrEmpty(ex.StackTrace)) {
                req.Error += "\n" + ex.StackTrace;
            }
        }

        // Most GM commands report through logs; attach the logs produced while executing.
        if (req.Command != "logs") {
            try {
                long next;
                var logs = GmLogBuffer.Collect(logStart, c_MaxLogsPerResponse, string.Empty, out next);
                if (null != logs && logs.Count > 0) {
                    req.Logs = logs;
                }
            }
            catch {
            }
        }

        req.ElapsedMs = sw.ElapsedMilliseconds;
        req.Done.Set();
    }

    private static string Invoke(Func<string, string> executor, string code, string notRegisteredError)
    {
        if (null == executor) {
            throw new InvalidOperationException(notRegisteredError);
        }
        var r = executor(code);
        return null == r ? "null" : r;
    }

    private static string BuildResponse(GmCommandRequest req)
    {
        var dict = new Dictionary<string, object>();
        if (null != req.Id) {
            dict["id"] = req.Id;
        }
        dict["ok"] = req.Ok;
        if (req.Ok) {
            if (null != req.ResultObject) {
                dict["result"] = req.ResultObject;
            }
            else {
                dict["result"] = req.Result;
            }
        }
        else {
            dict["error"] = req.Error;
        }
        dict["elapsedMs"] = req.ElapsedMs;
        if (null != req.Logs) {
            dict["logs"] = req.Logs;
        }
        if (req.NextLogIndex >= 0) {
            dict["next"] = req.NextLogIndex;
        }
        return GmJson.Serialize(dict);
    }

    private static string ErrorResponse(object id, string error)
    {
        var dict = new Dictionary<string, object>();
        if (null != id) {
            dict["id"] = id;
        }
        dict["ok"] = false;
        dict["error"] = error;
        return GmJson.Serialize(dict);
    }

    private static readonly Queue<GmCommandRequest> s_Requests = new Queue<GmCommandRequest>();
    private static readonly object s_Lock = new object();
}

// Log ring buffer: pollable through the logs command, also attached to exec/console replies.
internal static class GmLogBuffer
{
    internal const int c_MaxEntries = 2000;

    internal static void Install()
    {
        if (s_Installed) {
            return;
        }
        s_Installed = true;
        Application.logMessageReceived += OnLogReceived;
    }
    internal static void Uninstall()
    {
        if (!s_Installed) {
            return;
        }
        s_Installed = false;
        Application.logMessageReceived -= OnLogReceived;
    }
    internal static long CurrentIndex
    {
        get {
            lock (s_Lock) {
                return s_Seq;
            }
        }
    }
    internal static void Clear()
    {
        lock (s_Lock) {
            s_Entries.Clear();
        }
    }
    internal static List<Dictionary<string, object>> Collect(long since, int count, string filter, out long next)
    {
        if (count <= 0) {
            count = 200;
        }
        var ret = new List<Dictionary<string, object>>();
        lock (s_Lock) {
            next = since;
            if (since < 0) {
                int from = s_Entries.Count - 1;
                for (int i = from; i >= 0 && ret.Count < count; --i) {
                    var e = s_Entries[i];
                    if (!Match(e, filter)) {
                        continue;
                    }
                    ret.Insert(0, ToDict(e));
                }
                next = s_Seq;
            }
            else {
                for (int i = 0; i < s_Entries.Count; ++i) {
                    var e = s_Entries[i];
                    if (e.Seq <= since) {
                        continue;
                    }
                    if (ret.Count >= count) {
                        break;
                    }
                    if (!Match(e, filter)) {
                        next = e.Seq;
                        continue;
                    }
                    ret.Add(ToDict(e));
                    next = e.Seq;
                }
            }
        }
        return ret;
    }

    private static bool Match(Entry e, string filter)
    {
        if (string.IsNullOrEmpty(filter)) {
            return true;
        }
        return null != e.Message && e.Message.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
    }
    private static Dictionary<string, object> ToDict(Entry e)
    {
        var d = new Dictionary<string, object>();
        d["i"] = e.Seq;
        d["lv"] = e.Level;
        d["m"] = e.Message;
        return d;
    }
    private static void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        string level = "I";
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) {
            level = "E";
        }
        else if (type == LogType.Warning) {
            level = "W";
        }
        string msg = condition;
        if (level == "E" && !string.IsNullOrEmpty(stackTrace)) {
            msg = condition + "\n" + stackTrace;
        }
        lock (s_Lock) {
            ++s_Seq;
            s_Entries.Add(new Entry { Seq = s_Seq, Level = level, Message = msg });
            if (s_Entries.Count > c_MaxEntries) {
                s_Entries.RemoveAt(0);
            }
        }
    }

    private sealed class Entry
    {
        internal long Seq;
        internal string Level = "I";
        internal string Message = string.Empty;
    }

    private static readonly List<Entry> s_Entries = new List<Entry>();
    private static readonly object s_Lock = new object();
    private static long s_Seq;
    private static bool s_Installed;
}
