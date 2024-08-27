using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Assertions.Must;
using StoryScript;
using UnityEngine.Profiling;
using UnityEngine.UI;

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
    internal sealed class SetDebugCommand : SimpleStoryCommandBase<SetDebugCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int val = _params.Param1Value;
            StoryConfigManager.Instance.IsDebug = val != 0;
            return false;
        }
    }
    internal sealed class StartActivityCommand : SimpleStoryCommandBase<StartActivityCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var args = _params.Values;
            if (Application.platform == RuntimePlatform.Android) {
                if (args.Count >= 1 && args[0].IsString) {
                    string packageName = args[0].GetString();
                    string className = string.Empty;
                    int flags = 0;
                    IList list = null;
                    IDictionary dict = null;
                    bool noError = true;
                    for(int i = 1; i < args.Count; ++i) {
                        var arg = args[i];
                        if(arg.IsString && i == 1) {
                            className = arg.GetString();
                        }
                        else if(arg.IsInteger && i <= 2) {
                            flags = arg.GetInt();
                        }
                        else if (arg.IsObject && i == args.Count - 1) {
                            list = arg.As<IList>();
                            dict = arg.As<IDictionary>();
                        }
                        else {
                            //error arg
                            LogSystem.Error("startactivity, illegal argument {0}:{1}", i, arg);
                            noError = false;
                        }
                    }
                    if (noError) {
                        StartActivity(packageName, className, flags, list, dict);
                    }
                }
                else {
                    LogSystem.Error("expect startactivity(package_name, ...)");
                }
            }
            return false;
        }
        private static void StartActivity(string packageName, string className, int flags, IList list, IDictionary dict)
        {
            if (string.IsNullOrEmpty(packageName)) {
                packageName = Application.identifier;
            }
            if (string.IsNullOrEmpty(className)) {
                //className = "com.unity3d.player.UnityPlayerActivity";
                className = packageName + ".MainActivity";
            }

            using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var ca = up.GetStatic<AndroidJavaObject>("currentActivity")) {
                    using (var packageManager = ca.Call<AndroidJavaObject>("getPackageManager")) {
                        using (var intent = new AndroidJavaObject("android.content.Intent")) {
                            intent.Call<AndroidJavaObject>("setClassName", packageName, className);
                            if (flags != 0) {
                                intent.Call<AndroidJavaObject>("addFlags", flags);
                            }
                            if (null != list) {
                                for (int ix = 0; ix < list.Count - 1; ix += 2) {
                                    var key = list[ix] as string;
                                    var val = null == list[ix + 1] ? BoxedValue.NullObject : BoxedValue.FromObject(list[ix + 1]);
                                    if (!string.IsNullOrEmpty(key)) {
                                         GmRootScript.PutIntentExtra(intent, key, val);
                                    }
                                }
                            }
                            else if (null != dict) {
                                foreach(var elem in dict) {
                                    var pair = (DictionaryEntry)elem;
                                    var key = pair.Key as string;
                                    var val = null == pair.Value ? BoxedValue.NullObject : BoxedValue.FromObject(pair.Value);
                                    if (!string.IsNullOrEmpty(key)) {
                                        GmRootScript.PutIntentExtra(intent, key, val);
                                    }
                                }
                            }
                            ca.Call("startActivity", intent);

                            //intent.Call<AndroidJavaObject>("addFlags", 0x00008000); // Intent.FLAG_ACTIVITY_CLEAR_TASK
                            //intent.Call<AndroidJavaObject>("addFlags", 0x04000000); // Intent.FLAG_ACTIVITY_CLEAR_TOP
                            //intent.Call<AndroidJavaObject>("addFlags", 0x08000000); // Intent.FLAG_ACTIVITY_MULTIPLE_TASK
                            //intent.Call<AndroidJavaObject>("addFlags", 0x10000000); // Intent.FLAG_ACTIVITY_NEW_TASK
                            //intent.Call<AndroidJavaObject>("addFlags", 0x20000000); // Intent.FLAG_ACTIVITY_SINGLE_TOP
                            //ca.Call("finish");
                        }
                    }
                }
            }
        }
    }
    internal sealed class FinishActivityCommand : SimpleStoryCommandBase<FinishActivityCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            if (Application.platform == RuntimePlatform.Android) {
                FinishActivity();
            }
            return false;
        }
        private static void FinishActivity()
        {
            using (var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var ca = up.GetStatic<AndroidJavaObject>("currentActivity")) {
                    ca.Call("finish");
                }
            }
        }
    }
    internal sealed class ListenClipboardCommand : SimpleStoryCommandBase<ListenClipboardCommand, StoryValueParam<int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int> _params, long delta)
        {
            int val = _params.Param1Value;
            GmRootScript.ListenClipboard(val);
            return false;
        }
    }
    internal sealed class ListenAndroidCommand : SimpleStoryCommandBase<ListenAndroidCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            GmRootScript.ListenAndroid();
            return false;
        }
    }
    internal sealed class StartServiceCommand : SimpleStoryCommandBase<StartServiceCommand, StoryValueParam<string, string, BoxedValue>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, string, BoxedValue> _params, long delta)
        {
            string srvClass = _params.Param1Value;
            string extraName = _params.Param2Value;
            BoxedValue extraValue = _params.Param3Value;
            GmRootScript.StartService(srvClass, extraName, extraValue);
            return false;
        }
    }
    internal sealed class StopServiceCommand : SimpleStoryCommandBase<StopServiceCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string srvClass = _params.Param1Value;
            GmRootScript.StopService(srvClass);
            return false;
        }
    }
    internal sealed class ShellCommand : SimpleStoryCommandBase<ShellCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string cmd = _params.Param1Value;
            GmRootScript.ExecNoWait(cmd, 0);
            return false;
        }
    }
    internal sealed class ShellTimeoutCommand : SimpleStoryCommandBase<ShellTimeoutCommand, StoryValueParam<string, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, int> _params, long delta)
        {
            string cmd = _params.Param1Value;
            int timeout = _params.Param2Value;
            GmRootScript.ExecNoWait(cmd, timeout);
            return false;
        }
    }
    internal sealed class CleanupCompletedTasksCommand : SimpleStoryCommandBase<CleanupCompletedTasksCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            GmRootScript.CleanupCompletedTasks();
            return false;
        }
    }
    internal sealed class UseJavaTaskCommand : SimpleStoryCommandBase<UseJavaTaskCommand, StoryValueParam<bool>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<bool> _params, long delta)
        {
            GmRootScript.UseJavaTask = _params.Param1Value;
            return false;
        }
    }
    internal sealed class SetClipboardCommand : SimpleStoryCommandBase<SetClipboardCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string text = _params.Param1Value;
            GUIUtility.systemCopyBuffer = text;
            return false;
        }
    }
    internal sealed class EditorBreakCommand : SimpleStoryCommandBase<EditorBreakCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            UnityEngine.Debug.Break();
            return false;
        }
    }
    internal sealed class DebugBreakCommand : SimpleStoryCommandBase<DebugBreakCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            UnityEngine.Debug.DebugBreak();
            return false;
        }
    }
    internal sealed class ClearGlobalsCommand : SimpleStoryCommandBase<ClearGlobalsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            ClientGmStorySystem.Instance.ClearGlobalVariables();
            return false;
        }
    }
    internal sealed class SupportsGfxFormatCommand : SimpleStoryCommandBase<SupportsGfxFormatCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(GraphicsFormat));
            var us = Enum.GetValues(typeof(FormatUsage));
            int ct = 0;
            foreach (var e in vs) {
                var gf = (GraphicsFormat)e;
                try {
                    var rtf = GraphicsFormatUtility.GetRenderTextureFormat(gf);
                    var tf = GraphicsFormatUtility.GetTextureFormat(gf);
                    LogSystem.Warn("[check graphics format {0} <=> rt:{1} tex:{2}]", gf, rtf, tf);
                    foreach (var u in us) {
                        var fu = (FormatUsage)u;
                        if (!SystemInfo.IsFormatSupported(gf, fu)) {
                            LogSystem.Error("can't support graphics format {0} usage {1} <=> rt:{2} tex:{3}", gf, fu, rtf, tf);
                        }
                    }
                    ++ct;
                    if (ct % 20 == 0) {
                        Thread.Sleep(0);
                    }
                }
                catch (Exception ex) {
                    LogSystem.Error("invalid graphics format {0}, exception:{1}", gf, ex.Message);
                }
            }
            return false;
        }
    }
    internal sealed class SupportsTexCommand : SimpleStoryCommandBase<SupportsTexCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(TextureFormat));
            int ct = 0;
            foreach (var e in vs) {
                var tf = (TextureFormat)e;
                try {
                    var gfSrgb = GraphicsFormatUtility.GetGraphicsFormat(tf, true);
                    var gfLinear = GraphicsFormatUtility.GetGraphicsFormat(tf, false);
                    LogSystem.Warn("[check tex {0} <=> srgb:{1} linear:{2}]", tf, gfSrgb, gfLinear);
                    if (!SystemInfo.SupportsTextureFormat(tf)) {
                        LogSystem.Error("can't support tex {0}, srgb:{1} linear:{2}", tf, gfSrgb, gfLinear);
                    }
                    ++ct;
                    if (ct % 300 == 0) {
                        Thread.Sleep(0);
                    }
                }
                catch (Exception ex) {
                    LogSystem.Error("invalid tex format {0}, exception:{1}", tf, ex.Message);
                }
            }
            return false;
        }
    }
    internal sealed class SupportsRTCommand : SimpleStoryCommandBase<SupportsRTCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(RenderTextureFormat));
            int ct = 0;
            foreach (var e in vs) {
                var rtf = (RenderTextureFormat)e;
                try {
                    var gfSrgb = GraphicsFormatUtility.GetGraphicsFormat(rtf, true);
                    var gfLinear = GraphicsFormatUtility.GetGraphicsFormat(rtf, false);
                    LogSystem.Warn("[check RT {0} <=> srgb:{1} linear:{2}]", rtf, gfSrgb, gfLinear);
                    if (!SystemInfo.SupportsRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                    if (!SystemInfo.SupportsBlendingOnRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support blending on RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                    if (!SystemInfo.SupportsRandomWriteOnRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support random write on RT {0}, srgb:{1} linear:{2}", rtf, gfSrgb, gfLinear);
                    }
                    ++ct;
                    if (ct % 100 == 0) {
                        Thread.Sleep(0);
                    }
                }
                catch (Exception ex) {
                    LogSystem.Error("invalid rt format {0}, exception:{1}", rtf, ex.Message);
                }
            }
            return false;
        }
    }
    internal sealed class SupportsVertexAttributeFormatCommand : SimpleStoryCommandBase<SupportsVertexAttributeFormatCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(VertexAttributeFormat));
            int ct = 0;
            foreach (var e in vs) {
                var vaf = (VertexAttributeFormat)e;
                try {
                    for (int dim = 1; dim <= 4; ++dim) {
                        LogSystem.Warn("[check vertex attribute format {0} dim {1}]", vaf, dim);
                        if (!SystemInfo.SupportsVertexAttributeFormat(vaf, dim)) {
                            LogSystem.Error("can't support vertex attribute format {0} dim {1}", vaf, dim);
                        }
                    }
                    ++ct;
                    if (ct % 100 == 0) {
                        Thread.Sleep(0);
                    }
                }
                catch (Exception ex) {
                    LogSystem.Error("invalid vertex attribute format {0}, exception:{1}", vaf, ex.Message);
                }
            }
            return false;
        }
    }
    internal sealed class DeviceSupportsCommand : SimpleStoryCommandBase<DeviceSupportsCommand, StoryValueParam>
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
                log1.AppendFormat("{0} = {1}", pi.Name, v);
                log1.AppendLine();
                ++ct;
                if (ct % 10 == 0)
                {
                    LogSystem.Warn(log1.ToString());
                    log1.Length = 0;
                }

                if (v is bool) {
                    if (!(bool)v) {
                        log2.AppendFormat("{0} = false", pi.Name);
                        log2.AppendLine();
                    }
                }
            }
            LogSystem.Warn(log2.ToString());
            return false;
        }
    }
    internal sealed class LogResolutionsCommand : SimpleStoryCommandBase<LogResolutionsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            foreach (var reso in Screen.resolutions) {
                LogSystem.Warn("resolution:{0}x{1} refresh rate ratio:{2}", reso.width, reso.height, reso.refreshRateRatio);
            }
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal sealed class AllocMemoryCommand : SimpleStoryCommandBase<AllocMemoryCommand, StoryValueParam<string, int>>
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
    internal sealed class FreeMemoryCommand : SimpleStoryCommandBase<FreeMemoryCommand, StoryValueParam<string>>
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
    internal sealed class ConsumeCpuCommand : SimpleStoryCommandBase<ConsumeCpuCommand, StoryValueParam<int>>
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
    internal sealed class GcCommand : SimpleStoryCommandBase<GcCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            GC.Collect();
            return false;
        }
    }
    internal sealed class LogProfilerCommand : SimpleStoryCommandBase<LogProfilerCommand, StoryValueParam>
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
    internal sealed class CmdCommand : SimpleStoryCommandBase<CmdCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string cmd = _params.Param1Value;
            DebugConsole.Execute(cmd);
            return false;
        }
    }
    internal sealed class GmCommand : SimpleStoryCommandBase<GmCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string gmToServer = _params.Param1Value;
            LogSystem.Warn("todo: gm {0}", gmToServer);
            return false;
        }
    }
    internal sealed class PlayerPrefIntCommand : SimpleStoryCommandBase<PlayerPrefIntCommand, StoryValueParam<string, int>>
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
    internal sealed class PlayerPrefFloatCommand : SimpleStoryCommandBase<PlayerPrefFloatCommand, StoryValueParam<string, float>>
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
    internal sealed class PlayerPrefStringCommand : SimpleStoryCommandBase<PlayerPrefStringCommand, StoryValueParam<string, string>>
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
    internal sealed class PrefByJavaCommand : SimpleStoryCommandBase<PrefByJavaCommand, StoryValueParam<string, BoxedValue>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string, BoxedValue> _params, long delta)
        {
            string key = _params.Param1Value;
            BoxedValue val = _params.Param2Value;
            GmRootScript.SetPlayerPrefByJava(key, val);
            return false;
        }
    }
    internal sealed class WeTestTouchCommand : SimpleStoryCommandBase<WeTestTouchCommand, StoryValueParam<int, float, float>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<int, float, float> _params, long delta)
        {
            int action = _params.Param1Value;
            float x = _params.Param2Value;
            float y = _params.Param3Value;
            WeTestAutomation.InjectTouch(action, x, y);
            return false;
        }
    }
    internal sealed class LogMeshCommand : SimpleStoryCommandBase<LogMeshCommand, StoryValueParam<BoxedValue>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue> _params, long delta)
        {
            var meshObj = _params.Param1Value;
            Mesh mesh = StoryScriptUtility.GetMeshArg(ref meshObj);
            if (null != mesh) {
                var sb = new StringBuilder();
                sb.AppendFormat("mesh:{0} vertex count:{1} submesh count:{2} vertex attribute count:{3} vertex buffer count:{4}", mesh, mesh.vertexCount, mesh.subMeshCount, mesh.vertexAttributeCount, mesh.vertexBufferCount);
                sb.AppendLine();
                for(int ix = 0; ix < mesh.vertexAttributeCount; ++ix) {
                    var vattr = mesh.GetVertexAttribute(ix);
                    sb.AppendFormat("\tvattr:{0} attribute:{1} format:{2} dim:{3} stream:{4}", ix, vattr.attribute, vattr.format, vattr.dimension, vattr.stream);
                    sb.AppendLine();
                }
                for (int ix = 0; ix < mesh.vertexBufferCount; ++ix) {
                    var gf = mesh.GetVertexBuffer(ix);
                    sb.AppendFormat("\tvbuffer:{0} handle:0x{1:X} target:{2} stride:{3} count:{4}", ix, gf.bufferHandle.value, gf.target, gf.stride, gf.count);
                    sb.AppendLine();
                }
                for (int ix = 0; ix < mesh.subMeshCount; ++ix) {
                    var submesh = mesh.GetSubMesh(ix);
                    switch (submesh.topology) {
                        case MeshTopology.Triangles:
                            sb.AppendFormat("\tsubmesh:{0} vertex count:{1} index count:{2} topology:{3} triangle count:{4}", ix, submesh.vertexCount, submesh.indexCount, submesh.topology, submesh.indexCount / 3);
                            sb.AppendLine();
                            break;
                        default:
                            sb.AppendFormat("\tsubmesh:{0} vertex count:{1} index count:{2} topology:{3}", ix, submesh.vertexCount, submesh.indexCount, submesh.topology);
                            sb.AppendLine();
                            break;
                    }
                }
                LogSystem.Warn("{0}", sb.ToString());
            }
            return false;
        }
    }
    internal sealed class SetMaterialCommand : SimpleStoryCommandBase<SetMaterialCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var args = _params.Values;
            if (args.Count == 2) {
                var dest = args[0];
                var src = args[1];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var m = StoryScriptUtility.GetMaterialArg(ref src);
                if (null != rd && null != m) {
                    var oldMat = rd.material;
                    rd.material = m;
                    StoryScriptUtility.DestroyObject(oldMat);
                }
            }
            else if (args.Count == 3) {
                var dest = args[0];
                int ix = args[1].GetInt();
                var src = args[2];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var m = StoryScriptUtility.GetMaterialArg(ref src);
                if (null != rd && ix >= 0 && ix < rd.materials.Length && null != m) {
                    var mats = rd.materials;
                    var newMats = new Material[mats.Length];
                    Array.Copy(mats, newMats, mats.Length);
                    var oldMat = mats[ix];
                    newMats[ix] = m;
                    rd.materials = newMats;
                    StoryScriptUtility.DestroyObject(oldMat);
                }
            }
            else if (args.Count == 4) {
                var dest = args[0];
                int ix = args[1].GetInt();
                var src = args[2];
                int six = args[3].GetInt();
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var ms = StoryScriptUtility.GetMaterialsArg(ref src);
                if (null != rd && ix >= 0 && ix < rd.materials.Length && null != ms && six >= 0 && six < ms.Length) {
                    var mats = rd.materials;
                    var newMats = new Material[mats.Length];
                    Array.Copy(mats, newMats, mats.Length);
                    var oldMat = mats[ix];
                    newMats[ix] = ms[six];
                    rd.materials = newMats;
                    StoryScriptUtility.DestroyObject(oldMat);
                }
            }
            return false;
        }
    }
    internal sealed class SetSharedMaterialCommand : SimpleStoryCommandBase<SetSharedMaterialCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var args = _params.Values;
            if (args.Count == 2) {
                var dest = args[0];
                var src = args[1];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var m = StoryScriptUtility.GetMaterialArg(ref src);
                if (null != rd && null != m) {
                    rd.sharedMaterial = m;
                }
            }
            else if (args.Count == 3) {
                var dest = args[0];
                int ix = args[1].GetInt();
                var src = args[2];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var m = StoryScriptUtility.GetMaterialArg(ref src);
                if (null != rd && ix >= 0 && ix < rd.sharedMaterials.Length && null != m) {
                    var mats = rd.materials;
                    var newMats = new Material[mats.Length];
                    Array.Copy(mats, newMats, mats.Length);
                    newMats[ix] = m;
                    rd.sharedMaterials = newMats;
                }
            }
            else if (args.Count == 4) {
                var dest = args[0];
                int ix = args[1].GetInt();
                var src = args[2];
                int six = args[3].GetInt();
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var ms = StoryScriptUtility.GetMaterialsArg(ref src);
                if (null != rd && ix >= 0 && ix < rd.sharedMaterials.Length && null != ms && six >= 0 && six < ms.Length) {
                    var mats = rd.materials;
                    var newMats = new Material[mats.Length];
                    Array.Copy(mats, newMats, mats.Length);
                    newMats[ix] = ms[six];
                    rd.sharedMaterials = newMats;
                }
            }
            return false;
        }
    }
    internal sealed class SetMaterialsCommand : SimpleStoryCommandBase<SetMaterialsCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var args = _params.Values;
            if (args.Count == 2) {
                var dest = args[0];
                var src = args[1];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var ms = StoryScriptUtility.GetMaterialsArg(ref src);
                if (null != rd && null != ms) {
                    rd.materials = ms;
                }
            }
            return false;
        }
    }
    internal sealed class SetSharedMaterialsCommand : SimpleStoryCommandBase<SetSharedMaterialsCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var args = _params.Values;
            if (args.Count == 2) {
                var dest = args[0];
                var src = args[1];
                var rd = StoryScriptUtility.GetRendererArg(ref dest);
                var ms = StoryScriptUtility.GetMaterialsArg(ref src);
                if (null != rd && null != ms) {
                    rd.sharedMaterials = ms;
                }
            }
            return false;
        }
    }
    internal sealed class MaterialSetFloatCommand : SimpleStoryCommandBase<MaterialSetFloatCommand, StoryValueParam<BoxedValue, BoxedValue, float>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue, float> _params, long delta)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            var val = _params.Param3Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    mat.SetFloat(key.AsString, val);
                }
                else if (key.IsInteger) {
                    mat.SetFloat(key.GetInt(), val);
                }
            }
            return false;
        }
    }
    internal sealed class MaterialSetIntCommand : SimpleStoryCommandBase<MaterialSetIntCommand, StoryValueParam<BoxedValue, BoxedValue, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue, int> _params, long delta)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            var val = _params.Param3Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    mat.SetInt(key.AsString, val);
                }
                else if (key.IsInteger) {
                    mat.SetInt(key.GetInt(), val);
                }
            }
            return false;
        }
    }
    internal sealed class MaterialSetIntegerCommand : SimpleStoryCommandBase<MaterialSetIntegerCommand, StoryValueParam<BoxedValue, BoxedValue, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue, int> _params, long delta)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            var val = _params.Param3Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    mat.SetInteger(key.AsString, val);
                }
                else if (key.IsInteger) {
                    mat.SetInteger(key.GetInt(), val);
                }
            }
            return false;
        }
    }
    internal sealed class MaterialSetVectorCommand : SimpleStoryCommandBase<MaterialSetVectorCommand, StoryValueParam<BoxedValue, BoxedValue, BoxedValue>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue, BoxedValue> _params, long delta)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            var val = _params.Param3Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                var v = val.GetVector4();
                if (key.IsString) {
                    mat.SetVector(key.AsString, v);
                }
                else if (key.IsInteger) {
                    mat.SetVector(key.GetInt(), v);
                }
            }
            return false;
        }
    }
    internal sealed class MaterialSetColorCommand : SimpleStoryCommandBase<MaterialSetColorCommand, StoryValueParam<BoxedValue, BoxedValue, BoxedValue>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue, BoxedValue> _params, long delta)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            var val = _params.Param3Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                Color v;
                if (val.IsString) {
                    v = StoryScriptUtility.GetColor(val.AsString);
                }
                else {
                    v = val.GetColor();
                }
                if (key.IsString) {
                    mat.SetColor(key.AsString, v);
                }
                else if (key.IsInteger) {
                    mat.SetColor(key.GetInt(), v);
                }
            }
            return false;
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------
    internal sealed class IsDebugFunction : SimpleStoryFunctionBase<IsDebugFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = StoryConfigManager.Instance.IsDebug;
        }
    }
    internal sealed class IsDevelopmentFunction : SimpleStoryFunctionBase<IsDevelopmentFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = StoryConfigManager.Instance.IsDevelopment;
        }
    }
    internal sealed class TypeOfFunction : SimpleStoryFunctionBase<TypeOfFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            var args = _params.Values;
            string type = string.Empty;
            if (args.Count == 1) {
                type = args[0].AsString;
            }
            else if (args.Count == 2) {
                type = args[0].AsString + ", " + args[1].AsString;
            }
            if (string.IsNullOrEmpty(type))
                result.Value = BoxedValue.NullObject;
            else
                result.Value = StoryScriptUtility.GetType(type);
        }
    }
    internal sealed class GetMonoMemoryFunction : SimpleStoryFunctionBase<GetMonoMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = GC.GetTotalMemory(false) / 1024.0f / 1024.0f;
        }
    }
    internal sealed class GetNativeMemoryFunction : SimpleStoryFunctionBase<GetNativeMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalAllocatedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal sealed class GetGfxMemoryFunction : SimpleStoryFunctionBase<GetGfxMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1024.0f / 1024.0f;
        }
    }
    internal sealed class GetUnusedMemoryFunction : SimpleStoryFunctionBase<GetUnusedMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalUnusedReservedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal sealed class GetTotalMemoryFunction : SimpleStoryFunctionBase<GetTotalMemoryFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Profiler.GetTotalReservedMemoryLong() / 1024.0f / 1024.0f;
        }
    }
    internal sealed class DeviceInfoFunction : SimpleStoryFunctionBase<DeviceInfoFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = SystemInfo.deviceName + " | " + SystemInfo.deviceModel + " | "
                + SystemInfo.graphicsDeviceName + " | " + SystemInfo.graphicsDeviceVersion + " | "
                + SystemInfo.operatingSystem + " | " + SystemInfo.processorType;
        }
    }
    internal sealed class GetClipboardFunction : SimpleStoryFunctionBase<GetClipboardFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = GUIUtility.systemCopyBuffer;
        }
    }
    internal sealed class GetBoolFunction : SimpleStoryFunctionBase<GetBoolFunction, StoryValueParam<string, bool>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, bool> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<bool>("getBooleanExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetBoolArrayFunction : SimpleStoryFunctionBase<GetBoolArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<bool[]>("getBooleanArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetStringFunction : SimpleStoryFunctionBase<GetStringFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<string>("getStringExtra", str);
            }
