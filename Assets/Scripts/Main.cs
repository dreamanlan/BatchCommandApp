using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ExpressionAPI;
using Dsl;
using StoryScript;
using StoryScript.DslExpression;
using StoryApi;

[UnityEngine.Scripting.Preserve]
public class Main : MonoBehaviour
{
    void Awake()
    {
		StartupScript.InitLogger();
    }
    void Start()
    {
        s_Instance = this;

        var txtWriter = new StringWriter(m_LogBuilder);
        System.Console.SetOut(txtWriter);
        System.Console.SetError(txtWriter);
        Application.logMessageReceived += HandleLog;
        StartCoroutine(Loop());

        StartupScript.TryInit();
        UnityEditorApi.Register(StartupScript.Calculator);
        StartupScript.Calculator.Register("setclipboard", "setclipboard(str) api", new ExpressionFactoryHelper<SetClipboardExp>());
        StartupScript.Calculator.Register("getclipboard", "getclipboard() api", new ExpressionFactoryHelper<GetClipboardExp>());

        RunStartup(true);
    }
    void OnDestroy()
    {
        StartupScript.ReleaseLogger();
    }
    private IEnumerator Loop()
    {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            if (m_LogBuilder.Length > 0) {
                var txt = m_LogBuilder.ToString();
                m_LogBuilder.Length = 0;
                DebugConsole.Log(txt);
            }
        }
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        switch (type) {
            case LogType.Log:
                DebugConsole.Log(logString);
                break;
            case LogType.Warning:
                DebugConsole.LogWarning(logString);
                break;
            case LogType.Error:
                DebugConsole.LogError(string.Format("{0}{1}", logString, stackTrace));
                break;
            case LogType.Assert:
                DebugConsole.Log(string.Format("[Assert]:{0}{1}", logString, stackTrace), Color.blue);
                break;
            case LogType.Exception:
                DebugConsole.Log(string.Format("[Exception]:{0}{1}", logString, stackTrace), Color.magenta);
                break;
        }
    }

    private StringBuilder m_LogBuilder = new StringBuilder();

    public static bool ExistsApi(string method)
    {
        var t = typeof(Main);
        var mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (null != mi) {
            return true;
        }
        return false;
    }
    public static StartupScript.ApiDelegation GetApi(string method)
    {
        var t = typeof(Main);
        var mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (null != mi) {
            var delegation = System.Delegate.CreateDelegate(typeof(StartupScript.ApiDelegation), mi, false);
            if (null != delegation)
                return (StartupScript.ApiDelegation)delegation;
            else
                return (BoxedValueList args) => { object o = mi.Invoke(null, new object[] { args }); return BoxedValue.FromObject(o); };
        }
        return null;
    }

    public static void RunStartup(bool runGm)
    {
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
        bool needRun = false;
        for(int i = 0; i < StartupApi.c_max_startup_cfgs; ++i) {
            string file = Path.Combine(StartupScript.ScriptPath, string.Format("startup{0}.dsl", i));
            if (File.Exists(file)) {
                if (!needRun) {
                    StartupScript.TryInit();
                    StartupScript.ClearStartups();
                    needRun = true;
                }
                StartupScript.LoadStartup(file);
            }
        }
        if (needRun) {
            StartupScript.RunStartup(out var gmtxt);
            if (runGm) {
                RunInitGM(gmtxt);
            }
        }
        else {
            //Open later on demand
            //StartupScript.RunStartupOnlyCsharp();
            if (runGm) {
                RunInitGM(string.Empty);
            }
        }
#endif
    }
    public static void RunInitGM(string gmtxt)
    {
        if (!string.IsNullOrEmpty(gmtxt)) {
            GmRootScript.TryInit();
            GameObject obj = GmRootScript.GameObj;
            if (null != obj) {
                DebugConsole.Execute(gmtxt);
            }
        }
        else {
            string path = Application.persistentDataPath;
            if (Application.platform == RuntimePlatform.Android) {
                path = "/data/local/tmp";
            }
            string file = Path.Combine(path, "initgm.txt");
            if (File.Exists(file)) {
                GmRootScript.TryInit();
                GameObject obj = GmRootScript.GameObj;
                if (null != obj) {
                    //DebugConsole.Execute will reset the dsl, so the commands must be concatenated into one command
                    var sb = new StringBuilder();
                    var lines = File.ReadAllLines(file);
                    foreach (string line in lines) {
                        string str = line.Trim();
                        if (!str.StartsWith("//") && !str.StartsWith('#')) {
                            if (str.IndexOf(';') < 0 && str.IndexOf('(') < 0 && str.IndexOf(')') < 0 && str.IndexOfAny(s_WhiteChars) > 0) {
                                sb.Append("cmd(\"");
                                sb.Append(str.Replace("\"", "\\\""));
                                sb.Append("\");");
                            }
                            else {
                                sb.Append(str);
                                if (!str.EndsWith(";"))
                                    sb.Append(";");
                            }
                        }
                    }
                    DebugConsole.Execute(sb.ToString());
                }
            }
            else {
#if UNITY_EDITOR
                GmRootScript.TryInit();
#elif UNITY_STANDALONE
#if DEVELOPMENT_BUILD
            GmRootScript.TryInit();
#endif
#elif UNITY_ANDROID
#if DEVELOPMENT_BUILD
            GmRootScript.TryInit();
#endif
#endif
            }
        }
    }

    public static SortedList<string, string> GetApiDocs()
    {
        return StartupScript.Calculator.ApiDocs;
    }
    public static void LoadScript(string file)
    {
        var basePath = Application.persistentDataPath;
        if (!System.IO.Path.IsPathRooted(file)) {
            file = System.IO.Path.Combine(basePath, file);
        }
        StartupScript.Calculator.LoadDsl(file);
    }
    public static BoxedValue EvalAndRun(string code)
    {
        BoxedValue r = BoxedValue.EmptyString;
        var file = new Dsl.DslFile();
        if (file.LoadFromString(code, msg => Debug.LogWarning(msg))) {
            r = EvalAndRun(file.DslInfos);
        }
        return r;
    }
    public static BoxedValue EvalAndRun(params ISyntaxComponent[] expressions)
    {
        IList<ISyntaxComponent> exps = expressions;
        return EvalAndRun(exps);
    }
    public static BoxedValue EvalAndRun(IList<ISyntaxComponent> expressions)
    {
        BoxedValue r = BoxedValue.EmptyString;
        List<IExpression> exps = new List<IExpression>();
        StartupScript.Calculator.LoadDsl(expressions, exps);
        r = StartupScript.Calculator.CalcInCurrentContext(exps);
        return r;
    }
    public static void EvalAsFunc(string name, Dsl.FunctionData func, IList<string> argNames)
    {
        Debug.Assert(null != func);
        StartupScript.Calculator.LoadDsl(name, argNames, func);
    }
    public static bool TryGetApiFactory(string name, out IExpressionFactory factory)
    {
        return StartupScript.Calculator.ApiRegistry.TryGetFactory(name, out factory);
    }
    public static object Call(string func, params object[] args)
    {
        if (null == s_Instance)
            return null;
        var vargs = StartupScript.Calculator.NewCalculatorValueList();
        foreach(var arg in args) {
            vargs.Add(BoxedValue.FromObject(arg));
        }
        var r = StartupScript.Calculator.Calc(func, vargs);
        StartupScript.Calculator.RecycleCalculatorValueList(vargs);
        return r.GetObject();
    }

    private static Main s_Instance = null;
    private static char[] s_WhiteChars = new char[] { ' ', '\t' };
}

