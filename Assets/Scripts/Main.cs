﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using DslExpression;
using ExpressionAPI;

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

        m_Calculator.Init();
        m_Calculator.Register("copypdf", new ExpressionFactoryHelper<CopyPdfExp>());
        m_Calculator.Register("setclipboard", new ExpressionFactoryHelper<SetClipboardExp>());
        m_Calculator.Register("getclipboard", new ExpressionFactoryHelper<GetClipboardExp>());
        m_Calculator.Register("jc", new ExpressionFactoryHelper<JavaClassExp>());
        m_Calculator.Register("jo", new ExpressionFactoryHelper<JavaObjectExp>());
        m_Calculator.Register("jp", new ExpressionFactoryHelper<JavaProxyExp>());
        m_Calculator.Register("oc", new ExpressionFactoryHelper<ObjectcClassExp>());
        m_Calculator.Register("oo", new ExpressionFactoryHelper<ObjectcObjectExp>());
        m_Calculator.Register("getpss", new ExpressionFactoryHelper<GetPssExp>());
        m_Calculator.Register("getvss", new ExpressionFactoryHelper<GetVssExp>());
        m_Calculator.Register("getnative", new ExpressionFactoryHelper<GetNativeExp>());
        m_Calculator.Register("getgraphics", new ExpressionFactoryHelper<GetGraphicsExp>());
        m_Calculator.Register("getunknown", new ExpressionFactoryHelper<GetUnknownExp>());
        m_Calculator.Register("getjava", new ExpressionFactoryHelper<GetJavaExp>());
        m_Calculator.Register("getcode", new ExpressionFactoryHelper<GetCodeExp>());
        m_Calculator.Register("getstack", new ExpressionFactoryHelper<GetStackExp>());
        m_Calculator.Register("getsystem", new ExpressionFactoryHelper<GetSystemExp>());
        m_Calculator.Register("showmemory", new ExpressionFactoryHelper<ShowMemoryExp>());
        m_Calculator.Register("allocmemory", new ExpressionFactoryHelper<AllocMemoryExp>());
        m_Calculator.Register("freememory", new ExpressionFactoryHelper<FreeMemoryExp>());
        m_Calculator.Register("allochglobal", new ExpressionFactoryHelper<AllocHGlobalExp>());
        m_Calculator.Register("freehglobal", new ExpressionFactoryHelper<FreeHGlobalExp>());
        m_Calculator.Register("unloadunused", new ExpressionFactoryHelper<UnloadUnusedExp>());
        m_Calculator.Register("cms", new ExpressionFactoryHelper<CaptureMemorySnapshotExp>());
        m_Calculator.Register("loggc", new ExpressionFactoryHelper<LogGcExp>());
        m_Calculator.Register("setloggcsize", new ExpressionFactoryHelper<SetLogGcSizeExp>());
        m_Calculator.Register("setlognativesize", new ExpressionFactoryHelper<SetLogNativeSizeExp>());

        StartCoroutine(Loop());
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
        DebugConsole.Log("[" + type.ToString() + "]" + logString);
    }

    private void OnExecCommand(string cmd)
    {
        Dsl.DslFile file = new Dsl.DslFile();
        if(file.LoadFromString(string.Format("script(){{{0};}};", cmd), "cmd", msg => Debug.LogWarning(msg))) {
            m_Calculator.LoadDsl("main", file.DslInfos[0].First);
            var r = m_Calculator.Calc("main");
            if (null != r) {
                DebugConsole.Log(string.Format("result:{0}", r.ToString()));
            }
            else {
                DebugConsole.Log("result:null");
            }
        }
    }

    private void OnScript(string file)
    {
        var basePath = Application.persistentDataPath;
        if (!System.IO.Path.IsPathRooted(file)) {
            file = System.IO.Path.Combine(basePath, file);
        }
        m_Calculator.LoadDsl(file);
    }

    private void OnReset()
    {
        m_Calculator.Cleanup();
    }

    private void OnCommand(string cmd)
    {
        var vals = cmd.Split(' ', ',', ';', '|');
        string proc = vals[0];
        ArrayList al = new ArrayList(vals);
        al.RemoveAt(0);
        Call(proc, al.ToArray());
    }

    private void OnLog(string info)
    {
        DebugConsole.Log(info);
    }

    private StringBuilder m_LogBuilder = new StringBuilder();
    private DslExpression.DslCalculator m_Calculator = new DslExpression.DslCalculator();
    private Dictionary<string, object> m_KeyValues = new Dictionary<string, object>();

    public static object Call(string proc, params object[] args)
    {
        if (null == s_Instance)
            return null;
        object r = s_Instance.m_Calculator.Calc(proc, args);
        return r;
    }
    public static void AddKeyValue(string key, object val)
    {
        s_Instance.m_KeyValues[key] = val;
    }
    public static void RemoveKeyValue(string key)
    {
        s_Instance.m_KeyValues.Remove(key);
    }
    public static bool TryGetValue(string key, out object val)
    {
        return s_Instance.m_KeyValues.TryGetValue(key, out val);
    }
    private static Main s_Instance = null;
}