#endif
        }
    }
    internal sealed class GetStringArrayFunction : SimpleStoryFunctionBase<GetStringArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<string[]>("getStringArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetCharFunction : SimpleStoryFunctionBase<GetCharFunction, StoryValueParam<string, char>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, char> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<char>("getCharExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetCharArrayFunction : SimpleStoryFunctionBase<GetCharArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<char[]>("getCharArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetByteFunction : SimpleStoryFunctionBase<GetByteFunction, StoryValueParam<string, sbyte>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, sbyte> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<sbyte>("getByteExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetByteArrayFunction : SimpleStoryFunctionBase<GetByteArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<sbyte[]>("getByteArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetShortFunction : SimpleStoryFunctionBase<GetShortFunction, StoryValueParam<string, short>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, short> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<short>("getShortExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetShortArrayFunction : SimpleStoryFunctionBase<GetShortArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<short[]>("getShortArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetIntFunction : SimpleStoryFunctionBase<GetIntFunction, StoryValueParam<string, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, int> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<int>("getIntExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetIntArrayFunction : SimpleStoryFunctionBase<GetIntArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<int[]>("getIntArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetLongFunction : SimpleStoryFunctionBase<GetLongFunction, StoryValueParam<string, long>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, long> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<long>("getLongExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetLongArrayFunction : SimpleStoryFunctionBase<GetLongArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<long[]>("getLongArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetFloatFunction : SimpleStoryFunctionBase<GetFloatFunction, StoryValueParam<string, float>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, float> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<float>("getFloatExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetFloatArrayFunction : SimpleStoryFunctionBase<GetFloatArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<float[]>("getFloatArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetDoubleFunction : SimpleStoryFunctionBase<GetDoubleFunction, StoryValueParam<string, double>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, double> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            var def = _params.Param2Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = intent.Call<double>("getDoubleExtra", str, def);
            }
#endif
        }
    }
    internal sealed class GetDoubleArrayFunction : SimpleStoryFunctionBase<GetDoubleArrayFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var str = _params.Param1Value;
            if (!string.IsNullOrEmpty(str)) {
                var act = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var intent = act.Call<AndroidJavaObject>("getIntent");
                result.Value = BoxedValue.FromObject(intent.Call<double[]>("getDoubleArrayExtra", str));
            }
#endif
        }
    }
    internal sealed class GetActivityClassNameFunction : SimpleStoryFunctionBase<GetActivityClassNameFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
#if UNITY_ANDROID
            var javaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            string fullClassName = javaObject.Call<AndroidJavaObject>("getClass").Call<string>("getName");
            result.Value = fullClassName;
#endif
        }
    }
    internal sealed class PlayerPrefIntFunction : SimpleStoryFunctionBase<PlayerPrefIntFunction, StoryValueParam<string, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, int> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            int def = _params.Param2Value;
            int val = PlayerPrefs.GetInt(key, def);

            result.Value = val;
        }
    }
    internal sealed class PlayerPrefFloatFunction : SimpleStoryFunctionBase<PlayerPrefFloatFunction, StoryValueParam<string, float>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, float> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            float def = _params.Param2Value;
            float val = PlayerPrefs.GetFloat(key, def);

            result.Value = val;
        }
    }
    internal sealed class PlayerPrefStringFunction : SimpleStoryFunctionBase<PlayerPrefStringFunction, StoryValueParam<string, string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, string> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            string def = _params.Param2Value;
            string val = PlayerPrefs.GetString(key, def);

            result.Value = val;
        }
    }
    internal sealed class PrefByJavaFunction : SimpleStoryFunctionBase<PrefByJavaFunction, StoryValueParam<string, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, BoxedValue> _params, StoryValueResult result)
        {
            string key = _params.Param1Value;
            BoxedValue def = _params.Param2Value;
            result.Value = GmRootScript.GetPlayerPrefByJava(key, def);
        }
    }
    internal sealed class IsFormatSupportedFunction : SimpleStoryFunctionBase<IsFormatSupportedFunction, StoryValueParam<string, string>>
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
    internal sealed class GetCompatibleFormatFunction : SimpleStoryFunctionBase<GetCompatibleFormatFunction, StoryValueParam<string, string>>
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
    internal sealed class GetGraphicsFormatFunction : SimpleStoryFunctionBase<GetGraphicsFormatFunction, StoryValueParam<string>>
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
    internal sealed class GetMSAASampleCountFunction : SimpleStoryFunctionBase<GetMSAASampleCountFunction, StoryValueParam<int, int, string, int, int>>
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
    internal sealed class SystemInfoFunction : SimpleStoryFunctionBase<SystemInfoFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = typeof(SystemInfo);
        }
    }
    internal sealed class ScreenFunction : SimpleStoryFunctionBase<ScreenFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = typeof(Screen);
        }
    }
    internal sealed class ScreenWidthFunction : SimpleStoryFunctionBase<ScreenWidthFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Screen.width;
        }
    }
    internal sealed class ScreenHeightFunction : SimpleStoryFunctionBase<ScreenHeightFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Screen.height;
        }
    }
    internal sealed class ScreenDPIFunction : SimpleStoryFunctionBase<ScreenDPIFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Screen.dpi;
        }
    }
    internal sealed class ApplicationFunction : SimpleStoryFunctionBase<ApplicationFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = typeof(Application);
        }
    }
    internal sealed class AppIdFunction : SimpleStoryFunctionBase<AppIdFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.identifier;
        }
    }
    internal sealed class AppNameFunction : SimpleStoryFunctionBase<AppNameFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.productName;
        }
    }
    internal sealed class PlatformFunction : SimpleStoryFunctionBase<PlatformFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.platform.ToString();
        }
    }
    internal sealed class IsEditorFunction : SimpleStoryFunctionBase<IsEditorFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.isEditor;
        }
    }
    internal sealed class IsConsoleFunction : SimpleStoryFunctionBase<IsConsoleFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.isConsolePlatform;
        }
    }
    internal sealed class IsMobileFunction : SimpleStoryFunctionBase<IsMobileFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.isMobilePlatform;
        }
    }
    internal sealed class IsAndroidFunction : SimpleStoryFunctionBase<IsAndroidFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.platform == RuntimePlatform.Android;
        }
    }
    internal sealed class IsIPhoneFunction : SimpleStoryFunctionBase<IsIPhoneFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }
    internal sealed class IsPCFunction : SimpleStoryFunctionBase<IsPCFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = !Application.isMobilePlatform && !Application.isConsolePlatform;
        }
    }
    internal sealed class ShellFunction : SimpleStoryFunctionBase<ShellFunction, StoryValueParam<string>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string> _params, StoryValueResult result)
        {
            string cmd = _params.Param1Value;
            result.Value = GmRootScript.Exec(cmd, 0);
        }
    }
    internal sealed class ShellTimeoutFunction : SimpleStoryFunctionBase<ShellTimeoutFunction, StoryValueParam<string, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, int> _params, StoryValueResult result)
        {
            string cmd = _params.Param1Value;
            int timeout = _params.Param2Value;
            result.Value = GmRootScript.Exec(cmd, timeout);
        }
    }
    internal sealed class IsJavaTaskFunction : SimpleStoryFunctionBase<IsJavaTaskFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = GmRootScript.UseJavaTask;
        }
    }
    internal sealed class GetTaskCountFunction : SimpleStoryFunctionBase<GetTaskCountFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = GmRootScript.UseJavaTask ? GmRootScript.JavaTasks.Count : GmRootScript.Tasks.Count;
        }
    }
    internal sealed class WeTestGetXFunction : SimpleStoryFunctionBase<WeTestGetXFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = WeTestAutomation.GetX();
        }
    }
    internal sealed class WeTestGetYFunction : SimpleStoryFunctionBase<WeTestGetYFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = WeTestAutomation.GetY();
        }
    }
    internal sealed class WeTestGetWidthFunction : SimpleStoryFunctionBase<WeTestGetWidthFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = WeTestAutomation.GetWidth();
        }
    }
    internal sealed class WeTestGetHeightFunction : SimpleStoryFunctionBase<WeTestGetHeightFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = WeTestAutomation.GetHeight();
        }
    }
    internal sealed class GetPointerFunction : SimpleStoryFunctionBase<GetPointerFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = Input.mousePosition;
        }
    }
    internal sealed class PointerRaycastUisFunction : SimpleStoryFunctionBase<PointerRaycastUisFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = BoxedValue.FromObject(RaycastUisFunction.GetUiObjectsUnderMouse(Input.mousePosition));
        }
    }
    internal sealed class GetPointerComponentsFunction : SimpleStoryFunctionBase<GetPointerComponentsFunction, StoryValueParam<object, bool>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<object, bool> _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            var typeObj = _params.Param1Value;
            bool include_inactive = _params.Param2Value;
            var list = RaycastComponentsFunction.GetUiComponentsUnderMouse(Input.mousePosition, typeObj, include_inactive);
            if (null != list) {
                result.Value = BoxedValue.FromObject(list);
            }
        }
    }
    internal sealed class RaycastUisFunction : SimpleStoryFunctionBase<RaycastUisFunction, StoryValueParam<float, float>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<float, float> _params, StoryValueResult result)
        {
            float x = _params.Param1Value;
            float y = _params.Param2Value;
            result.Value = BoxedValue.FromObject(GetUiObjectsUnderMouse(new Vector2(x, y)));
        }
        internal static List<UnityEngine.EventSystems.RaycastResult> GetUiObjectsUnderMouse(Vector2 pos)
        {
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();

            var eventSystems = GameObject.FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            var graphicRaycasters = GameObject.FindObjectsOfType<UnityEngine.UI.GraphicRaycaster>();

            foreach (var eventSystem in eventSystems) {
                foreach (var graphicRaycaster in graphicRaycasters) {
                    var pointerEventData = new UnityEngine.EventSystems.PointerEventData(eventSystem);
                    pointerEventData.position = pos;

                    graphicRaycaster.Raycast(pointerEventData, results);
                }
            }

            return results;
        }
    }
    internal sealed class RaycastComponentsFunction : SimpleStoryFunctionBase<RaycastComponentsFunction, StoryValueParam<float, float, object, bool>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<float, float, object, bool> _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            float x = _params.Param1Value;
            float y = _params.Param2Value;
            var typeObj = _params.Param3Value;
            bool include_inactive = _params.Param4Value;
            var list = GetUiComponentsUnderMouse(new Vector2(x, y), typeObj, include_inactive);
            if (null != list) {
                result.Value = BoxedValue.FromObject(list);
            }
        }
        internal static List<Component> GetUiComponentsUnderMouse(Vector2 pos, object typeObj, bool include_inactive)
        {
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr) {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type) {
                var list = new List<Component>();
                var objs = RaycastUisFunction.GetUiObjectsUnderMouse(pos);
                foreach (var obj in objs) {
                    var comp = obj.gameObject.GetComponent(type);
                    if (null != comp) {
                        list.Add(comp);
                    }
                    else {
                        comp = obj.gameObject.GetComponentInParent(type, include_inactive);
                        if (null != comp) {
                            list.Add(comp);
                        }
                        else {
                            comp = obj.gameObject.GetComponentInChildren(type, include_inactive);
                            if (null != comp) {
                                list.Add(comp);
                            }
                        }
                    }
                }
                return list;
            }
            return null;
        }
    }
    internal sealed class GetScenePathFunction : SimpleStoryFunctionBase<GetScenePathFunction, StoryValueParam<System.Collections.IList, object, int>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<System.Collections.IList, object, int> _params, StoryValueResult result)
        {
            var prefixs = _params.Param1Value;
            var obj = _params.Param2Value;
            int up_level = _params.Param3Value;
            result.Value = GetScenePath(prefixs, obj, up_level);
        }
        internal static string GetScenePath(System.Collections.IList prefixs, object obj, int up_level)
        {
            var tr = obj as Transform;
            if (null == tr) {
                var gobj = obj as GameObject;
                var comp = obj as Component;
                if (null != gobj) {
                    tr = gobj.transform;
                }
                else if (null != comp) {
                    tr = comp.transform;
                }
                else {
                    var str = obj as string;
                    if (!string.IsNullOrEmpty(str)) {
                        gobj = GameObject.Find(str);
                        if (null != gobj) {
                            tr = gobj.transform;
                        }
                    }
                }
            }
            var sb = new StringBuilder();
            if (tr != null) {
                foreach (var prefix in prefixs) {
                    sb.Append(prefix);
                }
                sb.Append(StoryScriptUtility.GetScenePath(tr, up_level));
            }
            return sb.ToString();
        }
    }
    internal sealed class ShaderFunction : SimpleStoryFunctionBase<ShaderFunction, StoryValueParam>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam _params, StoryValueResult result)
        {
            result.Value = typeof(Shader);
        }
    }
    internal sealed class GetMaterialFunction : SimpleStoryFunctionBase<GetMaterialFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            if (_params.Values.Count == 1) {
                var matObj = _params.Values[0];
                var mat = StoryScriptUtility.GetMaterialArg(ref matObj);
                result.Value = mat;
            }
            else if (_params.Values.Count == 2) {
                var matObj = _params.Values[0];
                var matIx = _params.Values[1].GetInt();
                var mats = StoryScriptUtility.GetMaterialsArg(ref matObj);
                if (null != mats && matIx >= 0 && matIx < mats.Length) {
                    result.Value = mats[matIx];
                }
            }
        }
    }
    internal sealed class GetSharedMaterialFunction : SimpleStoryFunctionBase<GetSharedMaterialFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            if (_params.Values.Count == 1) {
                var matObj = _params.Values[0];
                var mat = StoryScriptUtility.GetSharedMaterialArg(ref matObj);
                result.Value = mat;
            }
            else if (_params.Values.Count == 2) {
                var matObj = _params.Values[0];
                var matIx = _params.Values[1].GetInt();
                var mats = StoryScriptUtility.GetSharedMaterialsArg(ref matObj);
                if (null != mats && matIx >= 0 && matIx < mats.Length) {
                    result.Value = mats[matIx];
                }
            }
        }
    }
    internal sealed class MaterialGetFloatFunction : SimpleStoryFunctionBase<MaterialGetFloatFunction, StoryValueParam<BoxedValue, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue> _params, StoryValueResult result)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    result.Value = mat.GetFloat(key.AsString);
                }
                else if(key.IsInteger) {
                    result.Value = mat.GetFloat(key.GetInt());
                }
            }
        }
    }
    internal sealed class MaterialGetIntFunction : SimpleStoryFunctionBase<MaterialGetIntFunction, StoryValueParam<BoxedValue, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue> _params, StoryValueResult result)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    result.Value = mat.GetInt(key.AsString);
                }
                else if (key.IsInteger) {
                    result.Value = mat.GetInt(key.GetInt());
                }
            }
        }
    }
    internal sealed class MaterialGetIntegerFunction : SimpleStoryFunctionBase<MaterialGetIntegerFunction, StoryValueParam<BoxedValue, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue> _params, StoryValueResult result)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    result.Value = mat.GetInteger(key.AsString);
                }
                else if (key.IsInteger) {
                    result.Value = mat.GetInteger(key.GetInt());
                }
            }
        }
    }
    internal sealed class MaterialGetVectorFunction : SimpleStoryFunctionBase<MaterialGetVectorFunction, StoryValueParam<BoxedValue, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue> _params, StoryValueResult result)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    result.Value = mat.GetVector(key.AsString);
                }
                else if (key.IsInteger) {
                    result.Value = mat.GetVector(key.GetInt());
                }
            }
        }
    }
    internal sealed class MaterialGetColorFunction : SimpleStoryFunctionBase<MaterialGetColorFunction, StoryValueParam<BoxedValue, BoxedValue>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<BoxedValue, BoxedValue> _params, StoryValueResult result)
        {
            var matObj = _params.Param1Value;
            var key = _params.Param2Value;
            Material mat = StoryScriptUtility.GetMaterialArg(ref matObj);
            if (null != mat) {
                if (key.IsString) {
                    result.Value = mat.GetColor(key.AsString);
                }
                else if (key.IsInteger) {
                    result.Value = mat.GetColor(key.GetInt());
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------
    internal sealed class LogComponentsCommand : SimpleStoryCommandBase<LogComponentsCommand, StoryValueParam<string, System.Collections.IList, object, int, bool>>
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
            foreach (var v in vals) {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr) {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type) {
                if (string.IsNullOrEmpty(root)) {
                    var objs = GameObject.FindObjectsOfType(type, include_inactive);
                    foreach (var obj in objs) {
                        var comp = obj as Component;
                        if (null != comp) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
                                LogSystem.Warn("{0}", StoryScriptUtility.GetScenePath(comp.transform, up_level));
                            }
                        }
                    }
                }
                else {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj) {
                        var comps = rootObj.GetComponentsInChildren(type, include_inactive);
                        foreach (var comp in comps) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
                                LogSystem.Warn("{0}", StoryScriptUtility.GetScenePath(comp.transform, up_level));
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
    internal sealed class LogScenePathCommand : SimpleStoryCommandBase<LogScenePathCommand, StoryValueParam<System.Collections.IList, object, int>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<System.Collections.IList, object, int> _params, long delta)
        {
            var prefixs = _params.Param1Value;
            var obj = _params.Param2Value;
            int up_level = _params.Param3Value;
            LogSystem.Warn(GetScenePathFunction.GetScenePath(prefixs, obj, up_level));
            return false;
        }
    }
    internal sealed class ClickUiCommand : SimpleStoryCommandBase<ClickUiCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals) {
                names.Add(v.ToString());
            }
            var btns0 = GameObject.FindObjectsOfType<Button>(false);
            foreach (var btn in btns0) {
                if (StoryScriptUtility.IsPathMatch(btn.transform, names)) {
                    btn.onClick.Invoke();
                    break;
                }
            }
            return false;
        }
    }
    internal sealed class ToggleOnCommand : SimpleStoryCommandBase<ToggleOnCommand, StoryValueParams>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals) {
                names.Add(v.ToString());
            }
            var toggles0 = GameObject.FindObjectsOfType<Toggle>(false);
            foreach (var toggle in toggles0) {
                if (StoryScriptUtility.IsPathMatch(toggle.transform, names)) {
                    toggle.isOn = !toggle.isOn;
                    break;
                }
            }
            return false;
        }
    }
    internal sealed class ClickOnPointerCommand : SimpleStoryCommandBase<ClickOnPointerCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            ClickOnPosCommand.ClickFirstOnPos(Input.mousePosition);
            return false;
        }
    }
    internal sealed class ToggleOnPointerCommand : SimpleStoryCommandBase<ToggleOnPointerCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            ToggleOnPosCommand.ClickFirstOnPos(Input.mousePosition);
            return false;
        }
    }
    internal sealed class ClickOnPosCommand : SimpleStoryCommandBase<ClickOnPosCommand, StoryValueParam<float, float>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<float, float> _params, long delta)
        {
            float x = _params.Param1Value;
            float y = _params.Param2Value;
            ClickFirstOnPos(new Vector2(x, y));
            return false;
        }
        internal static void ClickFirstOnPos(Vector2 pos)
        {
            var objs = RaycastUisFunction.GetUiObjectsUnderMouse(pos);
            Button firstButton = null;
            foreach (var obj in objs) {
                var btn0 = obj.gameObject.GetComponent<Button>();
                if (null != btn0) {
                    firstButton = btn0;
                    break;
                }
                else {
                    btn0 = obj.gameObject.GetComponentInParent<Button>();
                    if (null != btn0) {
                        firstButton = btn0;
                        break;
                    }
                    else {
                        btn0 = obj.gameObject.GetComponentInChildren<Button>();
                        if (null != btn0) {
                            firstButton = btn0;
                            break;
                        }
                    }
                }
            }
            if (null != firstButton) {
                firstButton.onClick.Invoke();
            }
        }
    }
    internal sealed class ToggleOnPosCommand : SimpleStoryCommandBase<ToggleOnPosCommand, StoryValueParam<float, float>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<float, float> _params, long delta)
        {
            float x = _params.Param1Value;
            float y = _params.Param2Value;
            ClickFirstOnPos(new Vector2(x, y));
            return false;
        }
        internal static void ClickFirstOnPos(Vector2 pos)
        {
            var objs = RaycastUisFunction.GetUiObjectsUnderMouse(pos);
            Toggle firstToggle = null;
            foreach (var obj in objs) {
                var toggle0 = obj.gameObject.GetComponent<Toggle>();
                if (null != toggle0) {
                    firstToggle = toggle0;
                    break;
                }
                else {
                    toggle0 = obj.gameObject.GetComponentInParent<Toggle>();
                    if (null != toggle0) {
                        firstToggle = toggle0;
                        break;
                    }
                    else {
                        toggle0 = obj.gameObject.GetComponentInChildren<Toggle>();
                        if (null != toggle0) {
                            firstToggle = toggle0;
                            break;
                        }
                    }
                }
            }
            if (null != firstToggle) {
                firstToggle.isOn = !firstToggle.isOn;
            }
        }
    }
    internal sealed class ClickCommand : SimpleStoryCommandBase<ClickCommand, StoryValueParam<object>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<object> _params, long delta)
        {
            var obj = _params.Param1Value;
            var btn = obj as Button;
            if (null != btn) {
                btn.onClick.Invoke();
            }
            return false;
        }
    }
    internal sealed class ToggleCommand : SimpleStoryCommandBase<ToggleCommand, StoryValueParam<object>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<object> _params, long delta)
        {
            var obj = _params.Param1Value;
            var toggle = obj as Toggle;
            if (null != toggle) {
                toggle.isOn = !toggle.isOn;
            }
            return false;
        }
    }
    internal sealed class LogCompiledPerfsCommand : SimpleStoryCommandBase<LogCompiledPerfsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            PerfGradeGm.LogCompiledPerfGrades();
            return false;
        }
    }
    internal sealed class ReloadPerfsCommand : SimpleStoryCommandBase<ReloadPerfsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            Main.TestPerfGrade();
            return false;
        }
    }
    internal sealed class RunPerfCommand : SimpleStoryCommandBase<RunPerfCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            var perfFile = _params.Param1Value;
            if (!Path.IsPathRooted(perfFile)) {
                perfFile = PerfGradeGm.ScriptPath + perfFile;
            }
            PerfGradeGm.ClearPerfGradeScripts();
            PerfGradeGm.LoadPerfGradeScript(perfFile);
            PerfGradeGm.RunPerfGrade();
            return false;
        }
    }
    internal sealed class CompilePerfCommand : SimpleStoryCommandBase<CompilePerfCommand, StoryValueParam<string>>
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
    internal sealed class LoadUiCommand : SimpleStoryCommandBase<LoadUiCommand, StoryValueParam<Dsl.ValueData>>
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
    internal sealed class ShowUiCommand : SimpleStoryCommandBase<ShowUiCommand, StoryValueParam>
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
    internal sealed class HideUiCommand : SimpleStoryCommandBase<HideUiCommand, StoryValueParam>
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
    internal sealed class FindRawImageFunction : SimpleStoryFunctionBase<FindRawImageFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<RawImage>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiImageFunction : SimpleStoryFunctionBase<FindUiImageFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<Image>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiButtonFunction : SimpleStoryFunctionBase<FindUiButtonFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<Button>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiToggleFunction : SimpleStoryFunctionBase<FindUiToggleFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<Toggle>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiSliderFunction : SimpleStoryFunctionBase<FindUiSliderFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<Slider>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiInputFunction : SimpleStoryFunctionBase<FindUiInputFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<InputField>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindTmpInputFunction : SimpleStoryFunctionBase<FindTmpInputFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<TMPro.TMP_InputField>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindUiDropdownFunction : SimpleStoryFunctionBase<FindUiDropdownFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<Dropdown>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindTmpDropdownFunction : SimpleStoryFunctionBase<FindTmpDropdownFunction, StoryValueParams>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;

            var vals = _params.Values;
            var names = new List<string>();
            foreach (var v in vals)
            {
                names.Add(v.ToString());
            }
            var comp0 = GameObject.FindObjectsOfType<TMPro.TMP_Dropdown>(false);
            foreach (var comp in comp0)
            {
                if (StoryScriptUtility.IsPathMatch(comp.transform, names))
                {
                    result.Value = comp;
                    break;
                }
            }
        }
    }
    internal sealed class FindComponentFunction : SimpleStoryFunctionBase<FindComponentFunction, StoryValueParam<string, System.Collections.IList, object, bool>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, System.Collections.IList, object, bool> _params, StoryValueResult result)
        {
            result.Value = BoxedValue.NullObject;
            var root = _params.Param1Value;
            var vals = _params.Param2Value;
            var typeObj = _params.Param3Value;
            bool include_inactive = _params.Param4Value;
            var names = new List<string>();
            foreach (var v in vals) {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr) {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type) {
                if (string.IsNullOrEmpty(root)) {
                    var objs = GameObject.FindObjectsOfType(type, include_inactive);
                    foreach (var obj in objs) {
                        var comp = obj as Component;
                        if (null != comp) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
                                result.Value = comp;
                                return;
                            }
                        }
                    }
                }
                else {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj) {
                        var comps = rootObj.GetComponentsInChildren(type, include_inactive);
                        foreach (var comp in comps) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
                                result.Value = comp;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    internal sealed class SearchComponentsFunction : SimpleStoryFunctionBase<SearchComponentsFunction, StoryValueParam<string, System.Collections.IList, object, bool>>
    {
        protected override void UpdateValue(StoryInstance instance, StoryValueParam<string, System.Collections.IList, object, bool> _params, StoryValueResult result)
        {
            var list = new List<Component>();
            var root = _params.Param1Value;
            var vals = _params.Param2Value;
            var typeObj = _params.Param3Value;
            bool include_inactive = _params.Param4Value;
            var names = new List<string>();
            foreach (var v in vals) {
                names.Add(v.ToString());
            }
            var type = typeObj as Type;
            var typeStr = typeObj as string;
            if (null != typeStr) {
                type = StoryScriptUtility.GetType(typeStr);
            }
            if (null != type) {
                if (string.IsNullOrEmpty(root)) {
                    var objs = GameObject.FindObjectsOfType(type, include_inactive);
                    foreach (var obj in objs) {
                        var comp = obj as Component;
                        if (null != comp) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
                                list.Add(comp);
                            }
                        }
                    }
                }
                else {
                    var rootObj = GameObject.Find(root);
                    if (null != rootObj) {
                        var comps = rootObj.GetComponentsInChildren(type, include_inactive);
                        foreach (var comp in comps) {
                            if (StoryScriptUtility.IsPathMatch(comp.transform, names)) {
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
