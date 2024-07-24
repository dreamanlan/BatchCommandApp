using System;
using System.Collections.Generic;
using StoryScript;

namespace GmCommands
{
    public sealed class ClientGmStorySystem
    {
        public void Init()
        {
            try {
                StoryCommandManager.ThreadCommandGroupsMask = 1 << (int)StoryCommandGroupDefine.GM;
                StoryFunctionManager.ThreadFunctionGroupsMask = 1 << (int)StoryFunctionGroupDefine.GM;

                RegisterCommon();

                //register Gm command
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setdebug", "setdebug(1_or_0) command", new StoryCommandFactoryHelper<SetDebugCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "listenclipboard", "listenclipboard(interval) command", new StoryCommandFactoryHelper<ListenClipboardCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "listenandroid", "listenandroid() command", new StoryCommandFactoryHelper<ListenAndroidCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "startservice", "startservice(srv_class, extra_name, extra_val) command", new StoryCommandFactoryHelper<StartServiceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "stopservice", "stopservice(srv_class) command", new StoryCommandFactoryHelper<StopServiceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "shell", "shell(cmd) command", new StoryCommandFactoryHelper<ShellCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "shelltimeout", "shelltimeout(cmd,ms) command", new StoryCommandFactoryHelper<ShellTimeoutCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "cleanupcompletedtasks", "cleanupcompletedtasks() command", new StoryCommandFactoryHelper<CleanupCompletedTasksCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setclipboard", "setclipboard(text) command", new StoryCommandFactoryHelper<SetClipboardCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "editorbreak", "editorbreak() command", new StoryCommandFactoryHelper<EditorBreakCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "debugbreak", "debugbreak() command", new StoryCommandFactoryHelper<DebugBreakCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "clearglobals", "clearglobals() command", new StoryCommandFactoryHelper<ClearGlobalsCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsgf", "supportsgf() command, print unsupported graphics format", new StoryCommandFactoryHelper<SupportsGfxFormatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportstex", "supportstex() command, print unsupported tex", new StoryCommandFactoryHelper<SupportsTexCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsrt", "supportsrt() command, print unsupported rt", new StoryCommandFactoryHelper<SupportsRTCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsva", "supportsva() command, print unsupported vertex attribute format", new StoryCommandFactoryHelper<SupportsVertexAttributeFormatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "devicesupports", "devicesupports() command, print unsupported feature", new StoryCommandFactoryHelper<DeviceSupportsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logresolutions", "logresolutions() command, print supported resolutions", new StoryCommandFactoryHelper<LogResolutionsCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "allocmemory", "allocmemory(key,size) command", new StoryCommandFactoryHelper<AllocMemoryCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "freememory", "freememory(key) command", new StoryCommandFactoryHelper<FreeMemoryCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "consumecpu", "consumecpu(time) command", new StoryCommandFactoryHelper<ConsumeCpuCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gc", "gc() command, force Garbage Collect", new StoryCommandFactoryHelper<GcCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logprofiler", "logprofiler() command", new StoryCommandFactoryHelper<LogProfilerCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "cmd", "cmd(str) command", new StoryCommandFactoryHelper<CmdCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefint", "prefint(key,val) command", new StoryCommandFactoryHelper<PlayerPrefIntCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "preffloat", "preffloat(key,val) command", new StoryCommandFactoryHelper<PlayerPrefFloatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefstr", "prefstr(key,val) command", new StoryCommandFactoryHelper<PlayerPrefStringCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "wetesttouch", "wetesttouch(action,x,y) command, simulate touch event with WeTest", new StoryCommandFactoryHelper<WeTestTouchCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logcomps", "logcomps(root_name,[name1,name2,...],type,up_level,include_inactive) command", new StoryCommandFactoryHelper<LogComponentsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logscenepath", "logscenepath([prefixs],obj,up_level) command", new StoryCommandFactoryHelper<LogScenePathCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "clickui", "clickui(name1,name2,...) command", new StoryCommandFactoryHelper<ClickUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "toggleon", "toggleon(name1,name2,...) command", new StoryCommandFactoryHelper<ToggleOnCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "clickonptr", "clickonptr() command", new StoryCommandFactoryHelper<ClickOnPointerCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "toggleonptr", "toggleonptr() command", new StoryCommandFactoryHelper<ToggleOnPointerCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "clickonpos", "clickonpos(x,y) command", new StoryCommandFactoryHelper<ClickOnPosCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "toggleonpos", "toggleonpos(x,y) command", new StoryCommandFactoryHelper<ToggleOnPosCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "click", "click(uiobj) command", new StoryCommandFactoryHelper<ClickCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "toggle", "toggle(uiobj) command", new StoryCommandFactoryHelper<ToggleCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logcperfs", "logcperfs() command", new StoryCommandFactoryHelper<LogCompiledPerfsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "reloadperfs", "reloadperfs() command", new StoryCommandFactoryHelper<ReloadPerfsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "runperf", "runperf(perf_dsl_file) command", new StoryCommandFactoryHelper<RunPerfCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "compileperf", "compileperf(perf_dsl_file) command", new StoryCommandFactoryHelper<CompilePerfCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadui", "loadui(ui_name_dsl) command", new StoryCommandFactoryHelper<LoadUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "showui", "showui() command", new StoryCommandFactoryHelper<ShowUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "hideui", "hideui() command", new StoryCommandFactoryHelper<HideUiCommand>());

                //register value or function
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "mono", "mono() function, get total mono memory", new StoryFunctionFactoryHelper<GetMonoMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "native", "native() function, get used native memory", new StoryFunctionFactoryHelper<GetNativeMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gfx", "gfx() function, get used gfx memory", new StoryFunctionFactoryHelper<GetGfxMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "unused", "unused() function, get unused reserved native memory", new StoryFunctionFactoryHelper<GetUnusedMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "total", "total() function, get total native memory", new StoryFunctionFactoryHelper<GetTotalMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "deviceinfo", "deviceinfo() function, get device name/model and gpu model", new StoryFunctionFactoryHelper<DeviceInfoFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getclipboard", "getclipboard() function, get system clipboard content", new StoryFunctionFactoryHelper<GetClipboardFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefint", "prefint(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefIntFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "preffloat", "preffloat(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefFloatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefstr", "prefstr(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefStringFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "istexsupported", "istexsupported(fmt_str,usage_str) function", new StoryFunctionFactoryHelper<IsFormatSupportedFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcompatibleformat", "getcompatibleformat(fmt_str,usage_str) function", new StoryFunctionFactoryHelper<GetCompatibleFormatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getgraphicsformat", "getgraphicsformat(def_fmt) function", new StoryFunctionFactoryHelper<GetGraphicsFormatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getmsaasamplecount", "getmsaasamplecount(w,h,color_fmt,depth_bit,mip_ct) function", new StoryFunctionFactoryHelper<GetMSAASampleCountFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "sysinfo", "sysinfo() function, return typeof(SystemInfo)", new StoryFunctionFactoryHelper<SystemInfoFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "screen", "screen() function, return typeof(Screen)", new StoryFunctionFactoryHelper<ScreenFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "screenwidth", "screenwidth() function, return Screen.width", new StoryFunctionFactoryHelper<ScreenWidthFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "screenheight", "screenheight() function, return Screen.height", new StoryFunctionFactoryHelper<ScreenHeightFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "screendpi", "screendpi() function, return Screen.dpi", new StoryFunctionFactoryHelper<ScreenDPIFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "app", "app() function, return typeof(Application)", new StoryFunctionFactoryHelper<ApplicationFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "appid", "appid() function, return Application.identifier", new StoryFunctionFactoryHelper<AppIdFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "appname", "appname() function, return Application.productName", new StoryFunctionFactoryHelper<AppNameFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "platform", "platform() function, return Application.platform", new StoryFunctionFactoryHelper<PlatformFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "iseditor", "iseditor() function, return Application.isEditor", new StoryFunctionFactoryHelper<IsEditorFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isconsole", "isconsole() function, return Application.isConsolePlatform", new StoryFunctionFactoryHelper<IsConsoleFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "ismobile", "ismobile() function, return Application.isMobilePlatform", new StoryFunctionFactoryHelper<IsMobileFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isandroid", "isandroid() function, return Application.platform==Android", new StoryFunctionFactoryHelper<IsAndroidFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isiphone", "isiphone() function, return Application.platform==IPhone", new StoryFunctionFactoryHelper<IsIPhoneFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "ispc", "ispc() function, return not mobile and not console", new StoryFunctionFactoryHelper<IsPCFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "shell", "shell(cmd) function, return string", new StoryFunctionFactoryHelper<ShellFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "shelltimeout", "shelltimeout(cmd,ms) function, return string", new StoryFunctionFactoryHelper<ShellTimeoutFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gettaskcount", "gettaskcount() function, return int", new StoryFunctionFactoryHelper<GetTaskCountFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "wetestx", "wetestx() function, WeTest GetX", new StoryFunctionFactoryHelper<WeTestGetXFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "wetesty", "wetesty() function, WeTest GetY", new StoryFunctionFactoryHelper<WeTestGetYFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "wetestwidth", "wetestwidth() function, WeTest GetWidth", new StoryFunctionFactoryHelper<WeTestGetWidthFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "wetestheight", "wetestheight() function, WeTest GetHeight", new StoryFunctionFactoryHelper<WeTestGetHeightFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpointer", "getpointer() function, return Input.mousePosition", new StoryFunctionFactoryHelper<GetPointerFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getptruis", "getptruis() function, return List<UnityEngine.EventSystems.RaycastResult>", new StoryFunctionFactoryHelper<PointerRaycastUisFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getptrcomps", "getptrcomps(type,include_inactive) function, return List<Component>", new StoryFunctionFactoryHelper<GetPointerComponentsFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "raycastuis", "raycastuis(x,y) function, return List<UnityEngine.EventSystems.RaycastResult>", new StoryFunctionFactoryHelper<RaycastUisFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "raycastcomps", "raycastcomps(x,y,type,include_inactive) function, return List<Component>", new StoryFunctionFactoryHelper<RaycastComponentsFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscenepath", "getscenepath([prefixs],obj,up_level) function, return partial scene path", new StoryFunctionFactoryHelper<GetScenePathFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findrawimg", "findrawimg(name1,name2,...) function", new StoryFunctionFactoryHelper<FindRawImageFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "finduiimg", "finduiimg(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiImageFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findbutton", "findbutton(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiButtonFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtoggle", "findtoggle(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiToggleFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findslider", "findslider(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiSliderFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findinput", "findinput(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiInputFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtmpinput", "findtmpinput(name1,name2,...) function", new StoryFunctionFactoryHelper<FindTmpInputFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "finddropdown", "finddropdown(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiDropdownFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtmpdropdown", "findtmpdropdown(name1,name2,...) function", new StoryFunctionFactoryHelper<FindTmpDropdownFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findcomp", "findcomp(root_name,[name1,name2,...],type,include_inactive) function", new StoryFunctionFactoryHelper<FindComponentFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "searchcomps", "searchcomps(root_name,[name1,name2,...],type,include_inactive) function", new StoryFunctionFactoryHelper<SearchComponentsFunction>());

                //failback to call perf grade api
                StoryCommandManager.Instance.OnCreateFailback = this.OnCreateCommandFailback;
                StoryFunctionManager.Instance.OnCreateFailback = this.OnCreateFunctionFailback;
            }
            catch (Exception ex) {
                LogSystem.Error("exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void RegisterCommon()
        {
            //register command
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessage", "sendmessage(objname,msg,arg1,arg2,...) command", new StoryCommandFactoryHelper<GmCommands.SendMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithtag", "sendmessagewithtag(tagname,msg,arg1,arg2,...) command", new StoryCommandFactoryHelper<GmCommands.SendMessageWithTagCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithgameobject", "sendmessagewithgameobject(gameobject,msg,arg1,arg2,...) command", new StoryCommandFactoryHelper<GmCommands.SendMessageWithGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "creategameobject", "creategameobject(name,prefab[,parent])[obj(\"varname\")]{position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));loadtimeout(1000);disable(\"typename\", \"typename\", ...);remove(\"typename\", \"typename\", ...);} command", new StoryCommandFactoryHelper<GmCommands.CreateGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "settransform", "settransform(name,local0_or_world1){position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));} command", new StoryCommandFactoryHelper<GmCommands.SetTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "addtransform", "addtransform(name,local0_or_world1){position(vector3(x,y,z));rotation(vector3(x,y,z));scale(vector3(x,y,z));} command", new StoryCommandFactoryHelper<GmCommands.AddTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "destroygameobject", "destroygameobject(path) command", new StoryCommandFactoryHelper<GmCommands.DestroyGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setparent", "setparent(obj_or_path,parent,int_stay_world_pos) command", new StoryCommandFactoryHelper<GmCommands.SetParentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setactive", "setactive(obj_or_path,1_or_0) command", new StoryCommandFactoryHelper<GmCommands.SetActiveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setvisible", "setvisible(obj_or_path,1_or_0) command", new StoryCommandFactoryHelper<GmCommands.SetVisibleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "addcomponent", "addcomponent(obj_or_path,type_or_str)[obj(\"varname\")] command", new StoryCommandFactoryHelper<GmCommands.AddComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "removecomponent", "removecomponent(obj_or_path,type_or_str) command", new StoryCommandFactoryHelper<GmCommands.RemoveComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "openurl", "openurl(url) command", new StoryCommandFactoryHelper<GmCommands.OpenUrlCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "quit", "quit() command", new StoryCommandFactoryHelper<GmCommands.QuitCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimation", "gameobjectanimation(obj, anim[, normalized_time]) command", new StoryCommandFactoryHelper<GmCommands.GameObjectAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimationparam", "gameobjectanimationparam(obj){float(name,val);int(name,val);bool(name,val);trigger(name,val);} command", new StoryCommandFactoryHelper<GmCommands.GameObjectAnimationParamCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadscp", "loadscp(dsl_script_file) or loadscp(\"name\", func(param1, param2, ...)) or loadscp(name => func(param1, param2, ...)) command", new StoryCommandFactoryHelper<LoadScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "callscp", "callscp(\"func\", arg1, arg2, ...) or callscp(func(arg1, arg2, ...)) command", new StoryCommandFactoryHelper<CallScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "evalscp", "evalscp(code_str) or evalscp(exp1,exp2,...) command", new StoryCommandFactoryHelper<EvalScriptCommand>());

            //register value or function
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gettime", "gettime() function", new StoryFunctionFactoryHelper<GmCommands.GetTimeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gettimescale", "gettimescale() function", new StoryFunctionFactoryHelper<GmCommands.GetTimeScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isactive", "isactive(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.IsActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isreallyactive", "isreallyactive(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.IsReallyActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isvisible", "isvisible(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.IsVisibleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponent", "getcomponent(obj_or_path,type_or_str) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentinparent", "getcomponentinparent(obj_or_path,type_or_str[,int_inc_inactive]) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentinchildren", "getcomponentinchildren(obj_or_path,type_or_str[,int_inc_inactive]) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponents", "getcomponents(obj_or_path,type_or_str) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentsinparent", "getcomponentsinparent(obj_or_path,type_or_str[,int_inc_inactive]) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentsinchildren", "getcomponentsinchildren(obj_or_path,type_or_str[,int_inc_inactive]) function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getgameobject", "getgameobject(obj_or_path) or getgameobject(obj_or_path){disable(comp1,comp2,...);remove(comp1,comp2,...);} function", new StoryFunctionFactoryHelper<GmCommands.GetGameObjectFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getparent", "getparent(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.GetParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getchild", "getchild(obj_or_path,child_path) function", new StoryFunctionFactoryHelper<GmCommands.GetChildFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getunitytype", "getunitytype(type_str) function", new StoryFunctionFactoryHelper<GmCommands.GetUnityTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getunityuitype", "getunityuitype(type_str) function", new StoryFunctionFactoryHelper<GmCommands.GetUnityUiTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getusertype", "getusertype(type_str) function", new StoryFunctionFactoryHelper<GmCommands.GetUserTypeFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getposition", "getposition(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetPositionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositionx", "getpositionx(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetPositionXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositiony", "getpositiony(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetPositionYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositionz", "getpositionz(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetPositionZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotation", "getrotation(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetRotationFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationx", "getrotationx(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetRotationXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationy", "getrotationy(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetRotationYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationz", "getrotationz(obj_or_path[,local0_or_world1]) function", new StoryFunctionFactoryHelper<GmCommands.GetRotationZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscale", "getscale(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.GetScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscalex", "getscalex(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.GetScaleXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscaley", "getscaley(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.GetScaleYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscalez", "getscalez(obj_or_path) function", new StoryFunctionFactoryHelper<GmCommands.GetScaleZFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "position", "position(x,y,z) function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "rotation", "rotation(x,y,z) function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "scale", "scale(x,y,z) function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "deg2rad", "deg2rad(deg) function", new StoryFunctionFactoryHelper<GmCommands.Deg2RadFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "rad2deg", "rad2deg(rad) function", new StoryFunctionFactoryHelper<GmCommands.Rad2DegFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getfilename", "getfilename(path) function", new StoryFunctionFactoryHelper<GmCommands.GetFileNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getdirname", "getdirname(path) function", new StoryFunctionFactoryHelper<GmCommands.GetDirectoryNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getextension", "getextension(path) function", new StoryFunctionFactoryHelper<GmCommands.GetExtensionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "combinepath", "combinepath(path1,path2) function", new StoryFunctionFactoryHelper<GmCommands.CombinePathFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getstreamingassets", "getstreamingassets() function", new StoryFunctionFactoryHelper<GmCommands.GetStreamingAssetsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpersistentpath", "getpersistentpath() function", new StoryFunctionFactoryHelper<GmCommands.GetPersistentPathFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "callscp", "callscp(func_name,arg1,arg2,...) or callscp(func(arg1,arg2,...)) function", new StoryFunctionFactoryHelper<CallScriptFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "evalscp", "evalscp(str) or evalscp(exp1,exp2,...) function", new StoryFunctionFactoryHelper<EvalScriptFunction>());
        }
        private bool OnCreateCommandFailback(Dsl.ISyntaxComponent comp, out IStoryCommand expression)
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
                        var exp = new PerfApiCommand();
                        exp.SetApi(fn);
                        exp.Init(callData);
                        expression = exp;
                        ret = true;
                    }
                }
            }
            return ret;
        }
        private bool OnCreateFunctionFailback(Dsl.ISyntaxComponent comp, out IStoryFunction expression)
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
                        var exp = new PerfApiFunction();
                        exp.SetApi(fn);
                        exp.InitFromDsl(callData);
                        expression = exp;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public int ActiveStoryCount
        {
            get
            {
                return m_StoryLogicInfos.Count;
            }
        }
        public StrBoxedValueDict GlobalVariables
        {
            get { return m_GlobalVariables; }
        }
        public void ClearGlobalVariables()
        {
            m_GlobalVariables.Clear();
        }
        public void Reset()
        {
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (null != info) {
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
            m_StoryLogicInfos.Clear();
        }
        public void LoadStory(string file)
        {
            m_StoryInstancePool.Clear();
            m_ConfigManager.Clear();
            m_ConfigManager.LoadStory(file, 0, string.Empty);
        }
        public void LoadStoryText(byte[] bytes)
        {
            m_StoryInstancePool.Clear();
            m_ConfigManager.Clear();
            m_ConfigManager.LoadStoryText(string.Empty, bytes, 0, string.Empty);
        }
        public StoryInstance GetStory(string storyId)
        {
            return GetStoryInstance(storyId);
        }
        public void StartStory(string storyId)
        {
            StoryInstance inst = NewStoryInstance(storyId);
            if (null != inst) {
                StopStory(storyId);
                m_StoryLogicInfos.Add(inst);
                inst.Context = null;
                inst.GlobalVariables = m_GlobalVariables;
                inst.Start();

                LogSystem.Info("StartStory {0}", storyId);
            }
        }
        public void StopStory(string storyId)
        {
            int count = m_StoryLogicInfos.Count;
            for (int index = count - 1; index >= 0; --index) {
                StoryInstance info = m_StoryLogicInfos[index];
                if (info.StoryId == storyId) {
                    m_StoryLogicInfos.RemoveAt(index);
                }
            }
        }
        public void Tick()
        {
            long time = TimeUtility.GetLocalMilliseconds();
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.Tick(time);
                if (info.IsTerminated) {
                    m_StoryLogicInfos.RemoveAt(ix);
                }
            }
        }
        public void SendMessage(string msgId, BoxedValueList args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId, args);
            }
        }

        private StoryInstance NewStoryInstance(string storyId)
        {
            StoryInstance instance = GetStoryInstance(storyId);
            if (null == instance) {
                instance = m_ConfigManager.NewStoryInstance(storyId, 0);
                if (instance == null) {
                    LogSystem.Error("Can't load story config, story:{0} !", storyId);
                    return null;
                }

                AddStoryInstance(storyId, instance);
                return instance;
            } else {
                return instance;
            }
        }
        private void AddStoryInstance(string storyId, StoryInstance info)
        {
            if (!m_StoryInstancePool.ContainsKey(storyId)) {
                m_StoryInstancePool.Add(storyId, info);
            } else {
                m_StoryInstancePool[storyId] = info;
            }
        }
        private StoryInstance GetStoryInstance(string storyId)
        {
            StoryInstance info;
            m_StoryInstancePool.TryGetValue(storyId, out info);
            return info;
        }

        private ClientGmStorySystem() { }

        private StrBoxedValueDict m_GlobalVariables = new StrBoxedValueDict();

        private List<StoryInstance> m_StoryLogicInfos = new List<StoryInstance>();
        private Dictionary<string, StoryInstance> m_StoryInstancePool = new Dictionary<string, StoryInstance>();

        private StoryConfigManager m_ConfigManager = StoryConfigManager.NewInstance();

        public static ClientGmStorySystem Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static ClientGmStorySystem s_Instance = new ClientGmStorySystem();
    }

    internal class PerfApiCommand : SimpleStoryCommandBase<PerfApiCommand, StoryValueParams>
    {
        public void SetApi(string method)
        {
            m_Method = method;
            if (!PerfGradeGm.GetApi(method, out m_Api)) {
                LogSystem.Error("Can't find method '{0}'", method);
            }
        }
        protected override void CopyFields(PerfApiCommand other)
        {
            m_Method = other.m_Method;
            m_Api = other.m_Api;
        }
        protected override bool ExecCommand(StoryInstance instance, StoryValueParams _params, long delta)
        {
            var list = PerfGradeGm.NewArgList();
            foreach (var operand in _params.Values) {
                list.Add(operand);
            }
            try {
                if (null != m_Api) {
                    m_Api(list);
                }
            }
            finally {
                PerfGradeGm.RecycleArgList(list);
            }
            return false;
        }

        private string m_Method;
        private PerfGrade.PerfApiDelegation m_Api;
    }
    internal class PerfApiFunction : SimpleStoryFunctionBase<PerfApiFunction, StoryValueParams>
    {
        public void SetApi(string method)
        {
            m_Method = method;
            if (!PerfGradeGm.GetApi(method, out m_Api)) {
                LogSystem.Error("Can't find method '{0}'", method);
            }
        }
        protected override void CopyFields(PerfApiFunction other)
        {
            m_Method = other.m_Method;
            m_Api = other.m_Api;
        }
        protected override void UpdateValue(StoryInstance instance, StoryValueParams _params, StoryValueResult result)
        {
            var list = PerfGradeGm.NewArgList();
            foreach (var operand in _params.Values) {
                list.Add(operand);
            }
            try {
                BoxedValue r = BoxedValue.NullObject;
                if (null != m_Api) {
                    r = m_Api(list);
                }
                result.Value = r;
            }
            finally {
                PerfGradeGm.RecycleArgList(list);
            }
        }

        private string m_Method;
        private PerfGrade.PerfApiDelegation m_Api;
    }
}