namespace ExpressionAPI
{
    internal sealed class CopyPdfExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            string file = m_File.Calc() as string;
            int start = ToInt(m_Start.Calc());
            int count = ToInt(m_Count.Calc());
            CopyPdf(file, start, count);
            return count;
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_File = exps[0];
            m_Start = exps[1];
            m_Count = exps[2];
            return true;
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
                } catch {
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

        private IExpression m_File;
        private IExpression m_Start;
        private IExpression m_Count;
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void SetClipboard(string str);
#endif
    }
    internal sealed class SetClipboardExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
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
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
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
    internal sealed class JavaClassExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                r = new JavaClass(str);
            }
            return r;
        }
    }
    internal sealed class JavaObjectExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                var al = new ArrayList();
                for(int i = 1; i < operands.Count; ++i) {
                    al.Add(operands[i]);
                }
                r = new JavaObject(str, al.ToArray());
            }
            return r;
        }
    }
    internal sealed class JavaProxyExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 2) {
                var _class = operands[0] as string;
                var scpMethod = operands[1] as string;
                r = new JavaProxy(_class, scpMethod);
            }
            return r;
        }
    }
    internal sealed class ObjectcClassExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                r = new ObjectcClass(str);
            }
            return r;
        }
    }
    internal sealed class ObjectcObjectExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                int objId = (int)System.Convert.ChangeType(operands[0], typeof(int));
                return new ObjectcObject(objId);
            }
            return r;
        }
    }
    internal sealed class GetPssExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetAppMemory();
        }
    }
    internal sealed class GetVssExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetVssMemory();
        }
    }
    internal sealed class GetNativeExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetNativeMemory();
        }
    }
    internal sealed class GetGraphicsExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetGraphicsMemory();
        }
    }
    internal sealed class GetUnknownExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetUnknownMemory();
        }
    }
    internal sealed class GetJavaExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetJavaMemory();
        }
    }
    internal sealed class GetCodeExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetCodeMemory();
        }
    }
    internal sealed class GetStackExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetStackMemory();
        }
    }
    internal sealed class GetSystemExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            return MemoryInfo.GetSystemMemory();
        }
    }
    internal sealed class ShowMemoryExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            string info = string.Format("pss:{0} n:{1} g:{2} u:{3} j:{4} c:{5} t:{6} s:{7} vss:{8}", MemoryInfo.GetAppMemory(), MemoryInfo.GetNativeMemory(), MemoryInfo.GetGraphicsMemory(), MemoryInfo.GetUnknownMemory(), MemoryInfo.GetJavaMemory(), MemoryInfo.GetCodeMemory(), MemoryInfo.GetStackMemory(), MemoryInfo.GetSystemMemory(), MemoryInfo.GetVssMemory());
            Debug.LogFormat("{0}", info);
            return info;
        }
    }
    internal sealed class AllocMemoryExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 2) {
                string key = operands[0] as string;
                int size = (int)System.Convert.ChangeType(operands[1], typeof(int));
                if (null != key) {
                    byte[] m = new byte[size];
                    for (int i = 0; i < size; ++i) {
                        m[i] = (byte)i;
                    }
                    Main.AddKeyValue(key, m);
                    r = key;
                }
            }
            return r;
        }
    }
    internal sealed class FreeMemoryExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                string key = operands[0] as string;
                if (null != key) {
                    Main.RemoveKeyValue(key);
                    System.GC.Collect();
                    r = key;
                }
            }
            return r;
        }
    }
    internal sealed class AllocHGlobalExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 2) {
                string key = operands[0] as string;
                int size = (int)System.Convert.ChangeType(operands[1], typeof(int));
                if (null != key) {
                    System.IntPtr m = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                    unsafe {
                        byte* ptr = (byte*)m;
                        for (int i = 0; i < size; ++i) {
                            ptr[i] = (byte)i;
                        }
                    }
                    Main.AddKeyValue(key, m);
                    r = key;
                }
            }
            return r;
        }
    }
    internal sealed class FreeHGlobalExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                string key = operands[0] as string;
                if (null != key) {
                    object v;
                    if(Main.TryGetValue(key, out v)) {
                        Main.RemoveKeyValue(key);
                        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)v);
                    }
                    System.GC.Collect();
                    r = key;
                }
            }
            return r;
        }
    }
    internal sealed class UnloadUnusedExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = true;
            for(int i = 0; i < 8; ++i) {
                System.GC.Collect();
            }
            Resources.UnloadUnusedAssets();            
            return r;
        }
    }
    internal sealed class CaptureMemorySnapshotExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = false;
            if (operands.Count >= 2) {
                string file = operands[0] as string;
                uint flags = (uint)System.Convert.ChangeType(operands[1], typeof(uint));
                if (null != file) {
                    if (System.IO.Path.GetExtension(file) != ".snap")
                        file = System.IO.Path.ChangeExtension(file, ".snap");
                    if (!System.IO.Path.IsPathRooted(file)) {
                        file = System.IO.Path.Combine(Application.persistentDataPath, file);
                    }
                    UnityHacker.CaptureMemorySnapshot(file, flags);
#if UNITY_ANDROID
                    string dirName = System.IO.Path.GetDirectoryName(file);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    File.WriteAllText(System.IO.Path.Combine(dirName, fileName + "_maps.txt"), File.ReadAllText("/proc/self/maps"));
                    File.WriteAllText(System.IO.Path.Combine(dirName, fileName + "_smaps.txt"), File.ReadAllText("/proc/self/smaps"));
#endif
                    r = true;
                }
            }
            return r;
        }
    }
    internal sealed class LogGcExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = false;
            if (operands.Count >= 1) {
                int val = (int)System.Convert.ChangeType(operands[0], typeof(int));
                string file = string.Empty;
                if (operands.Count >= 2)
                    file = operands[1] as string;
                if (val != 0) {
                    if (!string.IsNullOrEmpty(file)) {
                        if (System.IO.Path.GetExtension(file) != ".txt")
                            file = System.IO.Path.ChangeExtension(file, ".txt");
                        if (!System.IO.Path.IsPathRooted(file)) {
                            file = System.IO.Path.Combine(Application.persistentDataPath, file);
                        }
#if UNITY_ANDROID
                        string dirName = System.IO.Path.GetDirectoryName(file);
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        File.WriteAllText(System.IO.Path.Combine(dirName, fileName + "_maps.txt"), File.ReadAllText("/proc/self/maps"));
#endif
                        UnityHacker.StartGcLogger(file);
                        r = true;
                    }
                }
                else {
                    UnityHacker.StopGcLogger();
                    r = true;
                }
            }
            return r;
        }
    }
    internal sealed class SetLogGcSizeExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = false;
            if (operands.Count >= 2) {
                uint minSize = (uint)System.Convert.ChangeType(operands[0], typeof(uint));
                uint maxSize = (uint)System.Convert.ChangeType(operands[1], typeof(uint));
                UnityHacker.SetLogGcAllocSize(minSize, maxSize);
                r = true;
            }
            return r;
        }
    }
    internal sealed class SetLogNativeSizeExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = false;
            if (operands.Count >= 2) {
                uint minSize = (uint)System.Convert.ChangeType(operands[0], typeof(uint));
                uint maxSize = (uint)System.Convert.ChangeType(operands[1], typeof(uint));
                UnityHacker.SetLogNativeAllocSize(minSize, maxSize);
                r = true;
            }
            return r;
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
            using (System.IO.StreamReader reader = System.IO.File.OpenText("/proc/self/status")) {
                string fileBuffer = reader.ReadToEnd();
                int index = fileBuffer.IndexOf("VmPeak:");
                index += "VmPeak:".Length;
                for (; fileBuffer[index] == ' ' || fileBuffer[index] == '\t'; ++index) ;
                int vssKb = 0;
                for (; ; ++index) {
                    int num = fileBuffer[index] - '0';
                    if (num < 0 || num > 9)
                        break;

                    vssKb = vssKb * 10 + num;
                }
                vss = vssKb/1024.0f;
            }
#endif
            return vss;
        }
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
