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

                //注册Gm命令
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setdebug", "setdebug command", new StoryCommandFactoryHelper<SetDebugCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "editorbreak", "editorbreak command", new StoryCommandFactoryHelper<EditorBreakCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "debugbreak", "debugbreak command", new StoryCommandFactoryHelper<DebugBreakCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportstex", "supportstex command", new StoryCommandFactoryHelper<SupportsTexCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsrt", "supportsrt command", new StoryCommandFactoryHelper<SupportsRTCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsblendingonrt", "supportsblendingonrt command", new StoryCommandFactoryHelper<SupportsBlendingOnRTCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "devicesupports", "devicesupports command", new StoryCommandFactoryHelper<DeviceSupportsCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "allocmemory", "allocmemory command", new StoryCommandFactoryHelper<AllocMemoryCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "freememory", "freememory command", new StoryCommandFactoryHelper<FreeMemoryCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "consumecpu", "consumecpu command", new StoryCommandFactoryHelper<ConsumeCpuCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "cmd", "cmd command", new StoryCommandFactoryHelper<CmdCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefint", "prefint command", new StoryCommandFactoryHelper<PlayerPrefIntCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "preffloat", "preffloat command", new StoryCommandFactoryHelper<PlayerPrefFloatCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "prefstr", "prefstr command", new StoryCommandFactoryHelper<PlayerPrefStringCommand>());

                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadui", "loadui command", new StoryCommandFactoryHelper<LoadUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "showui", "showui command", new StoryCommandFactoryHelper<ShowUiCommand>());
                StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "hideui", "hideui command", new StoryCommandFactoryHelper<HideUiCommand>());

                //注册值与函数处理
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefint", "prefint function", new StoryFunctionFactoryHelper<PlayerPrefIntFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "preffloat", "preffloat function", new StoryFunctionFactoryHelper<PlayerPrefFloatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "prefstr", "prefstr function", new StoryFunctionFactoryHelper<PlayerPrefStringFunction>());

                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "istexsupported", "istexsupported function", new StoryFunctionFactoryHelper<IsFormatSupportedFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcompatibleformat", "getcompatibleformat function", new StoryFunctionFactoryHelper<GetCompatibleFormatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getgraphicsformat", "getgraphicsformat function", new StoryFunctionFactoryHelper<GetGraphicsFormatFunction>());
                StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getmsaasamplecount", "getmsaasamplecount function", new StoryFunctionFactoryHelper<GetMSAASampleCountFunction>());

            }
            catch (Exception ex) {
                LogSystem.Error("exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void RegisterCommon()
        {
            //注册命令
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessage", "sendmessage command", new StoryCommandFactoryHelper<GmCommands.SendMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithtag", "sendmessagewithtag command", new StoryCommandFactoryHelper<GmCommands.SendMessageWithTagCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithgameobject", "sendmessagewithgameobject command", new StoryCommandFactoryHelper<GmCommands.SendMessageWithGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "creategameobject", "creategameobject command", new StoryCommandFactoryHelper<GmCommands.CreateGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "settransform", "settransform command", new StoryCommandFactoryHelper<GmCommands.SetTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "addtransform", "addtransform command", new StoryCommandFactoryHelper<GmCommands.AddTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "destroygameobject", "destroygameobject command", new StoryCommandFactoryHelper<GmCommands.DestroyGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setparent", "setparent command", new StoryCommandFactoryHelper<GmCommands.SetParentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setactive", "setactive command", new StoryCommandFactoryHelper<GmCommands.SetActiveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setvisible", "setvisible command", new StoryCommandFactoryHelper<GmCommands.SetVisibleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "addcomponent", "addcomponent command", new StoryCommandFactoryHelper<GmCommands.AddComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "removecomponent", "removecomponent command", new StoryCommandFactoryHelper<GmCommands.RemoveComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "openurl", "openurl command", new StoryCommandFactoryHelper<GmCommands.OpenUrlCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "quit", "quit command", new StoryCommandFactoryHelper<GmCommands.QuitCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimation", "gameobjectanimation command", new StoryCommandFactoryHelper<GmCommands.GameObjectAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimationparam", "gameobjectanimationparam command", new StoryCommandFactoryHelper<GmCommands.GameObjectAnimationParamCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadscp", "loadscp(dsl_script_file) or loadscp(\"name\", func(param1, param2, ...)) or loadscp(name => func(param1, param2, ...)) command", new StoryCommandFactoryHelper<LoadScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "callscp", "callscp(\"func\", arg1, arg2, ...) or callscp(func(arg1, arg2, ...)) command", new StoryCommandFactoryHelper<CallScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "evalscp", "evalscp(code_str) or evalscp(exp1,exp2,...) command", new StoryCommandFactoryHelper<EvalScriptCommand>());

            //注册值与函数处理
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gettime", "gettime function", new StoryFunctionFactoryHelper<GmCommands.GetTimeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "gettimescale", "gettimescale function", new StoryFunctionFactoryHelper<GmCommands.GetTimeScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isactive", "isactive function", new StoryFunctionFactoryHelper<GmCommands.IsActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isreallyactive", "isreallyactive function", new StoryFunctionFactoryHelper<GmCommands.IsReallyActiveFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "isvisible", "isvisible function", new StoryFunctionFactoryHelper<GmCommands.IsVisibleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponent", "getcomponent function", new StoryFunctionFactoryHelper<GmCommands.GetComponentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentinparent", "getcomponentinparent function", new StoryFunctionFactoryHelper<GmCommands.GetComponentInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentinchildren", "getcomponentinchildren function", new StoryFunctionFactoryHelper<GmCommands.GetComponentInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponents", "getcomponents function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentsinparent", "getcomponentsinparent function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsInParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getcomponentsinchildren", "getcomponentsinchildren function", new StoryFunctionFactoryHelper<GmCommands.GetComponentsInChildrenFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getgameobject", "getgameobject function", new StoryFunctionFactoryHelper<GmCommands.GetGameObjectFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getparent", "getparent function", new StoryFunctionFactoryHelper<GmCommands.GetParentFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getchild", "getchild function", new StoryFunctionFactoryHelper<GmCommands.GetChildFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getunitytype", "getunitytype function", new StoryFunctionFactoryHelper<GmCommands.GetUnityTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getunityuitype", "getunityuitype function", new StoryFunctionFactoryHelper<GmCommands.GetUnityUiTypeFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getusertype", "getusertype function", new StoryFunctionFactoryHelper<GmCommands.GetUserTypeFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getposition", "getposition function", new StoryFunctionFactoryHelper<GmCommands.GetPositionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositionx", "getpositionx function", new StoryFunctionFactoryHelper<GmCommands.GetPositionXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositiony", "getpositiony function", new StoryFunctionFactoryHelper<GmCommands.GetPositionYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpositionz", "getpositionz function", new StoryFunctionFactoryHelper<GmCommands.GetPositionZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotation", "getrotation function", new StoryFunctionFactoryHelper<GmCommands.GetRotationFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationx", "getrotationx function", new StoryFunctionFactoryHelper<GmCommands.GetRotationXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationy", "getrotationy function", new StoryFunctionFactoryHelper<GmCommands.GetRotationYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getrotationz", "getrotationz function", new StoryFunctionFactoryHelper<GmCommands.GetRotationZFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscale", "getscale function", new StoryFunctionFactoryHelper<GmCommands.GetScaleFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscalex", "getscalex function", new StoryFunctionFactoryHelper<GmCommands.GetScaleXFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscaley", "getscaley function", new StoryFunctionFactoryHelper<GmCommands.GetScaleYFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getscalez", "getscalez function", new StoryFunctionFactoryHelper<GmCommands.GetScaleZFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "position", "position function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "rotation", "rotation function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "scale", "scale function", new StoryFunctionFactoryHelper<StoryScript.CommonFunctions.Vector3Function>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "deg2rad", "deg2rad function", new StoryFunctionFactoryHelper<GmCommands.Deg2RadFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "rad2deg", "rad2deg function", new StoryFunctionFactoryHelper<GmCommands.Rad2DegValue>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getfilename", "getfilename function", new StoryFunctionFactoryHelper<GmCommands.GetFileNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getdirname", "getdirname function", new StoryFunctionFactoryHelper<GmCommands.GetDirectoryNameFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getextension", "getextension function", new StoryFunctionFactoryHelper<GmCommands.GetExtensionFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "combinepath", "combinepath function", new StoryFunctionFactoryHelper<GmCommands.CombinePathFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getstreamingassets", "getstreamingassets function", new StoryFunctionFactoryHelper<GmCommands.GetStreamingAssetsFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "getpersistentpath", "getpersistentpath function", new StoryFunctionFactoryHelper<GmCommands.GetPersistentPathFunction>());

            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "callscp", "callscp(func_name,arg1,arg2,...) or callscp(func(arg1,arg2,...)) function", new StoryFunctionFactoryHelper<CallScriptFunction>());
            StoryFunctionManager.Instance.RegisterFunctionFactory(StoryFunctionGroupDefine.GM, "evalscp", "evalscp(str) or evalscp(exp1,exp2,...) function", new StoryFunctionFactoryHelper<EvalScriptFunction>());
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
        public void Reset()
        {
            m_GlobalVariables.Clear();
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
}
