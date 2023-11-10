using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using DslExpression;
using ExpressionAPI;
using Dsl;
using StoryScript;
using GmCommands;
using StoryApi;

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

        m_Calculator.OnLog = msg => { Debug.LogErrorFormat("{0}", msg); };
        m_Calculator.Init();
        m_Calculator.Register("regstoryapi", new ExpressionFactoryHelper<RegisterStoryApiExp>());
        m_Calculator.Register("loadui", new ExpressionFactoryHelper<LoadUiExp>());
        m_Calculator.Register("showui", new ExpressionFactoryHelper<ShowUiExp>());
        m_Calculator.Register("hideui", new ExpressionFactoryHelper<HideUiExp>());
        m_Calculator.Register("cmd", new ExpressionFactoryHelper<CmdExp>());
        m_Calculator.Register("copypdf", new ExpressionFactoryHelper<CopyPdfExp>());
        m_Calculator.Register("setclipboard", new ExpressionFactoryHelper<SetClipboardExp>());
        m_Calculator.Register("getclipboard", new ExpressionFactoryHelper<GetClipboardExp>());
        m_Calculator.Register("jc", new ExpressionFactoryHelper<JavaClassExp>());
        m_Calculator.Register("jo", new ExpressionFactoryHelper<JavaObjectExp>());
        m_Calculator.Register("jp", new ExpressionFactoryHelper<JavaProxyExp>());
        m_Calculator.Register("oc", new ExpressionFactoryHelper<ObjectcClassExp>());
        m_Calculator.Register("oo", new ExpressionFactoryHelper<ObjectcObjectExp>());
        m_Calculator.Register("systeminfo", new ExpressionFactoryHelper<SystemInfoExp>());
        m_Calculator.Register("getdevicemodel", new ExpressionFactoryHelper<GetDeviceModelExp>());
        m_Calculator.Register("getdevicename", new ExpressionFactoryHelper<GetDeviceNameExp>());
        m_Calculator.Register("getdeviceuid", new ExpressionFactoryHelper<GetDeviceUidExp>());
        m_Calculator.Register("getprocessortype", new ExpressionFactoryHelper<GetProcessorTypeExp>());
        m_Calculator.Register("getos", new ExpressionFactoryHelper<GetOSExp>());
        m_Calculator.Register("getgfxname", new ExpressionFactoryHelper<GetGraphicsDeviceNameExp>());
        m_Calculator.Register("getgfxvendor", new ExpressionFactoryHelper<GetGraphicsDeviceVendorExp>());
        m_Calculator.Register("getgfxversion", new ExpressionFactoryHelper<GetGraphicsDeviceVersionExp>());
        m_Calculator.Register("getiosgeneration", new ExpressionFactoryHelper<GetIosGenerationExp>());
        m_Calculator.Register("getiosversion", new ExpressionFactoryHelper<GetIosVersionExp>());
        m_Calculator.Register("getiosvendor", new ExpressionFactoryHelper<GetIosVendorExp>());
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
        m_Calculator.Register("gc", new ExpressionFactoryHelper<GCExp>());
        m_Calculator.Register("getactivity", new ExpressionFactoryHelper<GetActivityExp>());
        m_Calculator.Register("getintent", new ExpressionFactoryHelper<GetIntentExp>());
        m_Calculator.Register("getstring", new ExpressionFactoryHelper<GetStringExp>());
        m_Calculator.Register("getstringarray", new ExpressionFactoryHelper<GetStringArrayExp>());
        m_Calculator.Register("getint", new ExpressionFactoryHelper<GetIntExp>());
        m_Calculator.Register("getintarray", new ExpressionFactoryHelper<GetIntArrayExp>());
        m_Calculator.Register("getlong", new ExpressionFactoryHelper<GetLongExp>());
        m_Calculator.Register("getlongarray", new ExpressionFactoryHelper<GetLongArrayExp>());

        StartCoroutine(Loop());

        OnExecCommand("setloggcsize(16,40960000);loggc(1);showmemory();loop(10){allocmemory('m'+$$,1024);};unloadunused();gc();showmemory();loggc(0);");
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
    private CalculatorValue OnExecCommand(string cmd)
    {
        Dsl.DslFile file = new Dsl.DslFile();
        if(file.LoadFromString(string.Format("script(){{{0};}};", cmd), msg => Debug.LogWarning(msg))) {
            var func = file.DslInfos[0] as Dsl.FunctionData;
            m_Calculator.LoadDsl("main", func);
            var r = m_Calculator.Calc("main");
            if (!r.IsNullObject) {
                DebugConsole.Log(string.Format("result:{0}", r.ToString()));
            }
            else {
                DebugConsole.Log("result:null");
            }
            return r;
        }
        return CalculatorValue.NullObject;
    }
    private void OnLoadScript(string file)
    {
        var basePath = Application.persistentDataPath;
        if (!System.IO.Path.IsPathRooted(file)) {
            file = System.IO.Path.Combine(basePath, file);
        }
        m_Calculator.LoadDsl(file);
    }
    private void OnReset()
    {
        m_Calculator.Clear();
    }

    private StringBuilder m_LogBuilder = new StringBuilder();
    private DslExpression.DslCalculator m_Calculator = new DslExpression.DslCalculator();
    private Dictionary<string, object> m_KeyValues = new Dictionary<string, object>();

    public static void Reset()
    {
        s_Instance.OnReset();
    }
    public static void LoadScript(string file)
    {
        s_Instance.OnLoadScript(file);
    }
    public static CalculatorValue ExecCommand(string cmd)
    {
        return s_Instance.OnExecCommand(cmd);
    }
    public static void ResetStory()
    {
        GameObject obj = GmRootScript.GameObj;
        if (null != obj) {
            obj.SendMessage("OnResetStory");
        }
    }
    public static void ExecStoryCommand(string cmd)
    {
        GameObject obj = GmRootScript.GameObj;
        if (null != obj) {
            obj.SendMessage("OnExecStoryCommand", cmd);
        }
    }
    public static void ExecStoryFile(string file)
    {
        GameObject obj = GmRootScript.GameObj;
        if (null != obj) {
            obj.SendMessage("OnExecStoryFile", file);
        }
    }

    public static CalculatorValue EvalAndRun(string code)
    {
        CalculatorValue r = CalculatorValue.EmptyString;
        var file = new Dsl.DslFile();
        if (file.LoadFromString(code, msg => Debug.LogWarning(msg))) {
            r = EvalAndRun(file.DslInfos);
        }
        return r;
    }
    public static CalculatorValue EvalAndRun(params ISyntaxComponent[] expressions)
    {
        IList<ISyntaxComponent> exps = expressions;
        return EvalAndRun(exps);
    }
    public static CalculatorValue EvalAndRun(IList<ISyntaxComponent> expressions)
    {
        CalculatorValue r = CalculatorValue.EmptyString;
        List<IExpression> exps = new List<IExpression>();
        s_Instance.m_Calculator.LoadDsl(expressions, exps);
        r = s_Instance.m_Calculator.CalcInCurrentContext(exps);
        return r;
    }
    public static void EvalAsFunc(string name, Dsl.FunctionData func, IList<string> argNames)
    {
        Debug.Assert(null != func);
        s_Instance.m_Calculator.LoadDsl(name, argNames, func);
    }
    public static object Call(string func, params object[] args)
    {
        if (null == s_Instance)
            return null;
        var vargs = s_Instance.m_Calculator.NewCalculatorValueList();
        foreach(var arg in args) {
            vargs.Add(CalculatorValue.FromObject(arg));
        }
        var r = s_Instance.m_Calculator.Calc(func, vargs);
        s_Instance.m_Calculator.RecycleCalculatorValueList(vargs);
        return r.GetObject();
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

public static class VariantValue
{
    public static BoxedValue ToBoxedValue(CalculatorValue other)
    {
        BoxedValue newVal = new BoxedValue();
        switch (other.Type) {
            case CalculatorValue.c_ObjectType:
                var objVal = other.ObjectVal;
                if (objVal is Vector2) {
                    newVal.Type = BoxedValue.c_Vector2Type;
                    newVal.Union.Vector2Val = (Vector2)other.ObjectVal;
                }
                else if (objVal is Vector3) {
                    newVal.Type = BoxedValue.c_Vector3Type;
                    newVal.Union.Vector3Val = (Vector3)other.ObjectVal;
                }
                else if (objVal is Vector4) {
                    newVal.Type = BoxedValue.c_Vector4Type;
                    newVal.Union.Vector4Val = (Vector4)other.ObjectVal;
                }
                else if (objVal is Quaternion) {
                    newVal.Type = BoxedValue.c_QuaternionType;
                    newVal.Union.QuaternionVal = (Quaternion)other.ObjectVal;
                }
                else if (objVal is Color) {
                    newVal.Type = BoxedValue.c_ColorType;
                    newVal.Union.ColorVal = (Color)other.ObjectVal;
                }
                else if (objVal is Color32) {
                    newVal.Type = BoxedValue.c_Color32Type;
                    newVal.Union.Color32Val = (Color32)other.ObjectVal;
                }
                else {
                    newVal.Type = BoxedValue.c_ObjectType;
                    newVal.ObjectVal = other.ObjectVal;
                }
                break;
            case CalculatorValue.c_StringType:
                newVal.Type = BoxedValue.c_StringType;
                newVal.StringVal = other.StringVal;
                break;
            case CalculatorValue.c_BoolType:
                newVal.Type = BoxedValue.c_BoolType;
                newVal.Union.BoolVal = other.Union.BoolVal;
                break;
            case CalculatorValue.c_CharType:
                newVal.Type = BoxedValue.c_CharType;
                newVal.Union.CharVal = other.Union.CharVal;
                break;
            case CalculatorValue.c_SByteType:
                newVal.Type = BoxedValue.c_SByteType;
                newVal.Union.SByteVal = other.Union.SByteVal;
                break;
            case CalculatorValue.c_ShortType:
                newVal.Type = BoxedValue.c_ShortType;
                newVal.Union.ShortVal = other.Union.ShortVal;
                break;
            case CalculatorValue.c_IntType:
                newVal.Type = BoxedValue.c_IntType;
                newVal.Union.IntVal = other.Union.IntVal;
                break;
            case CalculatorValue.c_LongType:
                newVal.Type = BoxedValue.c_LongType;
                newVal.Union.LongVal = other.Union.LongVal;
                break;
            case CalculatorValue.c_ByteType:
                newVal.Type = BoxedValue.c_ByteType;
                newVal.Union.ByteVal = other.Union.ByteVal;
                break;
            case CalculatorValue.c_UShortType:
                newVal.Type = BoxedValue.c_UShortType;
                newVal.Union.UShortVal = other.Union.UShortVal;
                break;
            case CalculatorValue.c_UIntType:
                newVal.Type = BoxedValue.c_UIntType;
                newVal.Union.UIntVal = other.Union.UIntVal;
                break;
            case CalculatorValue.c_ULongType:
                newVal.Type = BoxedValue.c_ULongType;
                newVal.Union.ULongVal = other.Union.ULongVal;
                break;
            case CalculatorValue.c_FloatType:
                newVal.Type = BoxedValue.c_FloatType;
                newVal.Union.FloatVal = other.Union.FloatVal;
                break;
            case CalculatorValue.c_DoubleType:
                newVal.Type = BoxedValue.c_DoubleType;
                newVal.Union.DoubleVal = other.Union.DoubleVal;
                break;
            case CalculatorValue.c_DecimalType:
                newVal.Type = BoxedValue.c_DecimalType;
                newVal.Union.DecimalVal = other.Union.DecimalVal;
                break;
        }
        return newVal;
    }
    public static CalculatorValue ToCalculatorValue(BoxedValue other)
    {
        CalculatorValue newVal = new CalculatorValue();
        switch (other.Type) {
            case BoxedValue.c_ObjectType:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.ObjectVal;
                break;
            case BoxedValue.c_StringType:
                newVal.Type = CalculatorValue.c_StringType;
                newVal.StringVal = other.StringVal;
                break;
            case BoxedValue.c_BoolType:
                newVal.Type = CalculatorValue.c_BoolType;
                newVal.Union.BoolVal = other.Union.BoolVal;
                break;
            case BoxedValue.c_CharType:
                newVal.Type = CalculatorValue.c_CharType;
                newVal.Union.CharVal = other.Union.CharVal;
                break;
            case BoxedValue.c_SByteType:
                newVal.Type = CalculatorValue.c_SByteType;
                newVal.Union.SByteVal = other.Union.SByteVal;
                break;
            case BoxedValue.c_ShortType:
                newVal.Type = CalculatorValue.c_ShortType;
                newVal.Union.ShortVal = other.Union.ShortVal;
                break;
            case BoxedValue.c_IntType:
                newVal.Type = CalculatorValue.c_IntType;
                newVal.Union.IntVal = other.Union.IntVal;
                break;
            case BoxedValue.c_LongType:
                newVal.Type = CalculatorValue.c_LongType;
                newVal.Union.LongVal = other.Union.LongVal;
                break;
            case BoxedValue.c_ByteType:
                newVal.Type = CalculatorValue.c_ByteType;
                newVal.Union.ByteVal = other.Union.ByteVal;
                break;
            case BoxedValue.c_UShortType:
                newVal.Type = CalculatorValue.c_UShortType;
                newVal.Union.UShortVal = other.Union.UShortVal;
                break;
            case BoxedValue.c_UIntType:
                newVal.Type = CalculatorValue.c_UIntType;
                newVal.Union.UIntVal = other.Union.UIntVal;
                break;
            case BoxedValue.c_ULongType:
                newVal.Type = CalculatorValue.c_ULongType;
                newVal.Union.ULongVal = other.Union.ULongVal;
                break;
            case BoxedValue.c_FloatType:
                newVal.Type = CalculatorValue.c_FloatType;
                newVal.Union.FloatVal = other.Union.FloatVal;
                break;
            case BoxedValue.c_DoubleType:
                newVal.Type = CalculatorValue.c_DoubleType;
                newVal.Union.DoubleVal = other.Union.DoubleVal;
                break;
            case BoxedValue.c_DecimalType:
                newVal.Type = CalculatorValue.c_DecimalType;
                newVal.Union.DecimalVal = other.Union.DecimalVal;
                break;
            case BoxedValue.c_Vector2Type:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.Vector2Val;
                break;
            case BoxedValue.c_Vector3Type:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.Vector3Val;
                break;
            case BoxedValue.c_Vector4Type:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.Vector4Val;
                break;
            case BoxedValue.c_QuaternionType:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.QuaternionVal;
                break;
            case BoxedValue.c_ColorType:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.ColorVal;
                break;
            case BoxedValue.c_Color32Type:
                newVal.Type = CalculatorValue.c_ObjectType;
                newVal.ObjectVal = other.Union.Color32Val;
                break;
        }
        return newVal;
    }
}

namespace StoryApi
{
    internal class CallScriptCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CallScriptCommand cmd = new CallScriptCommand();
            cmd.m_FuncName = m_FuncName;
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string func = m_FuncName;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            Main.Call(func, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    StoryValue val = new StoryValue();
                    val.InitFromDsl(callData.GetParam(i));
                    m_Args.Add(val);
                }
            }
            return true;
        }

        internal string m_FuncName = string.Empty;
        private List<IStoryValue> m_Args = new List<IStoryValue>();
    }
    internal sealed class CallScriptValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    for (int i = 0; i < callData.GetParamNum(); ++i) {
                        StoryValue val = new StoryValue();
                        val.InitFromDsl(callData.GetParam(i));
                        m_Args.Add(val);
                    }
                }
            }
        }
        public IStoryValue Clone()
        {
            CallScriptValue val = new CallScriptValue();
            val.m_FuncName = m_FuncName;
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue varg = m_Args[i];
                val.m_Args.Add(varg.Clone());
            }
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            string funcName = m_FuncName;
            m_HaveValue = true;
            if (!string.IsNullOrEmpty(funcName)) {
                ArrayList al = new ArrayList();
                foreach (var varg in m_Args) {
                    al.Add(varg.Value.GetObject());
                }
                m_Value = BoxedValue.FromObject(Main.Call(funcName, al.ToArray()));
            }
            else {
                m_Value = BoxedValue.NullObject;
            }
        }

        internal string m_FuncName = string.Empty;
        private List<IStoryValue> m_Args = new List<IStoryValue>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CallScriptCommandFactory : IStoryCommandFactory
    {
        public IStoryCommand Create()
        {
            var cmd = new CallScriptCommand();
            cmd.m_FuncName = m_Name;
            return cmd;
        }
        internal CallScriptCommandFactory(string name)
        {
            m_Name = name;
        }
        private string m_Name;
    }
    internal sealed class CallScriptValueFactory : IStoryValueFactory
    {
        public IStoryValue Build()
        {
            var cmd = new CallScriptValue();
            cmd.m_FuncName = m_Name;
            return cmd;
        }
        internal CallScriptValueFactory(string name)
        {
            m_Name = name;
        }
        private string m_Name;
    }
}

