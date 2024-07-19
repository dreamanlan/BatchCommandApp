using StoryScript;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[UnityEngine.Scripting.Preserve]
public sealed partial class PerfGrade
{
    public delegate GradeEnum GetGradeDelegation();
    public delegate void DoSettingDelegation(GradeEnum grade);
    public delegate BoxedValue PerfApiDelegation(BoxedValueList args);

    public const int c_max_perf_grade_cfgs = 32;
    public enum GradeEnum
    {
        Unknown = -1,
        Ultra = 0,
        High,
        Middle,
        Low,
        Lowest,
        Num
    }

    public bool GradeSetState { get; private set; } = false;

    public void SetCallFromGrade(bool fromGrade)
    {
        GradeSetState = true;
        m_CallFromGrade = fromGrade;
    }
    private bool CheckContinue()
    {
        if (!m_CallFromGrade)
            GradeSetState = true;
        return GradeSetState;
    }

    public void ClearAll()
    {
        m_CompiledPerfGrades.Clear();
        m_ScriptablePerfGrades.Clear();

        m_Device2Grades.Clear();
        m_GradeCodes.Clear();
        m_DefaultGradeCodes.Clear();
        m_SettingCodes.Clear();
    }
    public void LogCompiledPerfGrades()
    {
        var sb = new StringBuilder();
        sb.AppendLine("compiled perf grades:");
        foreach(var id in m_CompiledPerfGrades) {
            sb.AppendLine("\t{0}", id);
        }
        Debug.LogFormat("{0}", sb.ToString());
    }
    public bool ExistsScriptablePerfGrade(int id)
    {
        return m_ScriptablePerfGrades.Contains(id);
    }
    public void AddOverridedScript(int id)
    {
        m_ScriptablePerfGrades.Add(id);
    }
    public void Init()
    {
        RegisterPerfGrade_0();
        RegisterPerfGrade_1();
        RegisterPerfGrade_2();
        RegisterPerfGrade_3();
        RegisterPerfGrade_4();
        RegisterPerfGrade_5();
        RegisterPerfGrade_6();
        RegisterPerfGrade_7();
        RegisterPerfGrade_8();
        RegisterPerfGrade_9();
        RegisterPerfGrade_10();
        RegisterPerfGrade_11();
        RegisterPerfGrade_12();
        RegisterPerfGrade_13();
        RegisterPerfGrade_14();
        RegisterPerfGrade_15();
        RegisterPerfGrade_16();
        RegisterPerfGrade_17();
        RegisterPerfGrade_18();
        RegisterPerfGrade_19();
        RegisterPerfGrade_20();
        RegisterPerfGrade_21();
        RegisterPerfGrade_22();
        RegisterPerfGrade_23();
        RegisterPerfGrade_24();
        RegisterPerfGrade_25();
        RegisterPerfGrade_26();
        RegisterPerfGrade_27();
        RegisterPerfGrade_28();
        RegisterPerfGrade_29();
        RegisterPerfGrade_30();
        RegisterPerfGrade_31();
    }
    public GradeEnum GetGrade()
    {
        string model = SystemInfo.deviceModel;
        if (m_Device2Grades.TryGetValue(model, out GradeEnum g1)) {
            Debug.LogFormat("set grade:{0} from device:{1}", g1, model);
            return g1;
        }
        string gpu = SystemInfo.graphicsDeviceName;
        if(m_Device2Grades.TryGetValue(gpu, out GradeEnum g2)) {
            Debug.LogFormat("set grade:{0} from gpu:{1}", g2, gpu);
            return g2;
        }
        foreach(var c in m_GradeCodes) {
            GradeEnum r = c();
            if (r != GradeEnum.Unknown) {
                return r;
            }
        }
        return GradeEnum.Unknown;
    }
    public GradeEnum GetDefaultGrade()
    {
        foreach (var c in m_DefaultGradeCodes) {
            GradeEnum r = c();
            if (r != GradeEnum.Unknown) {
                return r;
            }
        }
        return GradeEnum.Unknown;
    }
    public void DoSetting(GradeEnum grade)
    {
        foreach(var c in m_SettingCodes) {
            c(grade);
        }
    }

