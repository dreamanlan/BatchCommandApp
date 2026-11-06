using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StoryScript;
using StoryScript.DslExpression;

// DSL evaluation: wrap into script{ ... }, run synchronously and return the last statement value.
internal static class GmDslEval
{
    internal static string Eval(DslCalculator calc, string code, BoxedValue obj, BoxedValue item)
    {
        if (null == calc) {
            throw new ArgumentNullException("calc");
        }
        if (string.IsNullOrEmpty(code)) {
            throw new ArgumentException("code is empty");
        }
        string text = "script{ " + code + "; };";
        string loadError = null;
        var file = new Dsl.DslFile();
        if (!file.LoadFromString(text, msg => { if (null == loadError) loadError = msg; })) {
            throw new Exception("dsl load failed: " + (null != loadError ? loadError : text));
        }
        if (file.DslInfos.Count <= 0) {
            throw new Exception("dsl has no info");
        }
        var info = file.DslInfos[0];
        var func = info as Dsl.FunctionData;
        if (null == func) {
            var statement = info as Dsl.StatementData;
            if (null != statement) {
                func = statement.First.AsFunction;
            }
        }
        if (null == func) {
            throw new Exception("dsl info is not a function call");
        }
        calc.LoadDsl("main", new string[] { "$obj", "$item" }, func);
        var r = calc.Calc("main", obj, item);
        return ToText(r);
    }
    internal static string ToText(BoxedValue val)
    {
        if (val.IsNullObject) {
            return "null";
        }
        var s = val.ToString();
        return null == s ? "null" : s;
    }
}

// Command entry points shared by runtime and editor (always run on the main thread).
internal static class GmCommandApi
{
    internal static void EnsureInit()
    {
        if (s_Inited) {
            return;
        }
        s_Inited = true;

        GmLogBuffer.Install();

        try {
            GmCommands.ClientGmStorySystem.Instance.Init();
        }
        catch (Exception ex) {
            LogSystem.Error("GmCommandApi init gm commands exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }

        GmCommandDispatcher.DslExecutor = ExecDsl;
        GmCommandDispatcher.ConsoleExecutor = ExecConsole;
        GmCommandDispatcher.InfoProvider = BuildInfo;
    }

    // GM DSL evaluation against the shared registry (all GM commands/functions live there).
    internal static string ExecDsl(string code)
    {
        var calc = new DslCalculator();
        calc.ApiRegistry = DslCalculatorHost.GetSharedApiRegistry();
        calc.OnLog = msg => { Debug.LogWarning("[GmSocket] " + msg); };
        return GmDslEval.Eval(calc, code, BoxedValue.NullObject, BoxedValue.NullObject);
    }

    // DebugConsole command; in edit mode there is no DebugConsole instance, so fall back to DSL evaluation.
    internal static string ExecConsole(string cmd)
    {
        if (DebugConsole.HasInstance) {
            var r = DebugConsole.ExecuteForResult(cmd);
            return null == r ? "null" : r.ToString();
        }
        return ExecDsl(cmd);
    }

    internal static Dictionary<string, object> BuildInfo()
    {
        var d = new Dictionary<string, object>();
        d["mode"] = Application.isEditor ? (Application.isPlaying ? "editor-play" : "editor") : "player";
        d["unity"] = Application.unityVersion;
        d["platform"] = Application.platform.ToString();
        d["isEditor"] = Application.isEditor;
        d["isPlaying"] = Application.isPlaying;
        d["productName"] = Application.productName;
        d["frame"] = Time.frameCount;
        d["time"] = Time.realtimeSinceStartup;
        d["pendingRequests"] = GmCommandDispatcher.PendingCount;
        try {
            var scene = SceneManager.GetActiveScene();
            d["scene"] = scene.IsValid() ? scene.name : string.Empty;
        }
        catch {
            d["scene"] = string.Empty;
        }
        try {
            using (var p = System.Diagnostics.Process.GetCurrentProcess()) {
                d["pid"] = p.Id;
            }
        }
        catch {
        }
        return d;
    }

    private static bool s_Inited;
}