namespace ExpressionAPI
{
    internal sealed class RegisterStoryApiExp : AbstractExpression
    {
        protected override CalculatorValue DoCalc()
        {
            string name = string.Empty;
            if (null != m_FuncCall) {
                if (m_ArgNum == 1) {
                    name = m_Name;
                    var func = new Dsl.FunctionData();
                    func.AddParam(m_FuncCall);
                    Main.EvalAsFunc(name, func, m_Params);
                }
                else if (m_ArgNum == 2) {
                    name = m_NameExp.Calc().AsString;
                    var func = new Dsl.FunctionData();
                    func.AddParam(m_FuncCall);
                    Main.EvalAsFunc(name, func, m_Params);
                }
            }
            else {
                name = m_Name;
            }
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, name, new StoryApi.CallScriptCommandFactory(name));
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, name, new StoryApi.CallScriptValueFactory(name));
            return name;
        }
        protected override bool Load(FunctionData callData)
        {
            m_ArgNum = callData.GetParamNum();
            if (m_ArgNum == 1) {
                var func = callData.GetParam(0) as Dsl.FunctionData;
                if (null != func){
                    if (func.IsOperatorParamClass() && func.GetId() == "=>") {
                        m_Name = func.GetParamId(0);
                        m_FuncCall = func.GetParam(1) as Dsl.FunctionData;
                        if (null != m_FuncCall) {
                            foreach (var p in m_FuncCall.Params) {
                                m_Params.Add(p.GetId());
                            }
                        }
                    }
                    else if (func.IsParenthesisParamClass()) {
                        m_Name = func.GetId();
                        foreach (var p in func.Params) {
                            m_Params.Add(p.GetId());
                        }
                    }
                }
            }
            else if (m_ArgNum == 2) {
                m_NameExp = Calculator.Load(callData.GetParam(0));
                if (m_ArgNum > 1) {
                    m_FuncCall = callData.GetParam(1) as Dsl.FunctionData;
                    if (null != m_FuncCall) {
                        foreach (var p in m_FuncCall.Params) {
                            m_Params.Add(p.GetId());
                        }
                    }
                }
            }
            return true;
        }

        private int m_ArgNum = 0;
        private IExpression m_NameExp;
        private string m_Name = string.Empty;
        private Dsl.FunctionData m_FuncCall;
        private List<string> m_Params = new List<string>();
    }
    internal sealed class LoadUiExp : AbstractExpression
    {
        protected override CalculatorValue DoCalc()
        {
            var r = false;
            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.LoadUi(m_UiRes);
                    r = true;
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return r;
        }
        protected override bool Load(FunctionData callData)
        {
            m_UiRes = callData.GetParamId(0);
            return true;
        }

        private string m_UiRes = string.Empty;
    }
    internal sealed class ShowUiExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = false;
            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.ShowUi();
                    r = true;
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return r;
        }
    }
    internal sealed class HideUiExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = false;
            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.HideUi();
                    r = true;
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return r;
        }
    }
    internal sealed class CmdExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count > 0) {
                string cmd = operands[0].AsString;
                DebugConsole.Execute(cmd);
                r = cmd;
            }
            return r;
        }
    }
    internal sealed class CopyPdfExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if(operands.Count > 2) {
                string file = operands[0].AsString;
                int start = operands[1].GetInt();
                int count = operands[2].GetInt();
                CopyPdf(file, start, count);
                r = count;
            }
            return r;
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
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void SetClipboard(string str);
#endif
    }
    internal sealed class SetClipboardExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                var obj = operands[0].As<AndroidJavaClass>();
                if (null != obj) {
                    r = CalculatorValue.FromObject(new JavaClass(obj));
                }
                else {
                    var str = operands[0].AsString;
                    r = CalculatorValue.FromObject(new JavaClass(str));
                }
            }
            return r;
        }
    }
    internal sealed class JavaObjectExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                var obj = operands[0].As<AndroidJavaObject>();
                if (null != obj) {
                    r = CalculatorValue.FromObject(new JavaObject(obj));
                }
                else {
                    var str = operands[0].AsString;
                    var al = new ArrayList();
                    for (int i = 1; i < operands.Count; ++i) {
                        al.Add(operands[i].GetObject());
                    }
                    if (!string.IsNullOrEmpty(str)) {
                        r = CalculatorValue.FromObject(new JavaObject(str, al.ToArray()));
                    }
                }
            }
            return r;
        }
    }
    internal sealed class JavaProxyExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 2) {
                var _class = operands[0].AsString;
                var scpMethod = operands[1].AsString;
                r = CalculatorValue.FromObject(new JavaProxy(_class, scpMethod));
            }
            return r;
        }
    }
    internal sealed class ObjectcClassExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                r = CalculatorValue.FromObject(new ObjectcClass(str));
            }
            return r;
        }
    }
    internal sealed class ObjectcObjectExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                int objId = operands[0].GetInt();
                return CalculatorValue.FromObject(new ObjectcObject(objId));
            }
            return r;
        }
    }
    internal sealed class SystemInfoExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return typeof(SystemInfo);
        }
    }
    internal sealed class GetDeviceModelExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.deviceModel;
        }
    }
    internal sealed class GetDeviceNameExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.deviceName;
        }
    }
    internal sealed class GetDeviceUidExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }
    internal sealed class GetProcessorTypeExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.processorType;
        }
    }
    internal sealed class GetOSExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.operatingSystem;
        }
    }
    internal sealed class GetGraphicsDeviceNameExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.graphicsDeviceName;
        }
    }
    internal sealed class GetGraphicsDeviceVendorExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.graphicsDeviceVendor;
        }
    }
    internal sealed class GetGraphicsDeviceVersionExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return SystemInfo.graphicsDeviceVersion;
        }
    }
    internal sealed class GetIosGenerationExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
