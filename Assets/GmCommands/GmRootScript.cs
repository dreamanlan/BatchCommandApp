using GmCommands;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using StoryScript;

public class GmRootScript : MonoBehaviour
{
    void OnEnable()
    {
        m_Logger.Init(Application.persistentDataPath, string.Empty);
    }
    void OnDisable()
    {
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

    private SortedList<string, string> m_CommandDocs;
    private SortedList<string, string> m_FunctionDocs;
    private string m_LocalGmFile = string.Empty;
    private GmCommands.Logger m_Logger = new GmCommands.Logger();
    private bool m_Inited = false;

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

    private static GameObject s_GameObj = null;
}
