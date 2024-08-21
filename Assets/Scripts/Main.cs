using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ExpressionAPI;
using Dsl;
using StoryScript;
using StoryScript.DslExpression;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        s_Instance = this;

        var txtWriter = new StringWriter(m_LogBuilder);
        System.Console.SetOut(txtWriter);
        System.Console.SetError(txtWriter);
        Application.logMessageReceived += HandleLog;
        StartCoroutine(Loop());

        PerfGradeGm.TryInit();
        UnityEditorApi.Register(PerfGradeGm.Calculator);
        PerfGradeGm.Calculator.Register("setclipboard", "setclipboard(str) api", new ExpressionFactoryHelper<SetClipboardExp>());
        PerfGradeGm.Calculator.Register("getclipboard", "getclipboard() api", new ExpressionFactoryHelper<GetClipboardExp>());
        TestPerfGrade();
        TestGM();
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

    public static void TestGM()
    {
        string path = Application.persistentDataPath;
        if (Application.platform == RuntimePlatform.Android) {
            path = "/data/local/tmp";
        }
        string file = System.IO.Path.Combine(path, "initgm.txt");
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
    }
    public static void TestPerfGrade()
    {
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
        PerfGradeGm.ClearPerfGradeScripts();
        for (int i = 0; i < PerfGrade.c_max_perf_grade_cfgs; ++i) {
            string file = System.IO.Path.Combine(PerfGradeGm.ScriptPath, string.Format("perf{0}.dsl", i));
            if (File.Exists(file)) {
                PerfGradeGm.LoadPerfGradeScript(file);
            }
        }
        PerfGradeGm.RunPerfGrade();
#endif
    }

    public static SortedList<string, string> GetApiDocs()
    {
        return PerfGradeGm.Calculator.ApiDocs;
    }
    public static void LoadScript(string file)
    {
        var basePath = Application.persistentDataPath;
        if (!System.IO.Path.IsPathRooted(file)) {
            file = System.IO.Path.Combine(basePath, file);
        }
        PerfGradeGm.Calculator.LoadDsl(file);
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
        PerfGradeGm.Calculator.LoadDsl(expressions, exps);
        r = PerfGradeGm.Calculator.CalcInCurrentContext(exps);
        return r;
    }
    public static void EvalAsFunc(string name, Dsl.FunctionData func, IList<string> argNames)
    {
        Debug.Assert(null != func);
        PerfGradeGm.Calculator.LoadDsl(name, argNames, func);
    }
    public static object Call(string func, params object[] args)
    {
        if (null == s_Instance)
            return null;
        var vargs = PerfGradeGm.Calculator.NewCalculatorValueList();
        foreach(var arg in args) {
            vargs.Add(BoxedValue.FromObject(arg));
        }
        var r = PerfGradeGm.Calculator.Calc(func, vargs);
        PerfGradeGm.Calculator.RecycleCalculatorValueList(vargs);
        return r.GetObject();
    }

    private static Main s_Instance = null;
    private static char[] s_WhiteChars = new char[] { ' ', '\t' };
}