#if UNITY_ANDROID
            return 0;
#else
            return (int)UnityEngine.iOS.Device.generation;
#endif
        }
    }
    internal sealed class GetIosVersionExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
#if UNITY_ANDROID
            return string.Empty;
#else
            return UnityEngine.iOS.Device.systemVersion;
#endif
        }
    }
    internal sealed class GetIosVendorExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
#if UNITY_ANDROID
            return string.Empty;
#else
            return UnityEngine.iOS.Device.vendorIdentifier;
#endif
        }
    }
    internal sealed class GetPssExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetAppMemory();
        }
    }
    internal sealed class GetVssExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetVssMemory();
        }
    }
    internal sealed class GetNativeExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetNativeMemory();
        }
    }
    internal sealed class GetGraphicsExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetGraphicsMemory();
        }
    }
    internal sealed class GetUnknownExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetUnknownMemory();
        }
    }
    internal sealed class GetJavaExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetJavaMemory();
        }
    }
    internal sealed class GetCodeExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetCodeMemory();
        }
    }
    internal sealed class GetStackExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetStackMemory();
        }
    }
    internal sealed class GetSystemExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            return MemoryInfo.GetSystemMemory();
        }
    }
    internal sealed class ShowMemoryExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            string info = string.Format("pss:{0} n:{1} g:{2} u:{3} j:{4} c:{5} t:{6} s:{7} vss:{8}", MemoryInfo.GetAppMemory(), MemoryInfo.GetNativeMemory(), MemoryInfo.GetGraphicsMemory(), MemoryInfo.GetUnknownMemory(), MemoryInfo.GetJavaMemory(), MemoryInfo.GetCodeMemory(), MemoryInfo.GetStackMemory(), MemoryInfo.GetSystemMemory(), MemoryInfo.GetVssMemory());
            Debug.LogFormat("{0}", info);
            return info;
        }
    }
    internal sealed class AllocMemoryExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 2) {
                string key = operands[0].AsString;
                int size = operands[1].GetInt();
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                string key = operands[0].AsString;
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 2) {
                string key = operands[0].AsString;
                int size = operands[1].GetInt();
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
            if (operands.Count >= 1) {
                string key = operands[0].AsString;
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = true;
            for(int i = 0; i < 8; ++i) {
                System.GC.Collect();
            }
            Resources.UnloadUnusedAssets();            
            return r;
        }
    }
    internal sealed class CaptureMemorySnapshotExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = false;
            if (operands.Count >= 2) {
                string file = operands[0].AsString;
                uint flags = operands[1].GetUInt();
                if (null != file) {
                    if (string.IsNullOrEmpty(file)) {
                        file = string.Format("cms_{0}_snapshot", System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
                    }
                    if (System.IO.Path.GetExtension(file) != ".snap")
                        file = System.IO.Path.ChangeExtension(file, ".snap");
                    if (!System.IO.Path.IsPathRooted(file)) {
                        file = System.IO.Path.Combine(Application.persistentDataPath, file);
                    }
                    //UnityHacker.CaptureMemorySnapshot(file, flags);
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
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = false;
            if (operands.Count >= 1) {
                int val = operands[0].GetInt();
                string file = string.Empty;
                if (operands.Count >= 2)
                    file = operands[1].AsString;
                if (val != 0) {
                    if (string.IsNullOrEmpty(file)) {
                        file = string.Format("loggc_{0}_alloc", System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff"));
                    }
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
                        //UnityHacker.StartGcLogger(file);
                        r = true;
                    }
                }
                else {
                    //UnityHacker.StopGcLogger();
                    r = true;
                }
            }
            return r;
        }
    }
    internal sealed class SetLogGcSizeExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = false;
            if (operands.Count >= 2) {
                uint minSize = operands[0].GetUInt();
                uint maxSize = operands[1].GetUInt();
                //UnityHacker.SetLogGcAllocSize(minSize, maxSize);
                r = true;
            }
            return r;
        }
    }
    internal sealed class SetLogNativeSizeExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = false;
            if (operands.Count >= 2) {
                uint minSize = operands[0].GetUInt();
                uint maxSize = operands[1].GetUInt();
                //UnityHacker.SetLogNativeAllocSize(minSize, maxSize);
                r = true;
            }
            return r;
        }
    }
    internal sealed class GCExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            bool r = false;
            if (operands.Count >= 0) {
                System.GC.Collect(1);
                r = true;
            }
            return r;
        }
    }
    internal sealed class GetActivityExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            r = CalculatorValue.FromObject(new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"));
#endif
            return r;
        }
    }
    internal sealed class GetIntentExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            r = CalculatorValue.FromObject(act.Call<AndroidJavaObject>("getIntent"));
