using System;
using UnityEditor;
using UnityEngine;
using StoryScript;
using StoryScript.DslExpression;

// GM listener for edit mode: same protocol as runtime, commands executed from EditorApplication.update.
// Default port 39528 (runtime uses 39527, so both can be used at the same time in Play mode).
[InitializeOnLoad]
internal static class GmEditorSocketServer
{
    internal const int c_DefaultPort = 39528;
    private const string c_AutoStartKey = "UCRayTracing.GmSocket.EditorAutoStart";
    private const string c_PortKey = "UCRayTracing.GmSocket.EditorPort";

    static GmEditorSocketServer()
    {
        // exec uses the shared registry (GM commands), edit uses the editor DSL (resource/editor APIs).
        GmCommandDispatcher.EditorExecutor = ExecEditor;
        EditorApplication.delayCall -= OnEditorReady;
        EditorApplication.delayCall += OnEditorReady;
    }

    private static void OnEditorReady()
    {
        AssemblyReloadEvents.beforeAssemblyReload -= StopSilently;
        AssemblyReloadEvents.beforeAssemblyReload += StopSilently;
        EditorApplication.quitting -= StopSilently;
        EditorApplication.quitting += StopSilently;

        if (EditorPrefs.GetBool(c_AutoStartKey, true)) {
            Start();
        }
    }
    private static void StopSilently()
    {
        if (null == s_Server) {
            return;
        }
        EditorApplication.update -= Pump;
        s_Server.Stop();
        s_Server = null;
    }

    internal static bool IsRunning
    {
        get { return null != s_Server; }
    }
    internal static int Port
    {
        get {
            int port = EditorPrefs.GetInt(c_PortKey, c_DefaultPort);
            return port > 0 ? port : c_DefaultPort;
        }
    }
    internal static void Start()
    {
        if (null != s_Server) {
            return;
        }
        var server = new GmSocketServer(Port);
        if (!server.Start()) {
            return;
        }
        s_Server = server;
        EditorApplication.update -= Pump;
        EditorApplication.update += Pump;
        Debug.Log(string.Format("[GmSocket] editor listener started on 127.0.0.1:{0}", server.Port));
    }
    internal static void Stop()
    {
        if (null == s_Server) {
            return;
        }
        EditorApplication.update -= Pump;
        s_Server.Stop();
        s_Server = null;
        Debug.Log("[GmSocket] editor listener stopped.");
    }

    // Editor DSL evaluation: resource-processing APIs + editor APIs + GM commands.
    internal static string ExecEditor(string code)
    {
        return GmDslEval.Eval(EditorCalculator, code, BoxedValue.NullObject, BoxedValue.NullObject);
    }

    private static DslCalculator EditorCalculator
    {
        get {
            if (null == s_EditorCalculator) {
                // Make sure the shared registry already holds the GM commands before merging.
                GmCommandApi.EnsureInit();
                var calc = new DslCalculator();
                ResourceEditUtility.InitCalculator(calc);
                MergeSharedApis(calc);
                s_EditorCalculator = calc;
            }
            return s_EditorCalculator;
        }
    }
    private static void MergeSharedApis(DslCalculator calc)
    {
        try {
            var shared = DslCalculatorHost.GetSharedApiRegistry();
            int merged = 0;
            foreach (var pair in shared.ApiDocs) {
                if (!shared.TryGetFactory(pair.Key, out var factory)) {
                    continue;
                }
                if (calc.ApiRegistry.TryGetFactory(pair.Key, out var existing)) {
                    continue;
                }
                calc.Register(pair.Key, pair.Value, factory);
                ++merged;
            }
            Debug.Log(string.Format("[GmSocket] merged {0} shared api into editor calculator.", merged));
        }
        catch (Exception ex) {
            Debug.LogWarning("[GmSocket] merge shared api failed: " + ex.Message);
        }
    }
    private static void Pump()
    {
        GmCommandDispatcher.Pump();
    }

    [MenuItem("Tools/GM Socket/Start Editor Listener", false, 100)]
    private static void MenuStart()
    {
        EditorPrefs.SetBool(c_AutoStartKey, true);
        Start();
    }
    [MenuItem("Tools/GM Socket/Start Editor Listener", true)]
    private static bool MenuStartValidate()
    {
        return null == s_Server;
    }
    [MenuItem("Tools/GM Socket/Stop Editor Listener", false, 101)]
    private static void MenuStop()
    {
        EditorPrefs.SetBool(c_AutoStartKey, false);
        Stop();
    }
    [MenuItem("Tools/GM Socket/Stop Editor Listener", true)]
    private static bool MenuStopValidate()
    {
        return null != s_Server;
    }

    private static GmSocketServer s_Server;
    private static DslCalculator s_EditorCalculator;
}