namespace StoryApi
{
    internal static class MemoryInfo
    {
        internal static float GetAppMemory()
        {
#if UNITY_ANDROID
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            float ret = s_MemoryActivity.CallStatic<int>("getTotalPss")/1024.0f;
            return ret;
#elif UNITY_IOS
            return ios_GetAppMemory();
#else
            return 0;
#endif
        }
        internal static float GetNativeMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.native-heap")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.native-heap")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetGraphicsMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.graphics")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.graphics")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetUnknownMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.private-other")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.private-other")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetJavaMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.java-heap")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.java-heap")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetCodeMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.code")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.code")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetStackMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.stack")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.stack")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetSystemMemory()
        {
#if UNITY_ANDROID
            /*
            if (null == s_DebugObj) {
                s_DebugObj = new AndroidJavaObject("android.os.Debug");
            }
            if (null == s_MemoryInfoObj) {
                s_MemoryInfoObj = new AndroidJavaObject("android.os.Debug$MemoryInfo");
            }
            s_DebugObj.CallStatic("getMemoryInfo", s_MemoryInfoObj);
            return int.Parse(s_MemoryInfoObj.Call<string>("getMemoryStat", "summary.system")) / 1024.0f;
            */
            if (null == s_MemoryActivity) {
                s_MemoryActivity = new AndroidJavaClass("com.aaa.bbb.MainActivity");
            }
            var infos = s_MemoryActivity.CallStatic<AndroidJavaObject[]>("getMemoryInfo");
            float mem = 0;
            if (null != infos && infos.Length > 0) {
                mem = int.Parse(infos[0].Call<string>("getMemoryStat", "summary.system")) / 1024.0f;
            }
            return mem;
#else
            return 0;
#endif
        }
        internal static float GetVssMemory()
        {
            float vss = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            using (FileStream reader = new FileStream("/proc/self/status", FileMode.Open, FileAccess.Read, FileShare.Read, s_VssBuffer.Length, FileOptions.SequentialScan)) {
                int ct = reader.Read(s_VssBuffer, 0, s_VssBuffer.Length);
                int index = -1;
                int k = 0;
                for (int i = 0; i < ct && k < s_VmPeak.Length; ++i) {
                    if (s_VssBuffer[i] == s_VmPeak[k]) {
                        ++k;
                        if (k == s_VmPeak.Length) {
                            index = i + 1;
                            break;
                        }
                    }
                    else {
                        k = 0;
                    }
                }
                if (index >= 0) {
                    for (; index < ct && (s_VssBuffer[index] == ' ' || s_VssBuffer[index] == '\t'); ++index) ;
                    int vssKb = 0;
                    for (; index < ct; ++index) {
                        int num = s_VssBuffer[index] - '0';
                        if (num < 0 || num > 9)
                            break;

                        vssKb = vssKb * 10 + num;
                    }
                    vss = vssKb / 1024.0f;
                }
                reader.Close();
            }
#endif
            return vss;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        private static byte[] s_VssBuffer = new byte[1024];
        private static byte[] s_VmPeak = new byte[] { (byte)'V', (byte)'m', (byte)'P', (byte)'e', (byte)'a', (byte)'k', (byte)':' };
#endif
#if UNITY_ANDROID
        private static AndroidJavaClass s_MemoryActivity = null;
        private static AndroidJavaObject s_DebugObj = null;
        private static AndroidJavaObject s_MemoryInfoObj = null;
#endif
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern float ios_GetAppMemory();
#endif
    }
}