namespace StoryApi
{
    internal sealed class CopyPdfCommand : SimpleStoryCommandBase<CopyPdfCommand, StoryValueParam<string, int, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, int, int> _params, long delta)
        {
            string file = _params.Param1Value;
            int start = _params.Param2Value;
            int count = _params.Param3Value;
            CopyPdf(file, start, count);
            return false;
        }
        private void CopyPdf(string file, int start, int count)
        {
            var basePath = Application.persistentDataPath;
            if (!System.IO.Path.IsPathRooted(file)) {
                file = System.IO.Path.Combine(basePath, file);
            }
            Debug.LogFormat("read pdf {0}.", file);
            var sb = new StringBuilder();
            PdfReader reader = new PdfReader(file);
            for (int page = start; page < start + count && page <= reader.NumberOfPages; ++page) {
                try {
                    var txt = PdfTextExtractor.GetTextFromPage(reader, page);
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
    internal sealed class ShowMemoryCommand : SimpleStoryCommandBase<ShowMemoryCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            string info = string.Format("pss:{0} n:{1} g:{2} u:{3} j:{4} c:{5} t:{6} s:{7} vss:{8}", MemoryInfo.GetAppMemory(), MemoryInfo.GetNativeMemory(), MemoryInfo.GetGraphicsMemory(), MemoryInfo.GetUnknownMemory(), MemoryInfo.GetJavaMemory(), MemoryInfo.GetCodeMemory(), MemoryInfo.GetStackMemory(), MemoryInfo.GetSystemMemory(), MemoryInfo.GetVssMemory());
            Debug.LogFormat("{0}", info);
            return false;
        }
    }

    internal sealed class JavaClassFunction : SimpleStoryFunctionBase<JavaClassFunction, StoryValueParam<BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue> _params, StoryValueResult result)
        {
            var p1 = _params.Param1Value;
            var obj = p1.As<AndroidJavaClass>();
            if (null != obj) {
                result.Value = BoxedValue.FromObject(new JavaClass(obj));
            }
            else {
                var str = p1.AsString;
                result.Value = BoxedValue.FromObject(new JavaClass(str));
            }
        }
    }
    internal sealed class JavaObjectFunction : SimpleStoryFunctionBase<JavaObjectFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            var operands = _params.Values;
            if (operands.Count > 0) {
                var obj = operands[0].As<AndroidJavaClass>();
                if (null != obj) {
                    result.Value = BoxedValue.FromObject(new JavaObject(obj));
                }
                else {
                    var str = operands[0].AsString;
                    var al = new ArrayList();
                    for (int i = 1; i < operands.Count; ++i) {
                        al.Add(operands[i].GetObject());
                    }
                    if (!string.IsNullOrEmpty(str)) {
                        result.Value = BoxedValue.FromObject(new JavaObject(str, al.ToArray()));
                    }
                }
            }
        }
    }
    internal sealed class JavaProxyFunction : SimpleStoryFunctionBase<JavaProxyFunction, StoryValueParam<string, string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, string> _params, StoryValueResult result)
        {
            var _class = _params.Param1Value;
            var scpMethod = _params.Param2Value;
            result.Value = BoxedValue.FromObject(new JavaProxy(_class, scpMethod));
        }
    }
    internal sealed class ObjectcClassFunction : SimpleStoryFunctionBase<ObjectcClassFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
            var str = _params.Param1Value;
            result.Value = BoxedValue.FromObject(new ObjectcClass(str));
        }
    }
    internal sealed class ObjectcObjectFunction : SimpleStoryFunctionBase<ObjectcObjectFunction, StoryValueParam<int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<int> _params, StoryValueResult result)
        {
            int objId = _params.Param1Value;
            result.Value = BoxedValue.FromObject(new ObjectcObject(objId));
        }
    }
    internal sealed class GetPssFunction : SimpleStoryFunctionBase<GetPssFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetAppMemory();
        }
    }
    internal sealed class GetVssFunction : SimpleStoryFunctionBase<GetVssFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetVssMemory();
        }
    }
    internal sealed class GetNativeFunction : SimpleStoryFunctionBase<GetNativeFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetNativeMemory();
        }
    }
    internal sealed class GetGraphicsFunction : SimpleStoryFunctionBase<GetGraphicsFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetGraphicsMemory();
        }
    }
    internal sealed class GetUnknownFunction : SimpleStoryFunctionBase<GetUnknownFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetUnknownMemory();
        }
    }
    internal sealed class GetJavaFunction : SimpleStoryFunctionBase<GetJavaFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetJavaMemory();
        }
    }
    internal sealed class GetCodeFunction : SimpleStoryFunctionBase<GetCodeFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetCodeMemory();
        }
    }
    internal sealed class GetStackFunction : SimpleStoryFunctionBase<GetStackFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetStackMemory();
        }
    }
    internal sealed class GetSystemFunction : SimpleStoryFunctionBase<GetSystemFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = MemoryInfo.GetSystemMemory();
        }
    }
    internal sealed class GetActivityFunction : SimpleStoryFunctionBase<GetActivityFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            result.Value = BoxedValue.FromObject(new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
#endif
        }
    }
    internal sealed class GetIntentFunction : SimpleStoryFunctionBase<GetIntentFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            result.Value = BoxedValue.FromObject(act.Call<AndroidJavaObject>("getIntent"));
#endif
        }
    }

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
