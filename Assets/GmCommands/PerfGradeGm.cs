using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dsl;
using System.IO;
using StoryScript;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using System;
using StoryScript.DslExpression;

internal class PerfGradeApiExp : SimpleExpressionBase
{
    internal PerfGradeApiExp(string method)
    {
        m_Method = method;
        if(!PerfGradeGm.GetApi(method, out m_Api)) {
            LogSystem.Error("Can't find method '{0}'", method);
        }
    }
    protected override BoxedValue OnCalc(IList<BoxedValue> operands)
    {
        var list = PerfGradeGm.NewArgList();
        foreach (var operand in operands) {
            list.Add(operand);
        }
        try {
            BoxedValue r = BoxedValue.NullObject;
            if (null != m_Api) {
                r = m_Api(list);
            }
            return r;
        }
        finally {
            PerfGradeGm.RecycleArgList(list);
        }
    }

    private string m_Method = string.Empty;
    private PerfGrade.PerfApiDelegation m_Api;
}
public static class PerfGradeGm
{
    public static string ScriptPath
    {
        get {
            if(Application.isEditor) {
                return "Assets/../tools/perf_scripts/";
            }
            else if (Application.platform == RuntimePlatform.Android) {
                return "/data/local/tmp/";
            }
            else {
                return Application.persistentDataPath + "/";
            }
        }
    }
    public static void TryInit()
    {
        if (!s_Inited) {
            s_Calculator = new DslCalculator();
            s_Calculator.Init();
            s_Calculator.OnLog = LogSystem.Log;
            s_Calculator.OnLoadFailback = OnLoadFailback;
            s_Inited = true;
        }
    }
    public static void LogCompiledPerfGrades()
    {
        PerfGrade.Instance.LogCompiledPerfGrades();
    }
    public static void ClearPerfGradeScripts()
    {
        s_Calculator.Clear();
        s_PerfIds.Clear();
        s_Inits.Clear();
        s_Grades.Clear();
        s_DefGrades.Clear();
        s_Settings.Clear();
    }
    public static void LoadPerfGradeScript(string script_file)
    {
        int serialNumber = 0;

        string txt = File.ReadAllText(script_file);
        Dsl.DslFile dslFile = new DslFile();
        if(dslFile.LoadFromString(txt, msg => { LogSystem.Warn("{0}", msg); })) {
            foreach(var dslInfo in dslFile.DslInfos) {
                string perfSyntaxName = dslInfo.GetId();
                if (perfSyntaxName == "perf_grade") {
                    var perfGradeDsl = dslInfo as Dsl.FunctionData;
                    if (null != perfGradeDsl && perfGradeDsl.IsHighOrder && perfGradeDsl.HaveStatement()) {
                        int.TryParse(perfGradeDsl.LowerOrderFunction.GetParamId(0), out var perfId);
                        s_PerfIds.Add(perfId);

                        foreach(var block in perfGradeDsl.Params) {
                            var funcBlock = block as Dsl.FunctionData;
                            if (null != funcBlock) {
                                string funcId = funcBlock.GetId();
                                string fn = funcId + "_" + serialNumber;
                                ++serialNumber;
                                s_Calculator.LoadDsl(fn, funcBlock);
                                if (funcId == "init") {
                                    s_Inits.Add(fn);
                                }
                                else if(funcId == "grade") {
                                    if(funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if(!s_Grades.TryGetValue(g, out var list)) {
                                            list = new List<Tuple<string, string>>();
                                            s_Grades[g] = list;
                                        }
                                        list.Add(Tuple.Create(fn, script_file + ":" + funcBlock.GetLine()));
                                    }
                                    else {
                                        LogSystem.Error("grade must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                                else if (funcId == "default_grade") {
                                    if (funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if (!s_DefGrades.TryGetValue(g, out var list)) {
                                            list = new List<Tuple<string, string>>();
                                            s_DefGrades[g] = list;
                                        }
                                        list.Add(Tuple.Create(fn, script_file + ":" + funcBlock.GetLine()));
                                    }
                                    else {
                                        LogSystem.Error("default_grade must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                                else if (funcId == "setting") {
                                    if (funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if (!s_Settings.TryGetValue(g, out var list)) {
                                            list = new List<string>();
                                            s_Settings[g] = list;
                                        }
                                        list.Add(fn);
                                    }
                                    else {
                                        LogSystem.Error("setting must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                            }
                        }
                    }
                    else {
                        LogSystem.Error("perf_grade must have a integer id. syntax:{0} line:{1}", perfSyntaxName, dslInfo.GetLine());
                    }
                }
                else {
                    LogSystem.Error("only perf_grade can be toplevel element. syntax:{0} line:{1}", perfSyntaxName, dslInfo.GetLine());
                }
            }
        }
    }
    public static void RunPerfGrade()
    {
        s_Calculator.CheckFuncXrefs();
        PerfGrade.Instance.ClearAll();

        foreach (var perfId in s_PerfIds) {
            PerfGrade.Instance.AddOverridedScript(perfId);
        }

        PerfGrade.Instance.Init();
        foreach (var init in s_Inits) {
            s_Calculator.Calc(init);
        }

        var grade = PerfGrade.Instance.GetGrade();
        foreach (var pair in s_Grades) {
            int g = pair.Key;
            foreach (var tuple in pair.Value) {
                PerfGrade.Instance.GradeSetState = true;
                s_Calculator.Calc(tuple.Item1);
                if (PerfGrade.Instance.GradeSetState) {
                    grade = (PerfGrade.GradeEnum)g;
                    LogSystem.Warn("set grade:{0} from {1}", grade, tuple.Item2);
                    break;
                }
            }
            if (grade != PerfGrade.GradeEnum.Unknown) {
                break;
            }
        }

        if (grade == PerfGrade.GradeEnum.Unknown) {
            grade = PerfGrade.Instance.GetDefaultGrade();
            foreach (var pair in s_DefGrades) {
                int g = pair.Key;
                foreach (var tuple in pair.Value) {
                    PerfGrade.Instance.GradeSetState = true;
                    s_Calculator.Calc(tuple.Item1);
                    if (PerfGrade.Instance.GradeSetState) {
                        grade = (PerfGrade.GradeEnum)g;
                        LogSystem.Warn("set grade:{0} from {1}", grade, tuple.Item2);
                        break;
                    }
                }
                if (grade != PerfGrade.GradeEnum.Unknown) {
                    break;
                }
            }
        }

        LogSystem.Warn("final device grade:{0}", grade);

        PerfGrade.Instance.DoSetting(grade);
        if (s_Settings.TryGetValue((int)grade, out var codes)) {
            foreach (var code in codes) {
                s_Calculator.Calc(code);
            }
        }
    }
    public static void CompilePerfGradeScript(string script_file)
    {
#if UNITY_EDITOR
        string txt = File.ReadAllText(script_file);
        Dsl.DslFile dslFile = new DslFile();
        if (dslFile.LoadFromString(txt, msg => { LogSystem.Warn("{0}", msg); })) {
            foreach (var dslInfo in dslFile.DslInfos) {
                string perfSyntaxName = dslInfo.GetId();
                if (perfSyntaxName == "perf_grade") {
                    var perfGradeDsl = dslInfo as Dsl.FunctionData;
                    if (null != perfGradeDsl && perfGradeDsl.IsHighOrder && perfGradeDsl.HaveStatement()) {
                        var inits = new List<string>();
                        var grades = new SortedDictionary<int, List<string>>();
                        var defGrades = new SortedDictionary<int, List<string>>();
                        var settings = new SortedDictionary<int, List<string>>();
                        var gvars = new SortedSet<string>();
                        int serialNumber = 0;
                        int indent = 0;

                        s_IteratorStack.Clear();
                        s_NextId = 0;

                        var sb = new StringBuilder();
                        int.TryParse(perfGradeDsl.LowerOrderFunction.GetParamId(0), out var perfId);
                        sb.AppendLine("using System.Collections;");
                        sb.AppendLine("using System.Collections.Generic;");
                        sb.AppendLine("using System.Text;");
                        sb.AppendLine("using System.Text.RegularExpressions;");
                        sb.AppendLine("using UnityEngine;");
                        sb.AppendLine();
                        sb.AppendLine("partial class PerfGrade");
                        sb.AppendLine("{");
                        ++indent;
                        foreach (var block in perfGradeDsl.Params) {
                            var funcBlock = block as Dsl.FunctionData;
                            if (null != funcBlock) {
                                string funcId = funcBlock.GetId();
                                string tag = "_" + perfId + "_" + serialNumber;
                                ++serialNumber;
                                if (funcId == "init") {
                                    string fn = funcId + tag;
                                    inits.Add(fn);
                                    CompileFunction(fn, funcBlock, sb, indent, gvars);
                                }
                                else if (funcId == "grade") {
                                    if (funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if (!grades.TryGetValue(g, out var list)) {
                                            list = new List<string>();
                                            grades[g] = list;
                                        }
                                        string fn = funcId + tag + "_g" + g;
                                        list.Add(fn);
                                        CompileFunction(fn, funcBlock, sb, indent, gvars);
                                    }
                                    else {
                                        LogSystem.Error("grade must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                                else if (funcId == "default_grade") {
                                    if (funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if (!defGrades.TryGetValue(g, out var list)) {
                                            list = new List<string>();
                                            defGrades[g] = list;
                                        }
                                        string fn = funcId + tag + "_g" + g;
                                        list.Add(fn);
                                        CompileFunction(fn, funcBlock, sb, indent, gvars);
                                    }
                                    else {
                                        LogSystem.Error("default_grade must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                                else if (funcId == "setting") {
                                    if (funcBlock.IsHighOrder && funcBlock.HaveStatement()) {
                                        int.TryParse(funcBlock.LowerOrderFunction.GetParamId(0), out var g);
                                        if (!settings.TryGetValue(g, out var list)) {
                                            list = new List<string>();
                                            settings[g] = list;
                                        }
                                        string fn = funcId + tag + "_g" + g;
                                        list.Add(fn);
                                        CompileFunction(fn, funcBlock, sb, indent, gvars);
                                    }
                                    else {
                                        LogSystem.Error("setting must have a integer grade. syntax:{0} line:{1}", funcBlock, dslInfo.GetLine());
                                    }
                                }
                            }
                        }

                        sb.AppendLine();
                        sb.AppendLine("{0}GradeEnum combined_grade_{1}()", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}{{", Literal.GetIndentString(indent));
                        ++indent;
                        foreach (var pair in grades) {
                            int g = pair.Key;
                            foreach(var func in pair.Value) {
                                sb.AppendLine("{0}GradeSetState = true;", Literal.GetIndentString(indent));
                                sb.AppendLine("{0}{1}();", Literal.GetIndentString(indent), func);
                                sb.AppendLine("{0}if (GradeSetState) {{", Literal.GetIndentString(indent));
                                ++indent;
                                sb.AppendLine("{0}Debug.Log(\"set grade:{1} from {2}\");", Literal.GetIndentString(indent), (PerfGrade.GradeEnum)g, func);
                                sb.AppendLine("{0}return (GradeEnum){1};", Literal.GetIndentString(indent), g);
                                --indent;
                                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                            }
                        }
                        sb.AppendLine("{0}return GradeEnum.Unknown;", Literal.GetIndentString(indent));
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        sb.AppendLine("{0}GradeEnum combined_default_grade_{1}()", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}{{", Literal.GetIndentString(indent));
                        ++indent;
                        foreach (var pair in defGrades) {
                            int g = pair.Key;
                            foreach (var func in pair.Value) {
                                sb.AppendLine("{0}GradeSetState = true;", Literal.GetIndentString(indent));
                                sb.AppendLine("{0}{1}();", Literal.GetIndentString(indent), func);
                                sb.AppendLine("{0}if (GradeSetState) {{", Literal.GetIndentString(indent));
                                ++indent;
                                sb.AppendLine("{0}Debug.Log(\"set grade:{1} from {2}\");", Literal.GetIndentString(indent), (PerfGrade.GradeEnum)g, func);
                                sb.AppendLine("{0}return (GradeEnum){1};", Literal.GetIndentString(indent), g);
                                --indent;
                                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                            }
                        }
                        sb.AppendLine("{0}return GradeEnum.Unknown;", Literal.GetIndentString(indent));
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        sb.AppendLine("{0}void combined_setting_{1}(GradeEnum grade)", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}{{", Literal.GetIndentString(indent));
                        ++indent;
                        sb.AppendLine("{0}switch ((int)grade) {{", Literal.GetIndentString(indent));
                        ++indent;
                        foreach (var pair in settings) {
                            int g = pair.Key;
                            sb.AppendLine("{0}case {1}:", Literal.GetIndentString(indent), g);
                            ++indent;
                            foreach (var func in pair.Value) {
                                sb.AppendLine("{0}{1}();", Literal.GetIndentString(indent), func);
                            }
                            sb.AppendLine("{0}break;", Literal.GetIndentString(indent));
                            --indent;
                        }
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        sb.AppendLine();
                        sb.AppendLine("{0}partial void RegisterPerfGrade_{1}()", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}{{", Literal.GetIndentString(indent));
                        ++indent;
                        sb.AppendLine("{0}if (!ExistsScriptablePerfGrade({1})) {{", Literal.GetIndentString(indent), perfId);
                        ++indent;
                        foreach (var init in inits) {
                            sb.AppendLine("{0}{1}();", Literal.GetIndentString(indent), init);
                        }
                        sb.AppendLine("{0}m_GradeCodes.Add(combined_grade_{1});", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}m_DefaultGradeCodes.Add(combined_default_grade_{1});", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}m_SettingCodes.Add(combined_setting_{1});", Literal.GetIndentString(indent), perfId);
                        sb.AppendLine("{0}m_CompiledPerfGrades.Add({1});", Literal.GetIndentString(indent), perfId);
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        --indent;
                        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                        sb.AppendLine();
                        foreach (var v in gvars) {
                            sb.AppendLine("{0}private BoxedValue {1};", Literal.GetIndentString(indent), v);
                        }
                        --indent;
                        sb.AppendLine("}");
                        File.WriteAllText(string.Format("{0}perf_grade_{1}.cs", ScriptPath, perfId), sb.ToString());
                    }
                    else {
                        LogSystem.Error("perf_grade must have a integer id. syntax:{0} line:{1}", perfSyntaxName, dslInfo.GetLine());
                    }
                }
                else {
                    LogSystem.Error("only perf_grade can be toplevel element. syntax:{0} line:{1}", perfSyntaxName, dslInfo.GetLine());
                }
            }
        }
#endif
    }
    internal static bool GetApi(string method, out PerfGrade.PerfApiDelegation api)
    {
        bool r = false;
        api = PerfGrade.Instance.GetApi(method);
        if (null != api) {
            r = true;
        }
        return r;
    }
    internal static BoxedValueList NewArgList()
    {
        return s_BoxedValueListPool.Alloc();
    }
    internal static void RecycleArgList(BoxedValueList args)
    {
        s_BoxedValueListPool.Recycle(args);
    }
    private static bool OnLoadFailback(ISyntaxComponent comp, DslCalculator calculator, out IExpression expression)
    {
        bool ret = false;
        expression = null;

        var funcData = comp as Dsl.FunctionData;
        if (null != funcData) {
            //all perf grade apis are in the form of function calls.
            if (funcData.HaveParam()) {
                var callData = funcData;
                string fn = callData.GetId();
                if (PerfGrade.Instance.ExistsApi(fn)) {
                    var exp = new PerfGradeApiExp(fn);
                    if (exp.Load(funcData, calculator)) {
                        expression = exp;
                        ret = true;
                    }
                }
            }
        }
        return ret;
    }
    private static void CompileFunction(string funcName, Dsl.FunctionData funcData, StringBuilder sb, int indent, SortedSet<string> gvars)
    {
        var bodySb = new StringBuilder();
        sb.AppendLine("{0}void {1}()", Literal.GetIndentString(indent), funcName);
        sb.AppendLine("{0}{{", Literal.GetIndentString(indent));
        ++indent;
        SortedSet<string> lvars = new SortedSet<string>();
        foreach (var p in funcData.Params) {
            CompileSyntaxComponent(p, bodySb, indent, true, gvars, lvars);
        }
        foreach(var v in lvars) {
            sb.AppendLine("{0}BoxedValue {1};", Literal.GetIndentString(indent), v);
        }
        sb.Append(bodySb);
        --indent;
        sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
    }
    private static void CompileSyntaxComponent(Dsl.ISyntaxComponent comp, StringBuilder sb, int indent, bool isStatement, SortedSet<string> gvars, SortedSet<string> lvars)
    {
        var valData = comp as Dsl.ValueData;
        if (null != valData) {
            CompileValueData(valData, sb, indent, isStatement, gvars, lvars);
        }
        else {
            var funcData = comp as Dsl.FunctionData;
            if (null != funcData) {
                CompileFunctionData(funcData, sb, indent, isStatement, gvars, lvars);
            }
            else {
                var stmData = comp as Dsl.StatementData;
                if(null != stmData) {
                    CompileStatementData(stmData, sb, indent, isStatement, gvars, lvars);
                }
                else {
                    //null syntax
                }
            }
        }
    }
    private static void CompileValueData(Dsl.ValueData valData, StringBuilder sb, int indent, bool isStatement, SortedSet<string> gvars, SortedSet<string> lvars)
    {
        string id = valData.GetId();
        int idType = valData.GetIdType();
        if (isStatement) {
            sb.Append(Literal.GetIndentString(indent));
        }
        if (idType == Dsl.ValueData.STRING_TOKEN) {
            sb.Append('"');
            sb.Append(id);
            sb.Append('"');
        }
        else if (idType == Dsl.ValueData.NUM_TOKEN) {
            sb.Append(id);
        }
        else {
            if (id == "$$") {
                string vname = s_IteratorStack.Peek();
                sb.Append(vname);
            }
            else {
                string vname = id.Replace("@@", "g__").Replace("@", "g_").Replace("$", "_");
                if (id.StartsWith("@")) {
                    gvars.Add(vname);
                }
                else if (id.StartsWith("$")) {
                    lvars.Add(vname);
                }
                sb.Append(vname);
            }
        }
        if (isStatement) {
            sb.AppendLine(";");
        }
    }
    private static void CompileFunctionData(Dsl.FunctionData funcData, StringBuilder sb, int indent, bool isStatement, SortedSet<string> gvars, SortedSet<string> lvars)
    {
        string id = funcData.GetId();
        if (funcData.HaveStatement()) {
            if (id == "if") {
                sb.AppendFormat("{0}if (", Literal.GetIndentString(indent));
                CompileSyntaxComponent(funcData.LowerOrderFunction.GetParam(0), sb, indent, false, gvars, lvars);
                sb.AppendLine(") {");
                ++indent;
                CompileStatementBlock(funcData, sb, indent, true, gvars, lvars);
                --indent;
                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
            }
            else if (id == "while") {
                sb.AppendFormat("{0}while (", Literal.GetIndentString(indent));
                CompileSyntaxComponent(funcData.LowerOrderFunction.GetParam(0), sb, indent, false, gvars, lvars);
                sb.AppendLine(") {");
                ++indent;
                CompileStatementBlock(funcData, sb, indent, true, gvars, lvars);
                --indent;
                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
            }
            else if (id == "loop") {
                string iter = "iter_" + GenNextId();
                s_IteratorStack.Push(iter);
                sb.AppendFormat("{0}for (int {1} = 0; {1} < ", Literal.GetIndentString(indent), iter);
                CompileSyntaxComponent(funcData.LowerOrderFunction.GetParam(0), sb, indent, false, gvars, lvars);
                sb.AppendLine("; ++{0}) {{", iter);
                ++indent;
                CompileStatementBlock(funcData, sb, indent, true, gvars, lvars);
                --indent;
                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                s_IteratorStack.Pop();
            }
            else if (id == "looplist") {
                string iter = "iter_" + GenNextId();
                s_IteratorStack.Push(iter);
                sb.AppendFormat("{0}foreach (var {1} in ", Literal.GetIndentString(indent), iter);
                CompileSyntaxComponent(funcData.LowerOrderFunction.GetParam(0), sb, indent, false, gvars, lvars);
                sb.AppendLine(") {");
                ++indent;
                CompileStatementBlock(funcData, sb, indent, true, gvars, lvars);
                --indent;
                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                s_IteratorStack.Pop();
            }
            else if (id == "foreach") {
                string iter = "iter_" + GenNextId();
                s_IteratorStack.Push(iter);
                sb.AppendFormat("{0}foreach (var {1} in [", Literal.GetIndentString(indent), iter);
                var cd = funcData.LowerOrderFunction;
                string prestr = string.Empty;
                foreach (var p in cd.Params) {
                    sb.Append(prestr);
                    prestr = ", ";
                    CompileSyntaxComponent(p, sb, indent, false, gvars, lvars);
                }
                sb.AppendLine("]) {");
                ++indent;
                CompileStatementBlock(funcData, sb, indent, true, gvars, lvars);
                --indent;
                sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                s_IteratorStack.Pop();
            }
            else {
                LogSystem.Error("Unsupported block statement, id:{0} line:{1}", id, funcData.GetLine());
            }
        }
        else {
            string funcName;
            if (PerfGrade.Instance.ExistsApi(id)) {
                funcName = id + "_impl";
            }
            else {
                funcName = "_dsl_" + id;
            }
            if (isStatement) {
                sb.Append(Literal.GetIndentString(indent));
            }
            sb.Append(funcName);
            sb.Append('(');
            string prestr = string.Empty;
            foreach (var p in funcData.Params) {
                sb.Append(prestr);
                prestr = ", ";
                CompileSyntaxComponent(p, sb, indent, false, gvars, lvars);
            }
            sb.Append(')');
            if (isStatement) {
                sb.AppendLine(";");
            }
        }
    }
    private static void CompileStatementData(Dsl.StatementData stmData, StringBuilder sb, int indent, bool isStatement, SortedSet<string> gvars, SortedSet<string> lvars)
    {
        string id = stmData.GetId();
        if (id == "if") {
            CompileFunctionData(stmData.First.AsFunction, sb, indent, isStatement, gvars, lvars);
            for (int i = 1; i < stmData.GetFunctionNum(); ++i) {
                var fd = stmData.GetFunction(i).AsFunction;
                Debug.Assert(null != fd);
                if (fd.GetId() == "else") {
                    sb.AppendLine("{0}else {{", Literal.GetIndentString(indent));
                    ++indent;
                    CompileStatementBlock(fd, sb, indent, true, gvars, lvars);
                    --indent;
                    sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                }
                else {
                    sb.AppendFormat("{0}else if (", Literal.GetIndentString(indent));
                    CompileSyntaxComponent(fd.LowerOrderFunction.GetParam(0), sb, indent, false, gvars, lvars);
                    sb.AppendLine(") {");
                    ++indent;
                    CompileStatementBlock(fd, sb, indent, true, gvars, lvars);
                    --indent;
                    sb.AppendLine("{0}}}", Literal.GetIndentString(indent));
                }
            }
        }
        else {
            LogSystem.Error("Unsupported statement, id:{0} line:{1}", id, stmData.GetLine());
        }
    }
    private static void CompileStatementBlock(Dsl.FunctionData funcData, StringBuilder sb, int indent, bool isStatement, SortedSet<string> gvars, SortedSet<string> lvars)
    {
        foreach(var p in funcData.Params) {
            CompileSyntaxComponent(p, sb, indent, true, gvars, lvars);
        }
    }

    private static int GenNextId()
    {
        return s_NextId++;
    }

    private static Stack<string> s_IteratorStack = new Stack<string>();
    private static int s_NextId = 0;

    private static List<int> s_PerfIds = new List<int>();
    private static List<string> s_Inits = new List<string>();
    private static SortedDictionary<int, List<Tuple<string, string>>> s_Grades = new SortedDictionary<int, List<Tuple<string, string>>>();
    private static SortedDictionary<int, List<Tuple<string, string>>> s_DefGrades = new SortedDictionary<int, List<Tuple<string, string>>>();
    private static SortedDictionary<int, List<string>> s_Settings = new SortedDictionary<int, List<string>>();

    private static DslCalculator s_Calculator = null;
    private static StoryScript.SimpleObjectPool<BoxedValueList> s_BoxedValueListPool = new StoryScript.SimpleObjectPool<BoxedValueList>(256);
    private static bool s_Inited = false;
}
