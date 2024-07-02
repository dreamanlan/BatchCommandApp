using System;
using System.Collections.Generic;
using StoryScript;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Assertions.Must;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using UnityEngine.Profiling;
using System.Runtime.CompilerServices;

namespace GmCommands
{
    internal static class FakeProfiler
    {
        internal static long GetAllocatorCount()
        {
            return 0;
        }
        internal static long GetTotalAllocationCount()
        {
            return 0;
        }
        internal static long GetMallocLLAllocBytes()
        {
            return 0;
        }
        internal static long GetMallocOverrideBytes()
        {
            return 0;
        }
        internal static long GetTotalProfilerMemoryLong()
        {
            return 0;
        }
        internal static float ProfilerGetMainThreadFrameTime()
        {
            return 0.0f;
        }
        internal static float ProfilerGetRenderThreadFrameTime()
        {
            return 0.0f;
        }
        internal static int ProfilerGetVertsCount()
        {
            return 0;
        }
        internal static int ProfilerGetTriangleCount()
        {
            return 0;
        }
        internal static int ProfilerGetDrawCallCount()
        {
            return 0;
        }
        internal static int ProfilerGetSetPassCalls()
        {
            return 0;
        }
        internal static long ProfilerGetUsedTextureBytes()
        {
            return 0;
        }
        internal static int ProfilerGetUsedTextureCount()
        {
            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class SetDebugCommand : SimpleStoryCommandBase<SetDebugCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int val = _params.Param1Value;
            StoryConfigManager.Instance.IsDebug = val != 0;
            return false;
        }
    }
    internal class EditorBreakCommand : SimpleStoryCommandBase<EditorBreakCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            UnityEngine.Debug.Break();
            return false;
        }
    }
    internal class DebugBreakCommand : SimpleStoryCommandBase<DebugBreakCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            UnityEngine.Debug.DebugBreak();
            return false;
        }
    }
    internal class SupportsTexCommand : SimpleStoryCommandBase<SupportsTexCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(TextureFormat));
            foreach (var e in vs) {
                var tf = (TextureFormat)e;
                LogSystem.Warn("[check tex {0}]", tf);
                try {
                    if (!SystemInfo.SupportsTextureFormat(tf)) {
                        LogSystem.Error("can't support tex {0}", tf);
                    }
                }
                catch {
                    LogSystem.Error("invalid tex format {0}", tf);
                }
            }
            return false;
        }
    }
    internal class SupportsRTCommand : SimpleStoryCommandBase<SupportsRTCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(RenderTextureFormat));
            foreach (var e in vs) {
                var rtf = (RenderTextureFormat)e;
                try {
                    var gfSrgb = GraphicsFormatUtility.GetGraphicsFormat(rtf, true);
                    var gfLinear = GraphicsFormatUtility.GetGraphicsFormat(rtf, false);
                    LogSystem.Warn("[check RT {0} <=> srgb:{1} linear:{2}]", rtf, gfSrgb, gfLinear);
                    if (!SystemInfo.SupportsRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                    if(!SystemInfo.SupportsBlendingOnRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support blending on RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                    if (!SystemInfo.SupportsRandomWriteOnRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support random write on RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                }
                catch {
                    LogSystem.Error("invalid rt format {0}", rtf);
                }
            }
            return false;
        }
    }
    internal class SupportsVertexAttributeFormatCommand : SimpleStoryCommandBase<SupportsVertexAttributeFormatCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(VertexAttributeFormat));
            foreach (var e in vs) {
                var vaf = (VertexAttributeFormat)e;
                try {
                    for (int dim = 1; dim <= 4; ++dim) {
                        LogSystem.Warn("[check vertex attribute format {0} dim {1}]", vaf, dim);
                        if (!SystemInfo.SupportsVertexAttributeFormat(vaf, dim)) {
                            LogSystem.Error("can't support vertex attribute format {0} dim {1}", vaf, dim);
                        }
                    }
                }
                catch {
                    LogSystem.Error("invalid vertex attribute format {0}", vaf);
                }
            }
            return false;
        }
    }
    internal class DeviceSupportsCommand : SimpleStoryCommandBase<DeviceSupportsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var log1 = new StringBuilder();
            var log2 = new StringBuilder();
            int ct = 0;
            var t = typeof(SystemInfo);
            var pis = t.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var pi in pis) {
                var v = pi.GetValue(null);
                if (v is bool) {
                    if (!(bool)v) {
                        log1.AppendFormat("{0} = false", pi.Name);
                        log1.AppendLine();

                        log2.AppendFormat("{0} = false", pi.Name);
                        log2.AppendLine();
                        ++ct;
                    }
                }
                else {
                    log2.AppendFormat("{0} = {1}", pi.Name, v);
                    log2.AppendLine();
                    ++ct;
                }
                if (ct % 10 == 0) {
                    Debug.Log(log2.ToString());
                    log2.Length = 0;
                }
            }
            DebugConsole.Log(log1.ToString());
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class AllocMemoryCommand : SimpleStoryCommandBase<AllocMemoryCommand, StoryValueParam<string, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, int> _params, long delta)
        {
            string key = _params.Param1Value;
            int size = _params.Param2Value;
            BoxedValue m = BoxedValue.FromObject(new byte[size]);
            if (instance.GlobalVariables.ContainsKey(key)) {
                instance.GlobalVariables[key] = m;
            }
            else {
                instance.GlobalVariables.Add(key, m);
            }
            return false;
        }
    }
    internal class FreeMemoryCommand : SimpleStoryCommandBase<FreeMemoryCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string key = _params.Param1Value;
            if (instance.GlobalVariables.ContainsKey(key)) {
                instance.GlobalVariables.Remove(key);
                GC.Collect();
            }
            else {
                GC.Collect();
            }
            return false;
        }
    }
    internal class ConsumeCpuCommand : SimpleStoryCommandBase<ConsumeCpuCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int time = _params.Param1Value;
            long startTime = TimeUtility.GetElapsedTimeUs();
            while (startTime + time > TimeUtility.GetElapsedTimeUs()) {
            }
            return false;
        }
    }
    internal class GcCommand : SimpleStoryCommandBase<GcCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            GC.Collect();
            return false;
        }
    }
    internal class LogProfilerCommand : SimpleStoryCommandBase<LogProfilerCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            LogSystem.Warn("[memory] used heap:{0:f3} profiler:{1:f3} gfx:{2:f3} alloc:{3:f3} temp:{4:f3} unused reserved:{5:f3} reserved:{6:f3} alloc count:{7} total alloc count:{8} malloc LL:{9:f3} malloc override:{10:f3}", Profiler.usedHeapSizeLong / 1024.0f / 1024.0f, FakeProfiler.GetTotalProfilerMemoryLong() / 1024.0f / 1024.0f,
                Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024.0f / 1024.0f, Profiler.GetTotalAllocatedMemoryLong() / 1024.0f / 1024.0f, Profiler.GetTempAllocatorSize() / 1024.0f / 1024.0f, Profiler.GetTotalUnusedReservedMemoryLong() / 1024.0f / 1024.0f,
                Profiler.GetTotalReservedMemoryLong() / 1024.0f / 1024.0f,
                FakeProfiler.GetAllocatorCount(), FakeProfiler.GetTotalAllocationCount(),
                FakeProfiler.GetMallocLLAllocBytes() / 1024.0f / 1024.0f, FakeProfiler.GetMallocOverrideBytes() / 1024.0f / 1024.0f);
            LogSystem.Warn("[mono] used:{0} heap:{1}", Profiler.GetMonoUsedSizeLong() / 1024.0f / 1024.0f, Profiler.GetMonoHeapSizeLong() / 1024.0f / 1024.0f);
            LogSystem.Warn("[time] main:{0} rendering:{1}", FakeProfiler.ProfilerGetMainThreadFrameTime(), FakeProfiler.ProfilerGetRenderThreadFrameTime());
            LogSystem.Warn("[rendering] vertex:{0} triangle:{1} dc:{2} set pass:{3} tex:{4:f3} tex count:{5}", FakeProfiler.ProfilerGetVertsCount(), FakeProfiler.ProfilerGetTriangleCount(),
                FakeProfiler.ProfilerGetDrawCallCount(), FakeProfiler.ProfilerGetSetPassCalls(),
                FakeProfiler.ProfilerGetUsedTextureBytes() / 1024.0f / 1024.0f, FakeProfiler.ProfilerGetUsedTextureCount());
            return false;
        }
    }
    internal class CmdCommand : SimpleStoryCommandBase<CmdCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string cmd = _params.Param1Value;
            DebugConsole.Execute(cmd);
            return false;
        }
    }
    internal class PlayerPrefIntCommand : SimpleStoryCommandBase<PlayerPrefIntCommand, StoryValueParam<string, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, int> _params, long delta)
        {
            string key = _params.Param1Value;
            int val = _params.Param2Value;
            PlayerPrefs.SetInt(key, val);
            PlayerPrefs.Save();
            return false;
        }
    }
    internal class PlayerPrefFloatCommand : SimpleStoryCommandBase<PlayerPrefFloatCommand, StoryValueParam<string, float>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, float> _params, long delta)
        {
            string key = _params.Param1Value;
            float val = _params.Param2Value;
            PlayerPrefs.SetFloat(key, val);
            PlayerPrefs.Save();
            return false;
        }
    }
    internal class PlayerPrefStringCommand : SimpleStoryCommandBase<PlayerPrefStringCommand, StoryValueParam<string, string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, string> _params, long delta)
        {
            string key = _params.Param1Value;
            string val = _params.Param2Value;
            PlayerPrefs.SetString(key, val);
            PlayerPrefs.Save();
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------
    internal class GetMonoMemoryFunction : SimpleStoryFunctionBase<GetMonoMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = GC.GetTotalMemory(false) / 1024.0f / 1024.0f;
        }
    }
    internal class GetNativeMemoryFunction : SimpleStoryFunctionBase<GetNativeMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalAllocatedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal class GetGfxMemoryFunction : SimpleStoryFunctionBase<GetGfxMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024.0f / 1024.0f;
        }
    }
    internal class GetUnusedMemoryFunction : SimpleStoryFunctionBase<GetUnusedMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalUnusedReservedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal class GetTotalMemoryFunction : SimpleStoryFunctionBase<GetTotalMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalReservedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal class DeviceInfoFunction : SimpleStoryFunctionBase<DeviceInfoFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = SystemInfo.deviceName + " | " + SystemInfo.deviceModel + " | " + SystemInfo.graphicsDeviceName;
        }
    }
    internal class PlayerPrefIntFunction : SimpleStoryFunctionBase<PlayerPrefIntFunction, StoryValueParam<string, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, int> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            int def = _params.Param2Value;
            int val = PlayerPrefs.GetInt(key, def);

            result.Value = val;
        }
    }
    internal class PlayerPrefFloatFunction : SimpleStoryFunctionBase<PlayerPrefFloatFunction, StoryValueParam<string, float>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, float> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            float def = _params.Param2Value;
            float val = PlayerPrefs.GetFloat(key, def);

            result.Value = val;
        }
    }
    internal class PlayerPrefStringFunction : SimpleStoryFunctionBase<PlayerPrefStringFunction, StoryValueParam<string, string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, string> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            string def = _params.Param2Value;
            string val = PlayerPrefs.GetString(key, def);

            result.Value = val;
        }
    }
    internal class IsFormatSupportedFunction : SimpleStoryFunctionBase<IsFormatSupportedFunction, StoryValueParam<string, string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, string> _params, StoryValueResult result)
        {
            int r = -1;
            string ename = _params.Param1Value;
            string uname = _params.Param2Value;
            GraphicsFormat gf;
            FormatUsage fu;
            if (Enum.TryParse(ename, out gf) && Enum.TryParse(uname, out fu)) {
                try {
                    r = SystemInfo.IsFormatSupported(gf, fu) ? 1 : 0;
                }
                catch {
                    r = -1;
                }
            }
            result.Value = r;
        }
    }
    internal class GetCompatibleFormatFunction : SimpleStoryFunctionBase<GetCompatibleFormatFunction, StoryValueParam<string, string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, string> _params, StoryValueResult result)
        {
            string r = string.Empty;
            string ename = _params.Param1Value;
            string uname = _params.Param2Value;
            GraphicsFormat gf;
            FormatUsage fu;
            if (Enum.TryParse(ename, out gf) && Enum.TryParse(uname, out fu)) {
                try {
                    var f = SystemInfo.GetCompatibleFormat(gf, fu);
                    r = f.ToString();
                }
                catch {
                    r = string.Empty;
                }
            }
            result.Value = r;
        }
    }
    internal class GetGraphicsFormatFunction : SimpleStoryFunctionBase<GetGraphicsFormatFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
            string r = string.Empty;
            string ename = _params.Param1Value;
            DefaultFormat df;
            if (Enum.TryParse(ename, out df)) {
                try {
                    var f = SystemInfo.GetGraphicsFormat(df);
                    r = f.ToString();
                }
                catch {
                    r = string.Empty;
                }
            }
            result.Value = r;
        }
    }
    internal class GetMSAASampleCountFunction : SimpleStoryFunctionBase<GetMSAASampleCountFunction, StoryValueParam<int, int, string, int, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<int, int, string, int, int> _params, StoryValueResult result)
        {
            int r = -1;
            int w = _params.Param1Value;
            int h = _params.Param2Value;
            string ename = _params.Param3Value;
            int d = _params.Param4Value;
            int mips = _params.Param5Value;
            GraphicsFormat gfc;
            if (Enum.TryParse(ename, out gfc)) {
                try {
                    r = SystemInfo.GetRenderTextureSupportedMSAASampleCount(new RenderTextureDescriptor(w, h, gfc, d, mips));
                }
                catch {
                    r = -1;
                }
            }
            result.Value = r;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class ListComponentsCommand : SimpleStoryCommandBase<ListComponentsCommand, StoryValueParam<string, System.Collections.IList, object, int, bool>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, System.Collections.IList, object, int, bool> _params, long delta)
        {
            var list = new List<Component>();
            var root = _params.Param1Value;
            var vals = _params.Param2Value;
            var typeObj = _params.Param3Value;
            int up_level = _params.Param4Value;
            bool include_inactive = _params.Param5Value;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr)
            {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type)
            {
                if (string.IsNullOrEmpty(root))
                {
                    var objs = GameObject.FindObjectsOfType(type, include_inactive);
                    foreach (var obj in objs)
                    {
                        var comp = obj as Component;
                        if (null != comp)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                LogSystem.Warn("{0}", StoryScriptUtility.GetScenePath(comp.transform, up_level));
                            }
                        }
                    }
                }
                else
                {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj)
                    {
                        var comps = rootObj.GetComponentsInChildren(type, include_inactive);
                        foreach (var comp in comps)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                LogSystem.Warn("{0}", StoryScriptUtility.GetScenePath(comp.transform, up_level));
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
    internal class LogCompiledPerfsCommand : SimpleStoryCommandBase<LogCompiledPerfsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            PerfGradeGm.LogCompiledPerfGrades();
            return false;
        }
    }
    internal class ReloadPerfsCommand : SimpleStoryCommandBase<ReloadPerfsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            Main.TestPerfGrade();
            return false;
        }
    }
    internal class RunPerfCommand : SimpleStoryCommandBase<RunPerfCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            var perfFile = _params.Param1Value;
            if(!Path.IsPathRooted(perfFile)) {
                perfFile = PerfGradeGm.ScriptPath + perfFile;
            }
            PerfGradeGm.ClearPerfGradeScripts();
            PerfGradeGm.LoadPerfGradeScript(perfFile);
            PerfGradeGm.RunPerfGrade();
            return false;
        }
    }
    internal class CompilePerfCommand : SimpleStoryCommandBase<CompilePerfCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            var perfFile = _params.Param1Value;
            if (!Path.IsPathRooted(perfFile)) {
                perfFile = PerfGradeGm.ScriptPath + perfFile;
            }
            PerfGradeGm.CompilePerfGradeScript(perfFile);
            return false;
        }
    }
    internal class LoadUiCommand : SimpleStoryCommandBase<LoadUiCommand, StoryValueParam<Dsl.ValueData>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<Dsl.ValueData> _params, long delta)
        {
            var uiRes = _params.Param1Value;

            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.LoadUi(uiRes.GetId());
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return false;
        }
    }
    internal class ShowUiCommand : SimpleStoryCommandBase<ShowUiCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.ShowUi();
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return false;
        }
    }
    internal class HideUiCommand : SimpleStoryCommandBase<HideUiCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var fv = GmRootScript.GameObj;
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.HideUi();
                }
            }
            else {
                LogSystem.Error("can't find GmScript");
            }
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal class FindComponentFunction : SimpleStoryFunctionBase<FindComponentFunction, StoryValueParam<string, System.Collections.IList, object>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, System.Collections.IList, object> _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            var root = _params.Param1Value;
            var vals = _params.Param2Value;
            var typeObj = _params.Param3Value;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr)
            {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type)
            {
                if (string.IsNullOrEmpty(root))
                {
                    var objs = GameObject.FindObjectsOfType(type, false);
                    foreach (var obj in objs)
                    {
                        var comp = obj as Component;
                        if (null != comp)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                result.Value = comp;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj)
                    {
                        var comps = rootObj.GetComponentsInChildren(type, false);
                        foreach (var comp in comps)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                result.Value = comp;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    internal class SearchComponentsFunction : SimpleStoryFunctionBase<SearchComponentsFunction, StoryValueParam<string, System.Collections.IList, object>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, System.Collections.IList, object> _params, StoryValueResult result)
        {
            var list = new List<Component>();
            var root = _params.Param1Value;
            var vals = _params.Param2Value;
            var typeObj = _params.Param3Value;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr)
            {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type)
            {
                if (string.IsNullOrEmpty(root))
                {
                    var objs = GameObject.FindObjectsOfType(type, true);
                    foreach (var obj in objs)
                    {
                        var comp = obj as Component;
                        if (null != comp)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                list.Add(comp);
                            }
                        }
                    }
                }
                else
                {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj)
                    {
                        var comps = rootObj.GetComponentsInChildren(type, true);
                        foreach (var comp in comps)
                        {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                            {
                                list.Add(comp);
                            }
                        }
                    }
                }
            }
            result.Value = list;
        }
    }

}
