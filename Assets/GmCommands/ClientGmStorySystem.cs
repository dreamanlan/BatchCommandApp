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
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logcodenum", "logcodenum() command", new StoryCommandFactoryHelper<LogCodeNumCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "help", "help() command", new StoryCommandFactoryHelper<HelpCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setdebug", "setdebug(1_or_0) command", new StoryCommandFactoryHelper<SetDebugCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "startactivity", "startactivity(package_name[[,class_name[,flags]],extra_list_or_dict]) command", new StoryCommandFactoryHelper<StartActivityCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "finishactivity", "finishactivity() command", new StoryCommandFactoryHelper<FinishActivityCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "listenclipboard", "listenclipboard(interval) command", new StoryCommandFactoryHelper<ListenClipboardCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "listenandroid", "listenandroid() command", new StoryCommandFactoryHelper<ListenAndroidCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "startservice", "startservice(srv_class, extra_name, extra_val) command", new StoryCommandFactoryHelper<StartServiceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "stopservice", "stopservice(srv_class) command", new StoryCommandFactoryHelper<StopServiceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "shell", "shell(cmd) command", new StoryCommandFactoryHelper<ShellCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "shelltimeout", "shelltimeout(cmd,ms) command", new StoryCommandFactoryHelper<ShellTimeoutCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "cleanupcompletedtasks", "cleanupcompletedtasks() command", new StoryCommandFactoryHelper<CleanupCompletedTasksCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "usejavatask", "usejavatask(is_java) command", new StoryCommandFactoryHelper<UseJavaTaskCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setclipboard", "setclipboard(text) command", new StoryCommandFactoryHelper<SetClipboardCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "editorbreak", "editorbreak() command", new StoryCommandFactoryHelper<EditorBreakCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "debugbreak", "debugbreak() command", new StoryCommandFactoryHelper<DebugBreakCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "settimescale", "settimescale(scale) command", new StoryCommandFactoryHelper<SetTimeScaleCommand>());
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
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "startprofiler", "startprofiler() command", new StoryCommandFactoryHelper<StartProfilerCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "stopprofiler", "stopprofiler() command", new StoryCommandFactoryHelper<StopProfilerCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logprofiler", "logprofiler() command", new StoryCommandFactoryHelper<LogProfilerCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "cmd", "cmd(str) command", new StoryCommandFactoryHelper<CmdCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gm", "gm(str) command", new StoryCommandFactoryHelper<GmCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefint", "prefint(key,val) command", new StoryCommandFactoryHelper<PlayerPrefIntCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "preffloat", "preffloat(key,val) command", new StoryCommandFactoryHelper<PlayerPrefFloatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefstr", "prefstr(key,val) command", new StoryCommandFactoryHelper<PlayerPrefStringCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefdel", "prefdel(key) command", new StoryCommandFactoryHelper<PlayerPrefDeleteCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefdelall", "prefdelall() command", new StoryCommandFactoryHelper<PlayerPrefDeleteAllCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefbyjava", "prefbyjava(key,val) command", new StoryCommandFactoryHelper<PrefByJavaCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "wetesttouch", "wetesttouch(action,x,y) command, simulate touch event with WeTest", new StoryCommandFactoryHelper<WeTestTouchCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logmesh", "logmesh(mesh_or_path) command", new StoryCommandFactoryHelper<LogMeshCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setmat", "setmat(renderer_or_path,mat_or_path) or setmat(renderer_or_path,ix,mat_or_path) or setmat(renderer_or_path,ix,mats_or_path,ix) command", new StoryCommandFactoryHelper<SetMaterialCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setsmat", "setsmat(renderer_or_path,mat_or_path) or setsmat(renderer_or_path,ix,mat_or_path) or setsmat(renderer_or_path,ix,mats_or_path,ix) command", new StoryCommandFactoryHelper<SetSharedMaterialCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setmats", "setmats(mesh_or_path,mats_or_path) command", new StoryCommandFactoryHelper<SetMaterialsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setsmats", "setsmats(mesh_or_path,mats_or_path) command", new StoryCommandFactoryHelper<SetSharedMaterialsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "matsetfloat", "matsetfloat(mat_or_path,key,val) command, Material.SetFloat", new StoryCommandFactoryHelper<MaterialSetFloatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "matsetint", "matsetint(mat_or_path,key,val) command, Material.SetInt", new StoryCommandFactoryHelper<MaterialSetIntCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "matsetinteger", "matsetinteger(mat_or_path,key,val) command, Material.SetInteger", new StoryCommandFactoryHelper<MaterialSetIntegerCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "matsetvector", "matsetvector(mat_or_path,key,val) command, Material.SetVector", new StoryCommandFactoryHelper<MaterialSetVectorCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "matsetcolor", "matsetcolor(mat_or_path,key,val) command, Material.SetColor", new StoryCommandFactoryHelper<MaterialSetColorCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logobjs", "logobjs(name_key,type) command", new StoryCommandFactoryHelper<LogObjectsCommand>());
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

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "logcstartups", "logcstartups() command", new StoryCommandFactoryHelper<LogCompiledStartupsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "reloadstartups", "reloadstartups() command", new StoryCommandFactoryHelper<ReloadStartupsCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "runstartup", "runstartup(startup_dsl_file) command", new StoryCommandFactoryHelper<RunStartupCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "compilestartup", "compilestartup(startup_dsl_file) command", new StoryCommandFactoryHelper<CompileStartupCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadui", "loadui(ui_name_dsl) command", new StoryCommandFactoryHelper<LoadUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "showui", "showui() command", new StoryCommandFactoryHelper<ShowUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "hideui", "hideui() command", new StoryCommandFactoryHelper<HideUiCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "import", "import(ns,assembly) command", new StoryCommandFactoryHelper<ImportNamespaceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "unimport", "unimport(ns,assembly) command", new StoryCommandFactoryHelper<UnImportNamespaceCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "copypdf", "copypdf(file,page_start,page_count) command, copy pdf to clipboard", new StoryCommandFactoryHelper<StoryApi.CopyPdfCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "showmemory", "showmemory() command", new StoryCommandFactoryHelper<StoryApi.ShowMemoryCommand>());

                //register value or function
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isdebug", "isdebug() function", new StoryFunctionFactoryHelper<IsDebugFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isdev", "isdev() function", new StoryFunctionFactoryHelper<IsDevelopmentFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "typeof", "typeof(type) or typeof(type,assembly) function", new StoryFunctionFactoryHelper<TypeOfFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "mono", "mono() function, get total mono memory", new StoryFunctionFactoryHelper<GetMonoMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "native", "native() function, get used native memory", new StoryFunctionFactoryHelper<GetNativeMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gfx", "gfx() function, get used gfx memory", new StoryFunctionFactoryHelper<GetGfxMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "unused", "unused() function, get unused reserved native memory", new StoryFunctionFactoryHelper<GetUnusedMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "total", "total() function, get total native memory", new StoryFunctionFactoryHelper<GetTotalMemoryFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "deviceinfo", "deviceinfo() function, get device name/model and gpu model", new StoryFunctionFactoryHelper<DeviceInfoFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getclipboard", "getclipboard() function, get system clipboard content", new StoryFunctionFactoryHelper<GetClipboardFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getbool", "getbool(str) function", new StoryFunctionFactoryHelper<GetBoolFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getboolarray", "getboolarray(str) function", new StoryFunctionFactoryHelper<GetBoolArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getstring", "getstring(str) function", new StoryFunctionFactoryHelper<GetStringFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getstringarray", "getstringarray(str) function", new StoryFunctionFactoryHelper<GetStringArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getchar", "getchar(str) function", new StoryFunctionFactoryHelper<GetCharFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getchararray", "getchararray(str) function", new StoryFunctionFactoryHelper<GetCharArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getbyte", "getbyte(str) function", new StoryFunctionFactoryHelper<GetByteFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getbytearray", "getbytearray(str) function", new StoryFunctionFactoryHelper<GetByteArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getshort", "getshort(str) function", new StoryFunctionFactoryHelper<GetShortFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getshortarray", "getshortarray(str) function", new StoryFunctionFactoryHelper<GetShortArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getint", "getint(str) function", new StoryFunctionFactoryHelper<GetIntFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getintarray", "getintarray(str) function", new StoryFunctionFactoryHelper<GetIntArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getlong", "getlong(str) function", new StoryFunctionFactoryHelper<GetLongFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getlongarray", "getlongarray(str) function", new StoryFunctionFactoryHelper<GetLongArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getfloat", "getfloat(str) function", new StoryFunctionFactoryHelper<GetFloatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getfloatarray", "getfloatarray(str) function", new StoryFunctionFactoryHelper<GetFloatArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getdouble", "getdouble(str) function", new StoryFunctionFactoryHelper<GetDoubleFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getdoublearray", "getdoublearray(str) function", new StoryFunctionFactoryHelper<GetDoubleArrayFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getactivityclass", "getactivityclass() function", new StoryFunctionFactoryHelper<GetActivityClassNameFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefint", "prefint(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefIntFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "preffloat", "preffloat(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefFloatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefstr", "prefstr(key,defval) function", new StoryFunctionFactoryHelper<PlayerPrefStringFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefbyjava", "prefbyjava(key,defval) function", new StoryFunctionFactoryHelper<PrefByJavaFunction>());

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
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isjavatask", "isjavatask() function, return int", new StoryFunctionFactoryHelper<IsJavaTaskFunction>());
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

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "shader", "shader() function, return typeof(Shader)", new StoryFunctionFactoryHelper<ShaderFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getmat", "getmat(mat_or_path[,index]) function, renderer.material[s[ix]]", new StoryFunctionFactoryHelper<GetMaterialFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getsmat", "getsmat(mat_or_path[,index]) function, renderer.sharedMaterial[s[ix]]", new StoryFunctionFactoryHelper<GetSharedMaterialFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "matgetfloat", "matgetfloat(mat_or_path,key) function, Material.GetFloat", new StoryFunctionFactoryHelper<MaterialGetFloatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "matgetint", "matgetint(mat_or_path,key) function, Material.GetInt", new StoryFunctionFactoryHelper<MaterialGetIntFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "matgetinteger", "matgetinteger(mat_or_path,key) function, Material.GetInteger", new StoryFunctionFactoryHelper<MaterialGetIntegerFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "matgetvector", "matgetvector(mat_or_path,key) function, Material.GetVector", new StoryFunctionFactoryHelper<MaterialGetVectorFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "matgetcolor", "matgetcolor(mat_or_path,key) function, Material.GetColor", new StoryFunctionFactoryHelper<MaterialGetColorFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findrawimg", "findrawimg(name1,name2,...) function", new StoryFunctionFactoryHelper<FindRawImageFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "finduiimg", "finduiimg(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiImageFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findbutton", "findbutton(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiButtonFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtoggle", "findtoggle(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiToggleFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findslider", "findslider(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiSliderFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findinput", "findinput(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiInputFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtmpinput", "findtmpinput(name1,name2,...) function", new StoryFunctionFactoryHelper<FindTmpInputFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "finddropdown", "finddropdown(name1,name2,...) function", new StoryFunctionFactoryHelper<FindUiDropdownFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findtmpdropdown", "findtmpdropdown(name1,name2,...) function", new StoryFunctionFactoryHelper<FindTmpDropdownFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findobj", "findobj(name_key,type) function", new StoryFunctionFactoryHelper<FindObjectFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "searchobjs", "searchobjs(name_key,type) function", new StoryFunctionFactoryHelper<SearchObjectsFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "findcomp", "findcomp(root_name,[name1,name2,...],type,include_inactive) function", new StoryFunctionFactoryHelper<FindComponentFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "searchcomps", "searchcomps(root_name,[name1,name2,...],type,include_inactive) function", new StoryFunctionFactoryHelper<SearchComponentsFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "jc", "jc(jclass) or jc(str) function", new StoryFunctionFactoryHelper<StoryApi.JavaClassFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "jo", "jo(jobj) or jo(str,arg1,arg2,...) function", new StoryFunctionFactoryHelper<StoryApi.JavaObjectFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "jp", "jp(class_str,scp_method) function", new StoryFunctionFactoryHelper<StoryApi.JavaProxyFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "oc", "oc(str) function", new StoryFunctionFactoryHelper<StoryApi.ObjectcClassFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "oo", "oo(objId) function", new StoryFunctionFactoryHelper<StoryApi.ObjectcObjectFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpss", "getpss() function", new StoryFunctionFactoryHelper<StoryApi.GetPssFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getvss", "getvss() function", new StoryFunctionFactoryHelper<StoryApi.GetVssFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getnative", "getnative() function", new StoryFunctionFactoryHelper<StoryApi.GetNativeFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getgraphics", "getgraphics() function", new StoryFunctionFactoryHelper<StoryApi.GetGraphicsFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getunknown", "getunknown() function", new StoryFunctionFactoryHelper<StoryApi.GetUnknownFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getjava", "getjava() function", new StoryFunctionFactoryHelper<StoryApi.GetJavaFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcode", "getcode() function", new StoryFunctionFactoryHelper<StoryApi.GetCodeFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getstack", "getstack() function", new StoryFunctionFactoryHelper<StoryApi.GetStackFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getsystem", "getsystem() function", new StoryFunctionFactoryHelper<StoryApi.GetSystemFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getactivity", "getactivity() function", new StoryFunctionFactoryHelper<StoryApi.GetActivityFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getintent", "getintent() function", new StoryFunctionFactoryHelper<StoryApi.GetIntentFunction>());

                //failback to call startup api
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
                //all startup apis are in the form of function calls.
                if (funcData.HaveParam()) {
                    var callData = funcData;
                    string fn = callData.GetId();
                    if (StartupScript.ExistsApi(fn)) {
                        var exp = new StartupApiCommand();
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
                //all startup apis are in the form of function calls.
                if (funcData.HaveParam()) {
                    var callData = funcData;
                    string fn = callData.GetId();
                    if (StartupScript.ExistsApi(fn)) {
                        var exp = new StartupApiFunction();
                        exp.SetApi(fn);
                        exp.InitFromDsl(callData);
                        expression = exp;
                        ret = true;
                    }
                }
            }
            else {
                var valData = comp as Dsl.ValueData;
                if (null != valData && valData.IsId()) {
                    var typeName = valData.GetId();
                    var type = StoryScriptUtility.GetType(typeName);
                    if (null != type) {
                        var exp = new TypeHolderFunction();
                        exp.SetType(type);
                        exp.InitFromDsl(valData);
                        expression = exp;
                        ret = true;
                    }
                    else if (StoryScriptUtility.IsNamespace(typeName, out var id)) {
                        var exp = new NamespaceHolderFunction();
                        exp.SetNamespace(typeName);
                        exp.InitFromDsl(valData);
                        expression = exp;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public int ActiveStoryCount
        {
            get {
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
        public void SendMessage(string msgId)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId);
            }
        }
        public void SendMessage(string msgId, BoxedValue arg1)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId, arg1);
            }
        }
        public void SendMessage(string msgId, BoxedValue arg1, BoxedValue arg2)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId, arg1, arg2);
            }
        }
        public void SendMessage(string msgId, BoxedValue arg1, BoxedValue arg2, BoxedValue arg3)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                info.SendMessage(msgId, arg1, arg2, arg3);
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
            }
            else {
                return instance;
            }
        }
        private void AddStoryInstance(string storyId, StoryInstance info)
        {
            if (!m_StoryInstancePool.ContainsKey(storyId)) {
                m_StoryInstancePool.Add(storyId, info);
            }
            else {
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
            get {
                return s_Instance;
            }
        }
        private static ClientGmStorySystem s_Instance = new ClientGmStorySystem();
    }

    internal class StartupApiCommand : SimpleStoryCommandBase<StartupApiCommand, StoryFunctionParams>
    {
        public void SetApi(string method)
        {
            m_Method = method;
            if (!StartupScript.GetApi(method, out m_Api)) {
                LogSystem.Error("Can't find method '{0}'", method);
            }
        }
        protected override void CopyFields(StartupApiCommand other)
        {
            m_Method = other.m_Method;
            m_Api = other.m_Api;
        }
        protected override bool ExecCommand(StoryInstance instance, StoryFunctionParams _params, long delta)
        {
            var list = StartupScript.NewArgList();
            foreach (var operand in _params.Values) {
                list.Add(operand);
            }
            try {
                if (null != m_Api) {
                    m_Api(list);
                }
            }
            finally {
                StartupScript.RecycleArgList(list);
            }
            return false;
        }

        private string m_Method;
        private StartupApi.ApiDelegation m_Api;
    }
    internal class StartupApiFunction : SimpleStoryFunctionBase<StartupApiFunction, StoryFunctionParams>
    {
        public void SetApi(string method)
        {
            m_Method = method;
            if (!StartupScript.GetApi(method, out m_Api)) {
                LogSystem.Error("Can't find method '{0}'", method);
            }
        }
        protected override void CopyFields(StartupApiFunction other)
        {
            m_Method = other.m_Method;
            m_Api = other.m_Api;
        }
        protected override void UpdateValue(StoryInstance instance, StoryFunctionParams _params, StoryFunctionResult result)
        {
            var list = StartupScript.NewArgList();
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
                StartupScript.RecycleArgList(list);
            }
        }

        private string m_Method;
        private StartupApi.ApiDelegation m_Api;
    }
    internal class TypeHolderFunction : SimpleStoryFunctionBase<TypeHolderFunction, StoryFunctionParam>
    {
        public void SetType(Type type)
        {
            m_Type = type;
        }
        protected override void CopyFields(TypeHolderFunction other)
        {
            m_Type = other.m_Type;
        }
        protected override void UpdateValue(StoryInstance instance, StoryFunctionParam _params, StoryFunctionResult result)
        {
            result.Value = m_Type;
        }

        private Type m_Type;
    }
    internal class NamespaceObject : StoryScript.IObjectDispatch
    {
        public NamespaceObject(string ns)
        {
            m_Namespace = ns;
        }
        public int GetDispatchId(string name)
        {
            if (StoryScriptUtility.IsNamespace(name, out int id))
                return id;
            return -1;
        }
        public void SetProperty(int dispId, BoxedValue val)
        {
            throw new NotImplementedException();
        }
        public BoxedValue GetProperty(int dispId)
        {
            string name = StoryScriptUtility.GetNamespace(dispId);
            if (string.IsNullOrEmpty(name)) {
                return BoxedValue.NullObject;
            }
            else {
                string newName = m_Namespace + "." + name;
                var type = StoryScriptUtility.GetType(newName);
                if (null != type) {
                    return type;
                }
                else {
                    return BoxedValue.FromObject(new NamespaceObject(newName));
                }
            }
        }
        public BoxedValue InvokeMethod(int dispId, List<BoxedValue> args)
        {
            throw new NotImplementedException();
        }

        private string m_Namespace;
    }
    internal class NamespaceHolderFunction : SimpleStoryFunctionBase<NamespaceHolderFunction, StoryFunctionParam>
    {
        public void SetNamespace(string ns)
        {
            m_NamespaceObj = BoxedValue.FromObject(new NamespaceObject(ns));
        }
        protected override void CopyFields(NamespaceHolderFunction other)
        {
            m_NamespaceObj = other.m_NamespaceObj;
        }
        protected override void UpdateValue(StoryInstance instance, StoryFunctionParam _params, StoryFunctionResult result)
        {
            result.Value = m_NamespaceObj;
        }

        private BoxedValue m_NamespaceObj;
    }
}
