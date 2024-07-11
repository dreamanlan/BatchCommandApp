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
                    LogToConsole(msg);
                    Debug.LogError(msg);
                    break;
                case StoryLogType.Warn:
                    LogToConsole(msg);
                    Debug.LogWarning(msg);
                    break;
                case StoryLogType.Info:
                    LogToConsole(msg);
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
        ClientGmStorySystem.Instance.Reset();
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
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject androidJavaActivity = null;
            AndroidJavaObject assetManager = null;
            AndroidJavaObject inputStream = null;
            if (androidJavaClass != null)
                androidJavaActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (androidJavaActivity != null)
                assetManager = androidJavaActivity.Call<AndroidJavaObject>("getAssets");
            if (assetManager != null)
                inputStream = assetManager.Call<AndroidJavaObject>("open", file);
            if (inputStream != null) {
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
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        AndroidJavaClass serviceClass = new AndroidJavaClass(srvClass);
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
    internal static void StopService(string srvClass)
    {
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        AndroidJavaClass serviceClass = new AndroidJavaClass(srvClass);
        intent.Call<AndroidJavaObject>("setClass", context, serviceClass);

        context.Call<bool>("stopService", intent);
    }

    private static GameObject s_GameObj = null;
    private static Queue<string> s_CommandQueue = new Queue<string>();
    private static object s_Lock = new object();
    private const int c_max_command_in_queue = 1024;
    private const string c_clipboard_cmd_tag = "[cmd]:";
}

[UnityEngine.Scripting.Preserve]
internal sealed class BroadcastReceiverHandler
{
    internal void RegisterBroadcastReceiver(string actionName)
    {
        if (m_ReceiverInited) {
            return;
        }
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

        //AndroidJavaClass javaProxyClass = new AndroidJavaClass("com.unity3d.player.UnityPlayerNativeActivity");
        //AndroidJavaObject javaProxy = javaProxyClass.GetStatic<AndroidJavaObject>("currentActivity");

        var callback = new BroadcastReceiverCallback();
        m_BroadcastReceiver = new AndroidJavaObject("com.unity3d.broadcastlib.BroadcastHelper", callback);

        AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter");
        intentFilter.Call("addAction", actionName);
        context.Call<AndroidJavaObject>("registerReceiver", m_BroadcastReceiver, intentFilter);

        m_ReceiverInited = true;
    }
    internal void UnregisterBroadcastReceiver()
    {
        if (m_ReceiverInited && null != m_BroadcastReceiver) {
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

            context.Call("unregisterReceiver", m_BroadcastReceiver);
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
