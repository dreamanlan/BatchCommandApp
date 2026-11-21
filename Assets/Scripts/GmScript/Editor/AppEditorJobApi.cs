using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using StoryScript;
using StoryScript.DslExpression;

/// <summary>
/// Bridges the generic frame-driven coroutine runner in myself.gmscript to the
/// editor's long batch commands.
///
/// The commands themselves are NOT rewritten. Each job iterates at the finest
/// granularity the existing API already exposes
/// That keeps every existing synchronous DSL command byte-for-byte compatible
/// while removing the multi-hour single-tick main-thread block.
/// </summary>
public static class AppEditorJobApi
{
    /// <summary>A single-step job for commands that are already quick but still
    /// registered so the job API is uniform.</summary>
    private static IEnumerator OnceCo(string which, Func<string, string> one)
    {
        string r;
        bool failed = false;
        try { r = one(which ?? string.Empty) ?? string.Empty; }
        catch (Exception e) { r = "ERR " + e.Message; failed = true; }
        if (failed || r.StartsWith("ERR ")) yield return new GmJobFail(r);
        else yield return new GmJobStep(1, 1, which ?? string.Empty);
        yield return new GmJobResult(r);
    }

    // All jobs below drive Editor-only batch APIs (AssetDatabase,
    // EditorSceneManager, NavMeshBuilder), so they declare the Editor channel
    // bit. GmCoroutineRunner owns the channel-mask check; no local guard.
    //
    // Job argument convention (shared by MCP args[] and DSL job_start):
    //   args[0] = tile/scene/category filter (empty = all)
    //   args[1] = option flag: force / dry (1|true|yes|on|force|dry)
    // Additional args are reserved for future commands; Arg() returns "" when absent.
    private const GmJobChannel c_Channels = GmJobChannel.Editor;

    [InitializeOnLoadMethod]
    private static void RegisterJobs()
    {
        GmCoroutineRunner.Register("map_nav", (string[] a) =>
            OnceCo(GmCoroutineRunner.Arg(a, 0), str => str), c_Channels);
    }
}
