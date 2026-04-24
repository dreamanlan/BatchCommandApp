using System;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;

namespace GmCommands
{
    public delegate void RegisterGmCommandsAndFunctionsDelegation(DslCalculatorApiRegistry registry);
    public sealed class ClientGmStorySystem
    {
        public RegisterGmCommandsAndFunctionsDelegation OnRegisterGmCommandsAndFunctions;
        public void Init()
        {
            try {
                var registry = DslCalculatorHost.GetSharedApiRegistry();
                GmExpressionRegistrar.RegisterGmExpressions(registry);

                if (null != OnRegisterGmCommandsAndFunctions) {
                    OnRegisterGmCommandsAndFunctions(registry);
                }

                //failback to call startup api
                StoryInstance.OnLoadFailback = this.OnLoadFailback;
            }
            catch (Exception ex) {
                LogSystem.Error("exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private bool OnLoadFailback(Dsl.ISyntaxComponent comp, DslCalculator calculator, out IExpression expression)
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
                        var exp = new StartupApiExp(fn);
                        if (exp.Load(funcData, calculator)) {
                            expression = exp;
                            ret = true;
                        }
                        expression = exp;
                        ret = true;
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
                            exp.Load(valData, calculator);
                            expression = exp;
                            ret = true;
                        }
                        else if (StoryScriptUtility.IsNamespace(typeName, out var id)) {
                            var exp = new NamespaceHolderFunction();
                            exp.SetNamespace(typeName);
                            exp.Load(valData, calculator);
                            expression = exp;
                            ret = true;
                        }
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
        public StrBoxedValueDict ContextVariables
        {
            get { return m_ContextVariables; }
        }
        public void ClearContextVariables()
        {
            m_ContextVariables.Clear();
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
                inst.ContextVariables = m_ContextVariables;
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
        public BoxedValueList NewBoxedValueList()
        {
            var args = m_BoxedValueListPool.Alloc();
            args.Clear();
            return args;
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
        public void SendMessage(string msgId, BoxedValueList args)
        {
            int ct = m_StoryLogicInfos.Count;
            for (int ix = ct - 1; ix >= 0; --ix) {
                StoryInstance info = m_StoryLogicInfos[ix];
                var newArgs = info.NewBoxedValueList();
                newArgs.AddRange(args);
                info.SendMessage(msgId, newArgs);
            }
            m_BoxedValueListPool.Recycle(args);
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

        private SimpleObjectPool<BoxedValueList> m_BoxedValueListPool = new SimpleObjectPool<BoxedValueList>();
        private StrBoxedValueDict m_ContextVariables = new StrBoxedValueDict();

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

    internal class TypeHolderFunction : SimpleExpressionBase
    {
        public void SetType(Type type)
        {
            m_Type = type;
        }
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(m_Type);
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
    internal class NamespaceHolderFunction : SimpleExpressionBase
    {
        public void SetNamespace(string ns)
        {
            m_NamespaceObj = BoxedValue.FromObject(new NamespaceObject(ns));
        }
        protected override BoxedValue OnCalc(IList<BoxedValue> operands)
        {
            return BoxedValue.FromObject(m_NamespaceObj);
        }

        private BoxedValue m_NamespaceObj;
    }
}
