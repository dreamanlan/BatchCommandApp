using System;
using System.Collections.Generic;
using StoryScript;

namespace GmCommands
{
    /// <summary>
    /// Gm剧情系统是在游戏剧情系统之上添加GM命令构成的特殊剧情系统。游戏剧情系统添加的命令与值都可以在Gm剧情脚本里使用（反之亦然）
    /// </summary>
    /// <remarks>
    /// 1、在剧情系统中注册的命令与值是共享的，亦即Gm剧情系统注册的Gm命令与值在正常剧情脚本里也可以使用！
    /// （在发布时此系统应该从客户端移除。）
    /// 2、剧情脚本与Gm剧情脚本不是一套体系，互不相干。
    /// </remarks>
    public sealed class ClientGmStorySystem
    {
        public void Init()
        {
            StoryCommandManager.ThreadCommandGroupsMask = 1 << (int)StoryCommandGroupDefine.GM;
            StoryValueManager.ThreadValueGroupsMask = 1 << (int)StoryValueGroupDefine.GM;

            RegisterCommon();

            //注册Gm命令
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setdebug", new StoryCommandFactoryHelper<SetDebugCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "editorbreak", new StoryCommandFactoryHelper<EditorBreakCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "debugbreak", new StoryCommandFactoryHelper<DebugBreakCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportstex", new StoryCommandFactoryHelper<SupportsTexCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsrt", new StoryCommandFactoryHelper<SupportsRTCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "supportsblendingonrt", new StoryCommandFactoryHelper<SupportsBlendingOnRTCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "devicesupports", new StoryCommandFactoryHelper<DeviceSupportsCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "allocmemory", new StoryCommandFactoryHelper<AllocMemoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "freememory", new StoryCommandFactoryHelper<FreeMemoryCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "consumecpu", new StoryCommandFactoryHelper<ConsumeCpuCommand>());

            //注册值与函数处理
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "istexsupported", new StoryValueFactoryHelper<IsFormatSupportedValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcompatibleformat", new StoryValueFactoryHelper<GetCompatibleFormatValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getgraphicsformat", new StoryValueFactoryHelper<GetGraphicsFormatValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getmsaasamplecount", new StoryValueFactoryHelper<GetMSAASampleCountValue>());

        }

        private void RegisterCommon()
        {
            //注册命令
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessage", new StoryCommandFactoryHelper<SendMessageCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithtag", new StoryCommandFactoryHelper<SendMessageWithTagCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "sendmessagewithgameobject", new StoryCommandFactoryHelper<SendMessageWithGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "creategameobject", new StoryCommandFactoryHelper<CreateGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "settransform", new StoryCommandFactoryHelper<SetTransformCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "destroygameobject", new StoryCommandFactoryHelper<DestroyGameObjectCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setparent", new StoryCommandFactoryHelper<SetParentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setactive", new StoryCommandFactoryHelper<SetActiveCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "setvisible", new StoryCommandFactoryHelper<SetVisibleCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "addcomponent", new StoryCommandFactoryHelper<AddComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "removecomponent", new StoryCommandFactoryHelper<RemoveComponentCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "openurl", new StoryCommandFactoryHelper<OpenUrlCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "quit", new StoryCommandFactoryHelper<QuitCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimation", new StoryCommandFactoryHelper<GameObjectAnimationCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "gameobjectanimationparam", new StoryCommandFactoryHelper<GameObjectAnimationParamCommand>());

            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "loadscp", new StoryCommandFactoryHelper<LoadScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "callscp", new StoryCommandFactoryHelper<CallScriptCommand>());
            StoryCommandManager.Instance.RegisterCommandFactory(StoryCommandGroupDefine.GM, "evalscp", new StoryCommandFactoryHelper<EvalScriptCommand>());

            //注册值与函数处理
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "gettime", new StoryValueFactoryHelper<GetTimeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "gettimescale", new StoryValueFactoryHelper<GetTimeScaleValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "isactive", new StoryValueFactoryHelper<IsActiveValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "isreallyactive", new StoryValueFactoryHelper<IsReallyActiveValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "isvisible", new StoryValueFactoryHelper<IsVisibleValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponent", new StoryValueFactoryHelper<GetComponentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponentinparent", new StoryValueFactoryHelper<GetComponentInParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponentinchildren", new StoryValueFactoryHelper<GetComponentInChildrenValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponents", new StoryValueFactoryHelper<GetComponentsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponentsinparent", new StoryValueFactoryHelper<GetComponentsInParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getcomponentsinchildren", new StoryValueFactoryHelper<GetComponentsInChildrenValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getgameobject", new StoryValueFactoryHelper<GetGameObjectValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getparent", new StoryValueFactoryHelper<GetParentValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getchild", new StoryValueFactoryHelper<GetChildValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getunitytype", new StoryValueFactoryHelper<GetUnityTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getunityuitype", new StoryValueFactoryHelper<GetUnityUiTypeValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getusertype", new StoryValueFactoryHelper<GetUserTypeValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "deg2rad", new StoryValueFactoryHelper<Deg2RadValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "rad2deg", new StoryValueFactoryHelper<Rad2DegValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getfilename", new StoryValueFactoryHelper<GetFileNameValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getdirname", new StoryValueFactoryHelper<GetDirectoryNameValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getextension", new StoryValueFactoryHelper<GetExtensionValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "combinepath", new StoryValueFactoryHelper<CombinePathValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getstreamingassets", new StoryValueFactoryHelper<GetStreamingAssetsValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "getpersistentpath", new StoryValueFactoryHelper<GetPersistentPathValue>());

            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "callscp", new StoryValueFactoryHelper<CallScriptValue>());
            StoryValueManager.Instance.RegisterValueFactory(StoryValueGroupDefine.GM, "evalscp", new StoryValueFactoryHelper<EvalScriptValue>());
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