namespace ExpressionAPI
{
    using MemoryInfo = StoryApi.MemoryInfo;

    internal sealed class CopyPdfExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count >= 3) {
                string file = operands[0].GetString();
                int start = operands[1].GetInt();
                int count = operands[2].GetInt();
                CopyPdf(file, start, count);
            }
            return BoxedValue.NullObject;
        }
        private void CopyPdf(string file, int start, int count)
        {
            var basePath = Application.persistentDataPath;
            if (!System.IO.Path.IsPathRooted(file)) {
                file = System.IO.Path.Combine(basePath, file);
            }
            Debug.LogFormat("read pdf {0}.", file);
            var sb = new StringBuilder();
            var reader = new iTextSharp.text.pdf.PdfReader(file);
            for (int page = start; page < start + count && page <= reader.NumberOfPages; ++page) {
                try {
                    var txt = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                    sb.AppendLine(txt);
                }
                catch {
                    Debug.LogErrorFormat("page {0} read failed !", page);
                }
            }
            reader.Close();
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = sb.ToString();
#elif UNITY_IOS
            SetClipboard(sb.ToString());
#elif UNITY_ANDROID
            AndroidJavaClass cb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");
            cb.CallStatic ("setText", sb.ToString());
#endif
        }
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void SetClipboard(string str);
#endif
    }
    internal sealed class ShowMemoryExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            string info = string.Format("pss:{0} n:{1} g:{2} u:{3} j:{4} c:{5} t:{6} s:{7} vss:{8}", MemoryInfo.GetAppMemory(), MemoryInfo.GetNativeMemory(), MemoryInfo.GetGraphicsMemory(), MemoryInfo.GetUnknownMemory(), MemoryInfo.GetJavaMemory(), MemoryInfo.GetCodeMemory(), MemoryInfo.GetStackMemory(), MemoryInfo.GetSystemMemory(), MemoryInfo.GetVssMemory());
            Debug.LogFormat("{0}", info);
            return BoxedValue.NullObject;
        }
    }
    internal sealed class JavaClassExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count >= 1) {
                var p1 = operands[0];
                var obj = p1.As<AndroidJavaClass>();
                if (null != obj) {
                    return BoxedValue.FromObject(new JavaClass(obj));
                }
                else {
                    var str = p1.AsString;
                    return BoxedValue.FromObject(new JavaClass(str));
                }
            }
            return BoxedValue.NullObject;
        }
    }
    internal sealed class JavaObjectExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count > 0) {
                var obj = operands[0].As<AndroidJavaClass>();
                if (null != obj) {
                    return BoxedValue.FromObject(new JavaObject(obj));
                }
                else {
                    var str = operands[0].AsString;
                    var al = new ArrayList();
                    for (int i = 1; i < operands.Count; ++i) {
                        al.Add(operands[i].GetObject());
                    }
                    if (!string.IsNullOrEmpty(str)) {
                        return BoxedValue.FromObject(new JavaObject(str, al.ToArray()));
                    }
                }
            }
            return BoxedValue.NullObject;
        }
    }
    internal sealed class JavaProxyExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count >= 2) {
                var _class = operands[0].GetString();
                var scpMethod = operands[1].GetString();
                return BoxedValue.FromObject(new JavaProxy(_class, scpMethod));
            }
            return BoxedValue.NullObject;
        }
    }
    internal sealed class ObjectcClassExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count >= 1) {
                var str = operands[0].GetString();
                return BoxedValue.FromObject(new ObjectcClass(str));
            }
            return BoxedValue.NullObject;
        }
    }
    internal sealed class ObjectcObjectExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            if (operands.Count >= 1) {
                int objId = operands[0].GetInt();
                return BoxedValue.FromObject(new ObjectcObject(objId));
            }
            return BoxedValue.NullObject;
        }
    }
    internal sealed class GetPssExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetAppMemory();
        }
    }
    internal sealed class GetVssExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetVssMemory();
        }
    }
    internal sealed class GetNativeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetNativeMemory();
        }
    }
    internal sealed class GetGraphicsExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetGraphicsMemory();
        }
    }
    internal sealed class GetUnknownExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetUnknownMemory();
        }
    }
    internal sealed class GetJavaExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetJavaMemory();
        }
    }
    internal sealed class GetCodeExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetCodeMemory();
        }
    }
    internal sealed class GetStackExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetStackMemory();
        }
    }
    internal sealed class GetSystemExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return MemoryInfo.GetSystemMemory();
        }
    }
    internal sealed class GetActivityExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