    public bool ExistsApi(string method)
    {
        var t = this.GetType();
        var mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (null != mi) {
            return true;
        }
        else {
            mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (null != mi) {
                return true;
            }
        }
        return false;
    }
    public PerfApiDelegation GetApi(string method)
    {
        var t = this.GetType();
        var mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (null != mi) {
            var delegation = System.Delegate.CreateDelegate(typeof(PerfApiDelegation), this, mi, false);
            if (null != delegation)
                return (PerfApiDelegation)delegation;
            else
                return (BoxedValueList args) => { object o = mi.Invoke(this, new object[] { args }); return BoxedValue.FromObject(o); };
        }
        else {
            mi = t.GetMethod(method, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (null != mi) {
                var delegation = System.Delegate.CreateDelegate(typeof(PerfApiDelegation), mi, false);
                if (null != delegation)
                    return (PerfApiDelegation)delegation;
                else
                    return (BoxedValueList args) => { object o = mi.Invoke(null, new object[] { args }); return BoxedValue.FromObject(o); };
            }
        }
        return null;
    }

    //perf grade apis
    private BoxedValue device_name(BoxedValueList list)
    {
        string model = device_name_impl();
        return BoxedValue.FromString(model);
    }
    private string device_name_impl()
    {
        return SystemInfo.deviceName;
    }

    private BoxedValue device_model(BoxedValueList list)
    {
        string model = device_model_impl();
        return BoxedValue.FromString(model);
    }
    private string device_model_impl()
    {
        return SystemInfo.deviceModel;
    }

    private BoxedValue gpu_model(BoxedValueList list)
    {
        string model = gpu_model_impl();
        return BoxedValue.FromString(model);
    }
    private string gpu_model_impl()
    {
        return SystemInfo.graphicsDeviceName;
    }

    private BoxedValue gpu_ver(BoxedValueList list)
    {
        string ver = gpu_ver_impl();
        return BoxedValue.FromString(ver);
    }
    private string gpu_ver_impl()
    {
        return SystemInfo.graphicsDeviceVersion;
    }

    private BoxedValue os_ver(BoxedValueList list)
    {
        string type = os_ver_impl();
        return BoxedValue.FromString(type);
    }
    private string os_ver_impl()
    {
        return SystemInfo.operatingSystem;
    }

    private BoxedValue cpu_type(BoxedValueList list)
    {
        string type = cpu_type_impl();
        return BoxedValue.FromString(type);
    }
    private string cpu_type_impl()
    {
        return SystemInfo.processorType;
    }

    private BoxedValue cpu_core_count(BoxedValueList list)
    {
        int ct = cpu_core_count_impl();
        return BoxedValue.From(ct);
    }
    private int cpu_core_count_impl()
    {
        return SystemInfo.processorCount;
    }

    private BoxedValue cpu_freq(BoxedValueList list)
    {
        int freq = cpu_freq_impl();
        return BoxedValue.From(freq);
    }
    private int cpu_freq_impl()
    {
        return SystemInfo.processorFrequency;
    }

    private BoxedValue system_memory(BoxedValueList list)
    {
        int val = system_memory_impl();
        return BoxedValue.From(val);
    }
    private int system_memory_impl()
    {
        return SystemInfo.systemMemorySize;
    }

    private BoxedValue gpu_memory(BoxedValueList list)
    {
        int val = gpu_memory_impl();
        return BoxedValue.From(val);
    }
    private int gpu_memory_impl()
    {
        return SystemInfo.graphicsMemorySize;
    }

    private BoxedValue screen_width(BoxedValueList list)
    {
        int val = screen_width_impl();
        return BoxedValue.From(val);
    }
    private int screen_width_impl()
    {
        return Screen.width;
    }

    private BoxedValue screen_height(BoxedValueList list)
    {
        int val = screen_height_impl();
        return BoxedValue.From(val);
    }
    private int screen_height_impl()
    {
        return Screen.height;
    }

    private BoxedValue screen_dpi(BoxedValueList list)
    {
        float val = screen_dpi_impl();
        return BoxedValue.From(val);
    }
    private float screen_dpi_impl()
    {
        return Screen.dpi;
    }

    private BoxedValue log_sysinfo(BoxedValueList list)
    {
        return BoxedValue.FromBool(log_sysinfo_impl());
    }
    private bool log_sysinfo_impl()
    {
        Debug.LogFormat("device:{0} | {1} gpu:{2} ver:{3} os:{4} sys memory:{5} gpu memory:{6} cpu:{7} core:{8} freq:{9}", SystemInfo.deviceName, SystemInfo.deviceModel, SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion,
            SystemInfo.operatingSystem, SystemInfo.systemMemorySize, SystemInfo.graphicsMemorySize,
            SystemInfo.processorType, SystemInfo.processorCount, SystemInfo.processorFrequency);
        return true;
    }

    private BoxedValue string_like(BoxedValueList list)
    {
        bool first = true;
        string str = string.Empty;
        var regs = new List<string>();
        foreach (var v in list) {
            if (first)
                str = v.GetString();
            else
                regs.Add(v.GetString());
            first = false;
        }
        return BoxedValue.FromBool(string_like_helper(str, regs));
    }
    private bool string_like_impl(string str, params string[] regexes)
    {
        return string_like_helper(str, regexes);
    }

    private BoxedValue string_in(BoxedValueList list)
    {
        bool first = true;
        string str = string.Empty;
        var regs = new List<string>();
        foreach (var v in list) {
            if (first)
                str = v.GetString();
            else
                regs.Add(v.GetString());
            first = false;
        }
        return BoxedValue.FromBool(string_in_helper(str, regs));
    }
    private bool string_in_impl(string str, params string[] regexes)
    {
        return string_in_helper(str, regexes);
    }

    private BoxedValue string_contains_all(BoxedValueList list)
    {
        bool first = true;
        string str = string.Empty;
        var regs = new List<string>();
        foreach (var v in list) {
            if (first)
                str = v.GetString();
            else
                regs.Add(v.GetString());
            first = false;
        }
        return BoxedValue.FromBool(string_contains_all_helper(str, regs));
    }
    private bool string_contains_all_impl(string str, params string[] regexes)
    {
        return string_contains_all_helper(str, regexes);
    }

    private BoxedValue string_contains_any(BoxedValueList list)
    {
        bool first = true;
        string str = string.Empty;
        var regs = new List<string>();
        foreach (var v in list) {
            if (first)
                str = v.GetString();
            else
                regs.Add(v.GetString());
            first = false;
        }
        return BoxedValue.FromBool(string_contains_any_helper(str, regs));
    }
    private bool string_contains_any_impl(string str, params string[] regexes)
    {
        return string_contains_any_helper(str, regexes);
    }

    private BoxedValue is_mobile(BoxedValueList list)
    {
        bool r = is_mobile_impl();
        return BoxedValue.From(r);
    }
    private bool is_mobile_impl()
    {
        GradeSetState = GradeSetState && is_mobile_helper();
        return GradeSetState;
    }

    private BoxedValue is_console(BoxedValueList list)
    {
        bool r = is_console_impl();
        return BoxedValue.From(r);
    }
    private bool is_console_impl()
    {
        GradeSetState = GradeSetState && is_console_helper();
        return GradeSetState;
    }

    private BoxedValue is_editor(BoxedValueList list)
    {
        bool r = is_editor_impl();
        return BoxedValue.From(r);
    }
    private bool is_editor_impl()
    {
        GradeSetState = GradeSetState && is_editor_helper();
        return GradeSetState;
    }

    private BoxedValue is_android(BoxedValueList list)
    {
        bool r = is_android_impl();
        return BoxedValue.From(r);
    }
    private bool is_android_impl()
    {
        GradeSetState = GradeSetState && is_android_helper();
        return GradeSetState;
    }

    private BoxedValue is_iphone(BoxedValueList list)
    {
        bool r = is_iphone_impl();
        return BoxedValue.From(r);
    }
    private bool is_iphone_impl()
    {
        GradeSetState = GradeSetState && is_iphone_helper();
        return GradeSetState;
    }

    private BoxedValue is_pc(BoxedValueList list)
    {
        bool r = is_pc_impl();
        return BoxedValue.From(r);
    }
    private bool is_pc_impl()
    {
        GradeSetState = GradeSetState && is_pc_helper();
        return GradeSetState;
    }

    private BoxedValue name_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(name_like_impl(regs));
    }
    private bool name_like_impl(params string[] regexes)
    {
        return name_like_impl(regexes);
    }
    private bool name_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceName;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue name_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(name_in_impl(regs));
    }
    private bool name_in_impl(params string[] regexes)
    {
        return name_in_impl(regexes);
    }
    private bool name_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceName;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue name_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(name_contains_all_impl(regs));
    }
    private bool name_contains_all_impl(params string[] regexes)
    {
        return name_contains_all_impl(regexes);
    }
    private bool name_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceName;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue name_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(name_contains_any_impl(regs));
    }
    private bool name_contains_any_impl(params string[] regexes)
    {
        return name_contains_any_impl(regexes);
    }
    private bool name_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceName;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue device_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach(var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(device_like_impl(regs));
    }
    private bool device_like_impl(params string[] regexes)
    {
        return device_like_impl(regexes);
    }
    private bool device_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceModel;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue device_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(device_in_impl(regs));
    }
    private bool device_in_impl(params string[] regexes)
    {
        return device_in_impl(regexes);
    }
    private bool device_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceModel;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue device_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(device_contains_all_impl(regs));
    }
    private bool device_contains_all_impl(params string[] regexes)
    {
        return device_contains_all_impl(regexes);
    }
    private bool device_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceModel;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue device_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(device_contains_any_impl(regs));
    }
    private bool device_contains_any_impl(params string[] regexes)
    {
        return device_contains_any_impl(regexes);
    }
    private bool device_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.deviceModel;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_like_impl(regs));
    }
    private bool gpu_like_impl(params string[] regexes)
    {
        return gpu_like_impl(regexes);
    }
    private bool gpu_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceName;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_in_impl(regs));
    }
    private bool gpu_in_impl(params string[] regexes)
    {
        return gpu_in_impl(regexes);
    }
    private bool gpu_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceName;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_contains_all_impl(regs));
    }
    private bool gpu_contains_all_impl(params string[] regexes)
    {
        return gpu_contains_all_impl(regexes);
    }
    private bool gpu_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceName;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_contains_any_impl(regs));
    }
    private bool gpu_contains_any_impl(params string[] regexes)
    {
        return gpu_contains_any_impl(regexes);
    }
    private bool gpu_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceName;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue cpu_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(cpu_like_impl(regs));
    }
    private bool cpu_like_impl(params string[] regexes)
    {
        return cpu_like_impl(regexes);
    }
    private bool cpu_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.processorType;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue cpu_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(cpu_in_impl(regs));
    }
    private bool cpu_in_impl(params string[] regexes)
    {
        return cpu_in_impl(regexes);
    }
    private bool cpu_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.processorType;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue cpu_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(cpu_contains_all_impl(regs));
    }
    private bool cpu_contains_all_impl(params string[] regexes)
    {
        return cpu_contains_all_impl(regexes);
    }
    private bool cpu_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.processorType;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue cpu_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(cpu_contains_any_impl(regs));
    }
    private bool cpu_contains_any_impl(params string[] regexes)
    {
        return cpu_contains_any_impl(regexes);
    }
    private bool cpu_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.processorType;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_ver_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_ver_like_impl(regs));
    }
    private bool gpu_ver_like_impl(params string[] regexes)
    {
        return gpu_ver_like_impl(regexes);
    }
    private bool gpu_ver_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceVersion;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_ver_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_ver_in_impl(regs));
    }
    private bool gpu_ver_in_impl(params string[] regexes)
    {
        return gpu_ver_in_impl(regexes);
    }
    private bool gpu_ver_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceVersion;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_ver_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_ver_contains_all_impl(regs));
    }
    private bool gpu_ver_contains_all_impl(params string[] regexes)
    {
        return gpu_ver_contains_all_impl(regexes);
    }
    private bool gpu_ver_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceVersion;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue gpu_ver_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(gpu_ver_contains_any_impl(regs));
    }
    private bool gpu_ver_contains_any_impl(params string[] regexes)
    {
        return gpu_ver_contains_any_impl(regexes);
    }
    private bool gpu_ver_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.graphicsDeviceVersion;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue os_ver_like(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(os_ver_like_impl(regs));
    }
    private bool os_ver_like_impl(params string[] regexes)
    {
        return os_ver_like_impl(regexes);
    }
    private bool os_ver_like_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.operatingSystem;
            bool res = string_like_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue os_ver_in(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(os_ver_in_impl(regs));
    }
    private bool os_ver_in_impl(params string[] regexes)
    {
        return os_ver_in_impl(regexes);
    }
    private bool os_ver_in_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.operatingSystem;
            bool res = string_in_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue os_ver_contains_all(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(os_ver_contains_all_impl(regs));
    }
    private bool os_ver_contains_all_impl(params string[] regexes)
    {
        return os_ver_contains_all_impl(regexes);
    }
    private bool os_ver_contains_all_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.operatingSystem;
            bool res = string_contains_all_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue os_ver_contains_any(BoxedValueList list)
    {
        var regs = new List<string>();
        foreach (var v in list) {
            regs.Add(v.GetString());
        }
        return BoxedValue.FromBool(os_ver_contains_any_impl(regs));
    }
    private bool os_ver_contains_any_impl(params string[] regexes)
    {
        return os_ver_contains_any_impl(regexes);
    }
    private bool os_ver_contains_any_impl(IList<string> regexes)
    {
        if (CheckContinue()) {
            var model = SystemInfo.operatingSystem;
            bool res = string_contains_any_helper(model, regexes);
            GradeSetState = GradeSetState && res;
        }
        return GradeSetState;
    }

    private BoxedValue memory_below(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = memory_below_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool memory_below_impl(int mem)
    {
        GradeSetState = GradeSetState && (SystemInfo.systemMemorySize <= mem);
        return GradeSetState;
    }

    private BoxedValue memory_above(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = memory_above_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool memory_above_impl(int mem)
    {
        GradeSetState = GradeSetState && (SystemInfo.systemMemorySize >= mem);
        return GradeSetState;
    }

    private BoxedValue gpu_memory_below(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = gpu_memory_below_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool gpu_memory_below_impl(int mem)
    {
        GradeSetState = GradeSetState && (SystemInfo.graphicsMemorySize <= mem);
        return GradeSetState;
    }

    private BoxedValue gpu_memory_above(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = gpu_memory_above_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool gpu_memory_above_impl(int mem)
    {
        GradeSetState = GradeSetState && (SystemInfo.graphicsMemorySize >= mem);
        return GradeSetState;
    }

    private BoxedValue screen_below(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 2)
        {
            r = screen_below_impl(list[0].GetInt(), list[1].GetInt(), list[2].GetFloat());
        }
        return BoxedValue.From(r);
    }
    private bool screen_below_impl(int width, int height, float dpi)
    {
        GradeSetState = GradeSetState && (Screen.width <= width && Screen.height <= height && Screen.dpi <= dpi);
        return GradeSetState;
    }

    private BoxedValue screen_above(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 2)
        {
            r = screen_above_impl(list[0].GetInt(), list[1].GetInt(), list[2].GetFloat());
        }
        return BoxedValue.From(r);
    }
    private bool screen_above_impl(int width, int height, float dpi)
    {
        GradeSetState = GradeSetState && (Screen.width >= width && Screen.height >= height && Screen.dpi >= dpi);
        return GradeSetState;
    }

    private BoxedValue add_grade(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 1) {
            r = add_grade_impl(list[0].GetString(), list[1].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool add_grade_impl(string device, int grade)
    {
        bool r = false;
        if (grade >= 0 && grade < 5) {
            var g = (GradeEnum)grade;
            r = m_Device2Grades.TryAdd(device, g);
        }
        return r;
    }

    private BoxedValue set_quality(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = set_quality_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool set_quality_impl(int quality)
    {
        QualitySettings.SetQualityLevel(quality, true);
        return true;
    }

    private BoxedValue set_shader_lod(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = set_shader_lod_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool set_shader_lod_impl(int maxLod)
    {
        Shader.globalMaximumLOD = maxLod;
        return true;
    }

    private BoxedValue set_lod_level(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 1) {
            r = set_lod_level_impl(list[0].GetFloat(), list[1].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool set_lod_level_impl(float lodBias, int maxLodLevel)
    {
        QualitySettings.SetLODSettings(lodBias, maxLodLevel);
        return true;
    }

    private BoxedValue set_mipmap(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 0) {
            r = set_mipmap_impl(list[0].GetInt());
        }
        return BoxedValue.From(r);
    }
    private bool set_mipmap_impl(int mipmap)
    {
        QualitySettings.globalTextureMipmapLimit = mipmap;
        return true;
    }

    private BoxedValue set_resolution(BoxedValueList list)
    {
        bool r = false;
        if (list.Count > 4) {
            r = set_resolution_impl(list[0].GetInt(), list[1].GetInt(), list[2].GetBool(), list[3].GetUInt(), list[4].GetUInt());
        }
        return BoxedValue.From(r);
    }
    private bool set_resolution_impl(int width, int height, bool fullScreen, uint num, uint denom)
    {
        Screen.SetResolution(width, height, fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, new RefreshRate { numerator = num, denominator = denom });
        return true;
    }

    //dsl api alternative
    private void _dsl_debuglog(string fmt, params object[] args)
    {
        Debug.LogFormat(fmt, args);
    }

    //private methods
    private bool is_android_helper()
    {
        return Application.platform == RuntimePlatform.Android;
    }
    private bool is_iphone_helper()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }
    private bool is_editor_helper()
    {
        return Application.isEditor;
    }
    private bool is_mobile_helper()
    {
        return Application.isMobilePlatform;
    }
    private bool is_console_helper()
    {
        return Application.isConsolePlatform;
    }
    private bool is_pc_helper()
    {
        return !Application.isMobilePlatform && !Application.isConsolePlatform;
    }
    private bool string_like_helper(string str, IList<string> regexes)
    {
        foreach (var regex in regexes) {
            var r = GetCompiledRegex(regex);
            Debug.Assert(null != r);
            if (r.IsMatch(str))
                return true;
        }
        return false;
    }
    private bool string_in_helper(string str, IList<string> strs)
    {
        foreach (string s in strs) {
            if (string.Compare(str, s, true) == 0)
                return true;
        }
        return false;
    }
    private bool string_contains_all_helper(string str, IList<string> strs)
    {
        foreach (string s in strs) {
            if (!str.Contains(s, System.StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }
    private bool string_contains_any_helper(string str, IList<string> strs)
    {
        foreach(string s in strs) {
            if (str.Contains(s, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    private Regex GetCompiledRegex(string regex)
    {
        if(!m_Regexes.TryGetValue(regex, out var cr)) {
            cr = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            m_Regexes.Add(regex, cr);
        }
        return cr;
    }

    private bool m_CallFromGrade = false;
    private Dictionary<string, GradeEnum> m_Device2Grades = new Dictionary<string, GradeEnum> {
        {string.Empty, GradeEnum.Unknown},
    };
    private List<GetGradeDelegation> m_GradeCodes = new List<GetGradeDelegation>();
    private List<GetGradeDelegation> m_DefaultGradeCodes = new List<GetGradeDelegation>();
    private List<DoSettingDelegation> m_SettingCodes = new List<DoSettingDelegation>();
    private SortedSet<int> m_CompiledPerfGrades = new SortedSet<int>();
    private HashSet<int> m_ScriptablePerfGrades = new HashSet<int>();

    private Dictionary<string, Regex> m_Regexes = new Dictionary<string, Regex>();

    partial void RegisterPerfGrade_0();
    partial void RegisterPerfGrade_1();
    partial void RegisterPerfGrade_2();
    partial void RegisterPerfGrade_3();
    partial void RegisterPerfGrade_4();
    partial void RegisterPerfGrade_5();
    partial void RegisterPerfGrade_6();
    partial void RegisterPerfGrade_7();
    partial void RegisterPerfGrade_8();
    partial void RegisterPerfGrade_9();
    partial void RegisterPerfGrade_10();
    partial void RegisterPerfGrade_11();
    partial void RegisterPerfGrade_12();
    partial void RegisterPerfGrade_13();
    partial void RegisterPerfGrade_14();
    partial void RegisterPerfGrade_15();
    partial void RegisterPerfGrade_16();
    partial void RegisterPerfGrade_17();
    partial void RegisterPerfGrade_18();
    partial void RegisterPerfGrade_19();
    partial void RegisterPerfGrade_20();
    partial void RegisterPerfGrade_21();
    partial void RegisterPerfGrade_22();
    partial void RegisterPerfGrade_23();
    partial void RegisterPerfGrade_24();
    partial void RegisterPerfGrade_25();
    partial void RegisterPerfGrade_26();
    partial void RegisterPerfGrade_27();
    partial void RegisterPerfGrade_28();
    partial void RegisterPerfGrade_29();
    partial void RegisterPerfGrade_30();
    partial void RegisterPerfGrade_31();

    public static PerfGrade Instance
    {
        get {
            if (null == s_Instance) {
                s_Instance = new PerfGrade();
            }
            return s_Instance;
        }
    }
    private static PerfGrade s_Instance = null;
}
