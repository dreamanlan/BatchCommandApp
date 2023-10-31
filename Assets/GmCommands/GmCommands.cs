using System;
using System.Collections.Generic;
using StoryScript;
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Assertions.Must;
using System.Linq.Expressions;

namespace GmCommands
{
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
            foreach(var e in vs) {
                var rtf = (RenderTextureFormat)e;
                try {
                    if (!SystemInfo.SupportsRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support RT {0}", rtf);
                    }
                }
                catch {
                    LogSystem.Error("invalid rt format {0}", rtf);
                }
            }
            return false;
        }
    }
    internal class SupportsBlendingOnRTCommand : SimpleStoryCommandBase<SupportsBlendingOnRTCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var vs = Enum.GetValues(typeof(RenderTextureFormat));
            foreach (var e in vs) {
                var rtf = (RenderTextureFormat)e;
                try {
                    if (!SystemInfo.SupportsBlendingOnRenderTextureFormat(rtf)) {
                        LogSystem.Error("can't support blending on {0}", rtf);
                    }
                }
                catch {
                    LogSystem.Error("invalid rt format {0}", rtf);
                }
            }
            return false;
        }
    }
    internal class DeviceSupportsCommand : SimpleStoryCommandBase<DeviceSupportsCommand, StoryValueParam>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam _params, long delta)
        {
            var t = typeof(SystemInfo);
            var pis = t.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach(var pi in pis) {
                var v = pi.GetValue(null);
                if(v is bool) {
                    if (!(bool)v) {
                        LogSystem.Error("{0} = false", pi.Name);
                    }
                }
                else {
                    LogSystem.Info("{0} = {1}", pi.Name, v);
                }
            }
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
            } else {
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
            } else {
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
    //---------------------------------------------------------------------------------------------------------------------------------
    internal class IsFormatSupportedValue : SimpleStoryValueBase<IsFormatSupportedValue, StoryValueParam<string, string>>
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
    internal class GetCompatibleFormatValue : SimpleStoryValueBase<GetCompatibleFormatValue, StoryValueParam<string, string>>
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
    internal class GetGraphicsFormatValue : SimpleStoryValueBase<GetGraphicsFormatValue, StoryValueParam<string>>
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
    internal class GetMSAASampleCountValue : SimpleStoryValueBase<GetMSAASampleCountValue, StoryValueParam<int, int, string, int, int>>
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
    internal class LoadUiCommand : SimpleStoryCommandBase<LoadUiCommand, StoryValueParam<string>>
    {
        protected override bool ExecCommand(StoryInstance instance, StoryValueParam<string> _params, long delta)
        {
            string uiRes = _params.Param1Value;

            var fv = GameObject.Find("GmScript");
            if (null != fv) {
                var uihandler = fv.GetComponent<UiHanlder>();
                if (null != uihandler) {
                    uihandler.LoadUi(uiRes);
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
            var fv = GameObject.Find("GmScript");
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
            var fv = GameObject.Find("GmScript");
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

}