#if UNITY_ANDROID
            return BoxedValue.FromObject(new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
#else
            return BoxedValue.NullObject;
#endif
        }
    }
    internal sealed class GetIntentExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
#if UNITY_ANDROID
            var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            return BoxedValue.FromObject(act.Call<AndroidJavaObject>("getIntent"));
#else
            return BoxedValue.NullObject;
#endif
        }
    }

    internal sealed class SetClipboardExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            var r = BoxedValue.NullObject;
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (null != str) {
#if UNITY_EDITOR
                    GUIUtility.systemCopyBuffer = str;
#elif UNITY_IOS
                    SetClipboard(str);
#elif UNITY_ANDROID
                    AndroidJavaClass cb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");
                    cb.CallStatic ("setText", str);
#endif
                    r = str;
                }
            }
            return r;
        }
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void SetClipboard(string str);
#endif
    }
    internal sealed class GetClipboardExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            var r = BoxedValue.NullObject;
#if UNITY_EDITOR
            r = GUIUtility.systemCopyBuffer;
#elif UNITY_IOS
            r = GetClipboard();
#elif UNITY_ANDROID
            AndroidJavaClass cb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");
            r = cb.CallStatic<string>("getText");
#endif
            return r;
        }
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern string GetClipboard();
#endif
    }
}