#endif
            return r;
        }
    }
    internal sealed class GetStringExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = intent.Call<string>("getStringExtra", str);
                }
            }
#endif
            return r;
        }
    }
    internal sealed class GetStringArrayExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = CalculatorValue.FromObject(intent.Call<string[]>("getStringArrayExtra", str));
                }
            }
#endif
            return r;
        }
    }
    internal sealed class GetIntExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = intent.Call<int>("getIntExtra", str);
                }
            }
#endif
            return r;
        }
    }
    internal sealed class GetIntArrayExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = CalculatorValue.FromObject(intent.Call<int[]>("getIntArrayExtra", str));
                }
            }
#endif
            return r;
        }
    }
    internal sealed class GetLongExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = intent.Call<long>("getLongExtra", str);
                }
            }
#endif
            return r;
        }
    }
    internal sealed class GetLongArrayExp : SimpleExpressionBase
    {
        protected override CalculatorValue OnCalc(IList<CalculatorValue> operands)
        {
            var r = CalculatorValue.NullObject;
#if UNITY_ANDROID
            if (operands.Count >= 1) {
                var str = operands[0].AsString;
                if (!string.IsNullOrEmpty(str)) {
                    var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                    var intent = act.Call<AndroidJavaObject>("getIntent");
                    r = CalculatorValue.FromObject(intent.Call<long[]>("getLongArrayExtra", str));
                }
            }
#endif
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
