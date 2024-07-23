using GmCommands;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using StoryScript;

public sealed class GmRootScript : MonoBehaviour
{
    public delegate void DebugConsoleShowHideDelegation();

    void OnEnable()
    {
        m_Logger.Init(Application.persistentDataPath, string.Empty);
    }
    void OnDisable()
    {
        DeinitAndroidReceiver();
        m_Logger.Dispose();
    }
    void Start()
    {
        TryInit();
    }
    void Update()
    {
        try {
            TimeUtility.UpdateGfxTime(Time.time, Time.realtimeSinceStartup, Time.timeScale);
            if (m_ClipboardInterval > 0) {
                long curTime = TimeUtility.GetLocalRealMilliseconds();
                if (m_LastClipboardTime + m_ClipboardInterval < curTime) {
                    string cmd = GUIUtility.systemCopyBuffer.Trim();
                    if (cmd.StartsWith(c_clipboard_cmd_tag)) {
                        DebugConsole.Execute(cmd.Substring(c_clipboard_cmd_tag.Length));
                        GUIUtility.systemCopyBuffer = string.Empty;
                    }
                    m_LastClipboardTime = curTime;
                }
                else if(m_LastClipboardTime > curTime) {
                    m_LastClipboardTime = 0;
                }
            }
            if (null != m_AndroidReceiver) {
                HandleCommand();
            }
            ClientGmStorySystem.Instance.Tick();
        }
        catch (Exception ex) {
            LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    private void TryInitGmRoot()
    {
        if(m_Inited) return;
        m_Inited = true;

#if DEVELOPMENT_BUILD
        StoryScript.StoryConfigManager.Instance.IsDevelopment = true;
#else
        StoryScript.StoryConfigManager.Instance.IsDevelopment = false;
#endif

#if UNITY_EDITOR
        StoryScript.StoryConfigManager.Instance.IsDevice = false;
#elif UNITY_ANDROID || UNITY_IOS
        StoryScript.StoryConfigManager.Instance.IsDevice = true;
#endif

        ClientGmStorySystem.Instance.Init();
        PerfGradeGm.TryInit();

        m_CommandDocs = StoryScript.StoryCommandManager.Instance.GenCommandDocs();
        m_FunctionDocs = StoryScript.StoryFunctionManager.Instance.GenFunctionDocs();

        LogSystem.OnOutput = (StoryLogType type, string msg) => {
            switch (type) {
                case StoryLogType.Error:
                    //LogToConsole(msg);
                    Debug.LogError(msg);
                    break;
                case StoryLogType.Warn:
                    //LogToConsole(msg);
                    Debug.LogWarning(msg);
                    break;
                case StoryLogType.Info:
                    //LogToConsole(msg);
                    Debug.Log(msg);
                    break;
            }
            m_Logger.Log("{0}", msg);
        };
    }
    private void LogToConsole(string msg)
    {
        DebugConsole.Log(msg);
    }

    private void SetClipboardInterval(int interval)
    {
        m_ClipboardInterval = interval;
    }
    private void InitAndroidReceiver()
    {
#if UNITY_ANDROID
        if (null == m_AndroidReceiver) {
            m_AndroidReceiver = new BroadcastReceiverHandler();
        }
        m_AndroidReceiver.RegisterBroadcastReceiver("com.unity3d.command");
#endif
    }
    private void DeinitAndroidReceiver()
    {
#if UNITY_ANDROID
        if (null != m_AndroidReceiver) {
            m_AndroidReceiver.UnregisterBroadcastReceiver();
        }
#endif
    }

    private void OnResetStory()
    {
        try {
            ResetGmCode();
            ClientGmStorySystem.Instance.ClearGlobalVariables();
            ClientGmStorySystem.Instance.Reset();
            StoryScript.StoryConfigManager.Instance.Clear();
            LogSystem.Warn("ResetStory finish.");
        }
        catch (Exception ex) {
            LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    private void OnExecStoryCommand(string cmd)
    {
        try {
            ClientGmStorySystem.Instance.Reset();
            ClientGmStorySystem.Instance.LoadStoryText(Encoding.UTF8.GetBytes("script(main){onmessage(\"start\"){" + cmd + "}}"));
            ClientGmStorySystem.Instance.StartStory("main");
            LogSystem.Warn("ExecStoryCommand {0} finish.", cmd);
        }
        catch (Exception ex) {
            LogSystem.Error("ExecStoryCommand exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }
    private void OnExecStoryFile(string scriptFile)
    {
        try {
            if (string.IsNullOrEmpty(scriptFile)) {
                if (string.IsNullOrEmpty(m_LocalGmFile)) {
                    if (Application.platform == RuntimePlatform.Android) {
                        scriptFile = "/data/local/tmp/gm.dsl";
                    }
                    else {
                        scriptFile = "gm.dsl";
                    }
                    m_LocalGmFile = scriptFile;
                }
            }
            else {
                m_LocalGmFile = scriptFile;
            }
            RunLocalGmFile();

            LogSystem.Warn("ExecStoryFile {0} finish.", scriptFile);
        }
        catch (Exception ex) {
            LogSystem.Error("ExecStoryFile exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void ResetGmCode()
    {
        m_LocalGmFile = "";
    }
    private void RunLocalGmFile()
    {
        if (!string.IsNullOrEmpty(m_LocalGmFile)) {

            if (File.Exists(m_LocalGmFile)) {
                ClientGmStorySystem.Instance.Reset();
                ClientGmStorySystem.Instance.LoadStory(m_LocalGmFile);
                ClientGmStorySystem.Instance.StartStory("main");
            }
            else {
                string basePath = Application.persistentDataPath;
                if (Application.platform == RuntimePlatform.Android) {
                    basePath = "/data/local/tmp";
                }
                var path = Path.Combine(basePath, m_LocalGmFile);
                if (File.Exists(path)) {
                    ClientGmStorySystem.Instance.Reset();
                    ClientGmStorySystem.Instance.LoadStory(path);
                    ClientGmStorySystem.Instance.StartStory("main");
                }
                else {
                    var bytes = LoadFileFromStreamingAssets(m_LocalGmFile);
                    if (null != bytes) {
                        ClientGmStorySystem.Instance.Reset();
                        ClientGmStorySystem.Instance.LoadStoryText(bytes);
                        ClientGmStorySystem.Instance.StartStory("main");
                    }
                    else {
                        m_LocalGmFile = "";
                    }
                }
            }
        }
    }

    private int m_ClipboardInterval;
    private long m_LastClipboardTime;
    private BroadcastReceiverHandler m_AndroidReceiver;

    private SortedList<string, string> m_CommandDocs;
    private SortedList<string, string> m_FunctionDocs;
    private string m_LocalGmFile = string.Empty;
    private GmCommands.Logger m_Logger = new GmCommands.Logger();
    private bool m_Inited = false;

    public static DebugConsoleShowHideDelegation OnConsoleShow;
    public static DebugConsoleShowHideDelegation OnConsoleHide;

    public static SortedList<string, string> CommandDocs
    {
        get { return GetGmRootScript().m_CommandDocs; }
    }
    public static SortedList<string, string> FunctionDocs
    {
        get { return GetGmRootScript().m_FunctionDocs; }
    }
    public static GameObject GameObj
    {
        get {
            return s_GameObj;
        }
    }
    public static void TryLoad()
    {
        if (null == s_GameObj) {
            var prefab = Resources.Load<GameObject>("GmScript");
            var gobj = GameObject.Instantiate<GameObject>(prefab);
            gobj.name = "GmScript";
            TryInit();
        }
    }
    public static void TryInit()
    {
        if (null == s_GameObj) {
            s_GameObj = GameObject.Find("GmScript");
            if (null != s_GameObj) {
                GameObject.DontDestroyOnLoad(s_GameObj);
            }
        }
        if (null != s_GameObj) {
            GetGmRootScript().TryInitGmRoot();
        }
    }
    public static void OnDebugConsoleShow()
    {
        if (null != OnConsoleShow)
            OnConsoleShow();
    }
    public static void OnDebugConsoleHide()
    {
        if (null != OnConsoleHide)
            OnConsoleHide();
    }
    public static void ListenClipboard(int interval)
    {
        if (null != s_GameObj) {
            GetGmRootScript().SetClipboardInterval(interval);
        }
    }
    public static void ListenAndroid()
    {
        if (null != s_GameObj) {
            GetGmRootScript().InitAndroidReceiver();
        }
    }
    public static void SendCommand(string cmd)
    {
#if UNITY_ANDROID
        lock (s_Lock) {
            if (s_CommandQueue.Count < c_max_command_in_queue) {
                s_CommandQueue.Enqueue(cmd);
            }
            else {
                LogSystem.Error("command queue overflow !");
            }
        }
#endif
    }

    private static void HandleCommand()
    {
#if UNITY_ANDROID
        lock (s_Lock) {
            if (s_CommandQueue.Count > 0) {
                string cmd = s_CommandQueue.Dequeue();
                DebugConsole.Execute(cmd);
            }
        }
#endif
    }
    private static GmRootScript GetGmRootScript()
    {
        var obj = s_GameObj;
        Debug.Assert(null != obj);
        var scp = obj.GetComponent<GmRootScript>();
        return scp;
    }
    private static byte[] LoadFileFromStreamingAssets(string file)
    {
        if (Application.platform == RuntimePlatform.Android) {
            using (var androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var androidJavaActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                    if (null != androidJavaActivity) {
                        using (var assetManager = androidJavaActivity.Call<AndroidJavaObject>("getAssets")) {
                            if (null != assetManager) {
                                using (var inputStream = assetManager.Call<AndroidJavaObject>("open", file)) {
                                    if (null != inputStream) {
                                        int available = inputStream.Call<int>("available");
                                        System.IntPtr buffer = AndroidJNI.NewSByteArray(available);
                                        System.IntPtr javaClass = AndroidJNI.FindClass("java/io/InputStream");
                                        System.IntPtr javaMethodID = AndroidJNIHelper.GetMethodID(javaClass, "read", "([B)I");
                                        int read = AndroidJNI.CallIntMethod(inputStream.GetRawObject(), javaMethodID,
                                            new[] { new jvalue() { l = buffer } });
                                        sbyte[] sbytes = AndroidJNI.FromSByteArray(buffer);
                                        AndroidJNI.DeleteLocalRef(buffer);
                                        inputStream.Call("close");
                                        inputStream.Dispose();
                                        byte[] bytes = new byte[sbytes.Length];
                                        Array.Copy(bytes, sbytes, bytes.Length);
                                        return bytes;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else {
            string assembly = Path.Combine(Application.streamingAssetsPath, file);
            if (File.Exists(assembly)) {
                var bytes = File.ReadAllBytes(assembly);
                return bytes;
            }
        }
        return null;
    }

    internal static void StartService(string srvClass, string extraName, BoxedValue extraValue)
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                if (null != unityActivity) {
                    using (var context = unityActivity.Call<AndroidJavaObject>("getApplicationContext")) {
                        if (null != context) {
                            using (var intent = new AndroidJavaObject("android.content.Intent")) {
                                using (var serviceClass = new AndroidJavaClass(srvClass)) {
                                    intent.Call<AndroidJavaObject>("setClass", context, serviceClass);
                                    switch (extraValue.Type) {
                                        case BoxedValue.c_StringType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetString());
                                            break;
                                        case BoxedValue.c_ObjectType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetObject());
                                            break;
                                        case BoxedValue.c_BoolType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetBool());
                                            break;
                                        case BoxedValue.c_CharType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetChar());
                                            break;
                                        case BoxedValue.c_SByteType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetSByte());
                                            break;
                                        case BoxedValue.c_ByteType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetByte());
                                            break;
                                        case BoxedValue.c_ShortType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetShort());
                                            break;
                                        case BoxedValue.c_UShortType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetUShort());
                                            break;
                                        case BoxedValue.c_IntType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetInt());
                                            break;
                                        case BoxedValue.c_UIntType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetUInt());
                                            break;
                                        case BoxedValue.c_LongType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetLong());
                                            break;
                                        case BoxedValue.c_ULongType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetULong());
                                            break;
                                        case BoxedValue.c_FloatType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetFloat());
                                            break;
                                        case BoxedValue.c_DoubleType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetDouble());
                                            break;
                                        case BoxedValue.c_DecimalType:
                                            intent.Call<AndroidJavaObject>("putExtra", extraName, extraValue.GetDecimal());
                                            break;
                                        default:
                                            // unsupported type, do nothing
                                            break;
                                    }
                                    context.Call<AndroidJavaObject>("startService", intent);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    internal static void StopService(string srvClass)
    {
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                if (null != unityActivity) {
                    using (var context = unityActivity.Call<AndroidJavaObject>("getApplicationContext")) {
                        if (null != context) {
                            using (var intent = new AndroidJavaObject("android.content.Intent")) {
                                using (var serviceClass = new AndroidJavaClass(srvClass)) {
                                    intent.Call<AndroidJavaObject>("setClass", context, serviceClass);
                                    context.Call<bool>("stopService", intent);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal static string Exec(string cmd)
    {
        var sb = new StringBuilder();
        int exitCode = -1;
        if (Application.platform == RuntimePlatform.Android) {
            using (var javaRuntime = new AndroidJavaClass("java.lang.Runtime")) {
                using (var runtime = javaRuntime.CallStatic<AndroidJavaObject>("getRuntime")) {
                    if (null != runtime) {
                        using (var process = runtime.Call<AndroidJavaObject>("exec", cmd)) {
                            if (null != process) {
                                using (var inputStream = process.Call<AndroidJavaObject>("getInputStream")) {
                                    using (var inputStreamReader = new AndroidJavaObject("java.io.InputStreamReader", inputStream)) {
                                        using (var bufferedReader = new AndroidJavaObject("java.io.BufferedReader", inputStreamReader)) {
                                            string line;
                                            while ((line = bufferedReader.Call<string>("readLine")) != null) {
                                                sb.AppendLine(line);
                                            }
                                        }
                                    }
                                }
                                exitCode = process.Call<int>("waitFor");
                            }
                        }
                    }
                }
            }
        }
        else {
            string fileName, args;
            if (cmd.StartsWith('"')) {
                int endExe = cmd.IndexOf('"', 1);
                fileName = cmd.Substring(0, endExe + 1);
                args = cmd.Substring(endExe + 1).Trim();
            }
            else {
                int splitPos = cmd.IndexOf(' ');
                while (splitPos > 0) {
                    if (cmd[splitPos - 1] == '\\') {
                        splitPos = cmd.IndexOf(' ', splitPos + 1);
                    }
                    else {
                        break;
                    }
                }
                if (splitPos > 0) {
                    fileName = cmd.Substring(0, splitPos).Trim();
                    args = cmd.Substring(splitPos + 1).Trim();
                }
                else {
                    fileName = cmd.Trim();
                    args = string.Empty;
                }
            }
            var outEncoding = Encoding.GetEncoding(GetACP());
            exitCode = RunCommand(fileName, args, sb, sb, outEncoding);
        }
        LogSystem.Warn("Command:{0} exit code:{1}", cmd, exitCode);
        return sb.ToString();
    }

    private static int RunCommand(string fileName, string args, StringBuilder output, StringBuilder error, Encoding outEncoding)
    {
        //Considering cross-platform compatibility, do not use specific process environment variables.
        try {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            var psi = p.StartInfo;
            psi.FileName = fileName;
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.ErrorDialog = false;
            psi.WorkingDirectory = Environment.CurrentDirectory;
            psi.RedirectStandardInput = false;
            if (null != output) {
                psi.RedirectStandardOutput = true;
                psi.StandardOutputEncoding = outEncoding;
                p.OutputDataReceived += (sender, e) => OnOutputDataReceived(sender, e, output, outEncoding);
            }
            if (null != error) {
                psi.RedirectStandardError = true;
                psi.StandardErrorEncoding = outEncoding;
                p.ErrorDataReceived += (sender, e) => OnErrorDataReceived(sender, e, error, outEncoding);
            }
            if (p.Start()) {
                if (psi.RedirectStandardOutput)
                    p.BeginOutputReadLine();
                if (psi.RedirectStandardError)
                    p.BeginErrorReadLine();
                p.WaitForExit();
                if (psi.RedirectStandardOutput) {
                    p.CancelOutputRead();
                }
                if (psi.RedirectStandardError) {
                    p.CancelErrorRead();
                }
                int r = p.ExitCode;
                p.Close();
                return r;
            }
            else {
                LogSystem.Error("process({0} {1}) failed.", fileName, args);
                return -1;
            }
        }
        catch (Exception ex) {
            LogSystem.Error("process({0} {1}) exception:{2} stack:{3}", fileName, args, ex.Message, ex.StackTrace);
            while (null != ex.InnerException) {
                ex = ex.InnerException;
                LogSystem.Error("\t=> exception:{0} stack:{1}", ex.Message, ex.StackTrace);
            }
            return -1;
        }
    }

    private static void OnOutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e, StringBuilder output, Encoding srcEncoding)
    {
        var p = sender as System.Diagnostics.Process;
        if (p.StartInfo.RedirectStandardOutput && null != e.Data) {
            if (null != output) {
                string str = e.Data;
                if (srcEncoding != Encoding.UTF8) {
                    var bytes = srcEncoding.GetBytes(str);
                    bytes = Encoding.Convert(srcEncoding, Encoding.UTF8, bytes);
                    str = Encoding.UTF8.GetString(bytes);
                }
                output.Append(str);
            }
        }
    }

    private static void OnErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e, StringBuilder error, Encoding srcEncoding)
    {
        var p = sender as System.Diagnostics.Process;
        if (p.StartInfo.RedirectStandardError && null != e.Data) {
            if (null != error) {
                string str = e.Data;
                if (srcEncoding != Encoding.UTF8) {
                    var bytes = srcEncoding.GetBytes(str);
                    bytes = Encoding.Convert(srcEncoding, Encoding.UTF8, bytes);
                    str = Encoding.UTF8.GetString(bytes);
                }
                error.Append(str);
            }
        }
    }

    private static GameObject s_GameObj = null;
    private static Queue<string> s_CommandQueue = new Queue<string>();
    private static object s_Lock = new object();
    private const int c_max_command_in_queue = 1024;
    private const string c_clipboard_cmd_tag = "[cmd]:";

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern int GetACP();
#else
    static int GetACP()
    {
        return Encoding.Default.CodePage;
    }
#endif
}

[UnityEngine.Scripting.Preserve]
internal sealed class BroadcastReceiverHandler
{
    internal void RegisterBroadcastReceiver(string actionName)
    {
        if (m_ReceiverInited) {
            return;
        }
        using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                if (null != unityActivity) {
                    using (var context = unityActivity.Call<AndroidJavaObject>("getApplicationContext")) {
                        if (null != context) {
                            var callback = new BroadcastReceiverCallback();
                            m_BroadcastReceiver = new AndroidJavaObject("com.unity3d.broadcastlib.BroadcastHelper", callback);

                            using (var intentFilter = new AndroidJavaObject("android.content.IntentFilter")) {
                                intentFilter.Call("addAction", actionName);
                                context.Call<AndroidJavaObject>("registerReceiver", m_BroadcastReceiver, intentFilter);
                            }
                        }
                    }
                }
            }
        }
        m_ReceiverInited = true;
    }
    internal void UnregisterBroadcastReceiver()
    {
        if (m_ReceiverInited && null != m_BroadcastReceiver) {
            using (var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) {
                    if (null != unityActivity) {
                        using (var context = unityActivity.Call<AndroidJavaObject>("getApplicationContext")) {
                            if (null != context) {
                                context.Call("unregisterReceiver", m_BroadcastReceiver);
                            }
                        }
                    }
                }
            }
        }
    }

    private bool m_ReceiverInited;
    private AndroidJavaObject m_BroadcastReceiver;
}

[UnityEngine.Scripting.Preserve]
internal sealed class BroadcastReceiverCallback : AndroidJavaProxy
{
    public BroadcastReceiverCallback() : base("com.unity3d.broadcastlib.IBroadcastReceiver")
    { }
    void onReceive(AndroidJavaObject context, AndroidJavaObject intent)
    {
        string cmd = intent.Call<string>("getStringExtra", "cmd");
        if(!string.IsNullOrEmpty(cmd)) {
            GmRootScript.SendCommand(cmd);

            LogSystem.Info("receive a command: {0}", cmd);
        }
    }
}

[UnityEngine.Scripting.Preserve]
internal static class WeTestAutomation
{
    internal static void InjectTouch(int action, float x, float y)
    {
        Prepare();
        if (null != s_WeTestAutomation)
        {
            s_WeTestAutomation.CallStatic("InjectTouchEvent", action, x, y);
        }
    }
    internal static float GetX()
    {
        float x = 0;
        Prepare();
        if (null != s_WeTestAutomation)
        {
            x = s_WeTestAutomation.CallStatic<float>("GetX");
        }
        return x;
    }
    internal static float GetY()
    {
        float y = 0;
        Prepare();
        if (null != s_WeTestAutomation)
        {
            y = s_WeTestAutomation.CallStatic<float>("GetY");
        }
        return y;
    }
    internal static int GetWidth()
    {
        int w = 0;
        Prepare();
        if (null != s_WeTestAutomation)
        {
            w = s_WeTestAutomation.CallStatic<int>("GetWidth");
        }
        return w;
    }
    internal static int GetHeight()
    {
        int h = 0;
        Prepare();
        if (null != s_WeTestAutomation)
        {
            h = s_WeTestAutomation.CallStatic<int>("GetHeight");
        }
        return h;
    }
    private static void Prepare()
    {
        if (null == s_WeTestAutomation)
        {
            s_WeTestAutomation = new AndroidJavaClass("com.tencent.wetest.U3DAutomation");
        }
    }

    private static AndroidJavaClass s_WeTestAutomation;
}
