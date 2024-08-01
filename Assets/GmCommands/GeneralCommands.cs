using System;
using System.Collections;
using System.Collections.Generic;
using Dsl;
using StoryScript;

namespace GmCommands
{
    /// <summary>
    /// sendmessage(objname,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageCommand cmd = new SendMessageCommand();
            cmd.m_ObjName = m_ObjName.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjName.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string objname = m_ObjName.Value;
            string msg = m_Msg.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                StoryScriptUtility.SendMessage(objname, msg, null);
            else if (args.Length == 1)
                StoryScriptUtility.SendMessage(objname, msg, args[0]);
            else
                StoryScriptUtility.SendMessage(objname, msg, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjName.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction<string> m_ObjName = new StoryValue<string>();
        private IStoryFunction<string> m_Msg = new StoryValue<string>();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
    /// <summary>
    /// sendmessagewithtag(tagname,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageWithTagCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageWithTagCommand cmd = new SendMessageWithTagCommand();
            cmd.m_ObjTag = m_ObjTag.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjTag.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string objtag = m_ObjTag.Value;
            string msg = m_Msg.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            if (args.Length == 0)
                StoryScriptUtility.SendMessageWithTag(objtag, msg, null);
            else if (args.Length == 1)
                StoryScriptUtility.SendMessageWithTag(objtag, msg, args[0]);
            else
                StoryScriptUtility.SendMessageWithTag(objtag, msg, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjTag.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction<string> m_ObjTag = new StoryValue<string>();
        private IStoryFunction<string> m_Msg = new StoryValue<string>();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
    /// <summary>
    /// sendmessagewithgameobject(gameobject,msg,arg1,arg2,...);
    /// </summary>
    internal class SendMessageWithGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SendMessageWithGameObjectCommand cmd = new SendMessageWithGameObjectCommand();
            cmd.m_Object = m_Object.Clone();
            cmd.m_Msg = m_Msg.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Object.Evaluate(instance, handler, iterator, args);
            m_Msg.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_Object.Value;
            UnityEngine.GameObject uobj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == uobj) {
                try {
                    int objId = objVal.IsInteger ? objVal.GetInt() : -1;
                    uobj = null;
                } catch {
                    uobj = null;
                }
            }
            if (null != uobj) {
                string msg = m_Msg.Value;
                ArrayList arglist = new ArrayList();
                for (int i = 0; i < m_Args.Count; ++i) {
                    IStoryFunction val = m_Args[i];
                    arglist.Add(val.Value.GetObject());
                }
                object[] args = arglist.ToArray();
                if (args.Length == 0)
                    uobj.SendMessage(msg, UnityEngine.SendMessageOptions.DontRequireReceiver);
                else if (args.Length == 1)
                    uobj.SendMessage(msg, args[0], UnityEngine.SendMessageOptions.DontRequireReceiver);
                else
                    uobj.SendMessage(msg, args, UnityEngine.SendMessageOptions.DontRequireReceiver);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Object.InitFromDsl(callData.GetParam(0));
                m_Msg.InitFromDsl(callData.GetParam(1));
            }
            for (int i = 2; i < callData.GetParamNum(); ++i) {
                StoryValue val = new StoryValue();
                val.InitFromDsl(callData.GetParam(i));
                m_Args.Add(val);
            }
            return true;
        }
        private IStoryFunction m_Object = new StoryValue();
        private IStoryFunction<string> m_Msg = new StoryValue<string>();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
    /// <summary>
    /// creategameobject(name, prefab[, parent])[obj("varname")]{
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    ///     loadtimeout(1000);
    ///     disable("typename", "typename", ...);
    ///     remove("typename", "typename", ...);
    /// };
    /// </summary>
    internal class CreateGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CreateGameObjectCommand cmd = new CreateGameObjectCommand();
            cmd.m_Name = m_Name.Clone();
            cmd.m_Prefab = m_Prefab.Clone();
            cmd.m_HaveParent = m_HaveParent;
            cmd.m_Parent = m_Parent.Clone();
            cmd.m_HaveObj = m_HaveObj;
            cmd.m_ObjVarName = m_ObjVarName.Clone();
            cmd.m_Position = m_Position.Clone();
            cmd.m_Rotation = m_Rotation.Clone();
            cmd.m_Scale = m_Scale.Clone();
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                cmd.m_DisableComponents.Add(m_DisableComponents[i].Clone());
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                cmd.m_RemoveComponents.Add(m_RemoveComponents[i].Clone());
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Name.Evaluate(instance, handler, iterator, args);
            m_Prefab.Evaluate(instance, handler, iterator, args);
            if (m_HaveParent) {
                m_Parent.Evaluate(instance, handler, iterator, args);
            }
            if (m_HaveObj) {
                m_ObjVarName.Evaluate(instance, handler, iterator, args);
            }
            m_Position.Evaluate(instance, handler, iterator, args);
            m_Rotation.Evaluate(instance, handler, iterator, args);
            m_Scale.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                m_DisableComponents[i].Evaluate(instance, handler, iterator, args);
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                m_RemoveComponents[i].Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string name = m_Name.Value;
            string prefab = m_Prefab.Value;
            List<string> disables = new List<string>();
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                disables.Add(m_DisableComponents[i].Value);
            }
            List<string> removes = new List<string>();
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                removes.Add(m_RemoveComponents[i].Value);
            }
            var uo = UnityEngine.Resources.Load(prefab);
            var o = UnityEngine.GameObject.Instantiate(uo);
            AfterLoad(instance, name, o, disables, removes);

            return false;
        }
        private void AfterLoad(StoryInstance instance, string name, UnityEngine.Object o, List<string> disables, List<string> removes)
        {
            var obj = o as UnityEngine.GameObject;
            if (null != obj) {
                foreach (string disable in disables) {
                    var type = StoryScriptUtility.GetType(disable);
                    if (null != type) {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i) {
                            var t = comps[i].GetType();
                            t.InvokeMember("enabled", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, comps[i], new object[] { false });
                        }
                    }
                }
                foreach (string remove in removes) {
                    var type = StoryScriptUtility.GetType(remove);
                    if (null != type) {
                        var comps = obj.GetComponentsInChildren(type);
                        for (int i = 0; i < comps.Length; ++i) {
                            StoryScriptUtility.DestroyObject(comps[i]);
                        }
                    }
                }

                obj.name = name;
                if (m_HaveParent) {
                    var parentVal = m_Parent.Value;
                    string path = parentVal.IsString ? parentVal.StringVal : null;
                    if (null != path) {
                        var pobj = UnityEngine.GameObject.Find(path);
                        if (null != pobj) {
                            obj.transform.SetParent(pobj.transform, false);
                        }
                    } else {
                        UnityEngine.GameObject pobj = parentVal.IsObject ? parentVal.ObjectVal as UnityEngine.GameObject : null;
                        if (null != pobj) {
                            obj.transform.SetParent(pobj.transform, false);
                        }
                    }
                }
                if (m_Position.HaveValue) {
                    var v = m_Position.Value;
                    obj.transform.localPosition = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_Rotation.HaveValue) {
                    var v = m_Rotation.Value;
                    obj.transform.localEulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_Scale.HaveValue) {
                    var v = m_Scale.Value;
                    obj.transform.localScale = new UnityEngine.Vector3(v.x, v.y, v.z);
                }
                if (m_HaveObj) {
                    string varName = m_ObjVarName.Value;
                    instance.SetVariable(varName, obj);
                }
            }
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                var callData = funcData.LowerOrderFunction;
                LoadCall(callData);
            }
            else if(funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second) {
                    Load(first);
                    if (second.IsHighOrder) {
                        LoadVarName(second.LowerOrderFunction);
                    }
                    else if (second.HaveParam()) {
                        LoadVarName(second);
                    }
                }
                if (null != second && second.HaveStatement()) {
                    foreach (var comp in second.Params) {
                        var cd = comp as Dsl.FunctionData;
                        if (null != cd) {
                            LoadOptional(cd);
                        }
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_Name.InitFromDsl(callData.GetParam(0));
                m_Prefab.InitFromDsl(callData.GetParam(1));
                if (num > 2) {
                    m_HaveParent = true;
                    m_Parent.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1) {
                m_ObjVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObj = true;
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position") {
                if (num == 3)
                    m_Position.InitFromDsl(callData);
                else
                    m_Position.InitFromDsl(callData.GetParam(0));
            }
            else if (id == "rotation") {
                if (num == 3)
                    m_Rotation.InitFromDsl(callData);
                else
                    m_Rotation.InitFromDsl(callData.GetParam(0));
            }
            else if (id == "scale") {
                if (num == 3)
                    m_Scale.InitFromDsl(callData);
                else
                    m_Scale.InitFromDsl(callData.GetParam(0));
            } else if (id == "disable") {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    var p = new StoryValue<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_DisableComponents.Add(p);
                }
            } else if (id == "remove") {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    var p = new StoryValue<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_RemoveComponents.Add(p);
                }
            }
        }

        private IStoryFunction<string> m_Name = new StoryValue<string>();
        private IStoryFunction<string> m_Prefab = new StoryValue<string>();
        private IStoryFunction m_Parent = new StoryValue();
        private bool m_HaveParent = false;
        private bool m_HaveObj = false;
        private IStoryFunction<string> m_ObjVarName = new StoryValue<string>();
        private IStoryFunction<UnityEngine.Vector3> m_Position = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Rotation = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Scale = new StoryValue<UnityEngine.Vector3>();
        private List<IStoryFunction<string>> m_DisableComponents = new List<IStoryFunction<string>>();
        private List<IStoryFunction<string>> m_RemoveComponents = new List<IStoryFunction<string>>();
    }
    /// <summary>
    /// settransform(name, world_or_local){
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    /// };
    /// </summary>
    internal class SetTransformCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetTransformCommand cmd = new SetTransformCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_LocalOrWorld = m_LocalOrWorld.Clone();
            cmd.m_Position = m_Position.Clone();
            cmd.m_Rotation = m_Rotation.Clone();
            cmd.m_Scale = m_Scale.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);
            m_Position.Evaluate(instance, handler, iterator, args);
            m_Rotation.Evaluate(instance, handler, iterator, args);
            m_Scale.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int local0OrWorld1 = m_LocalOrWorld.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = null;
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    if (m_Position.HaveValue) {
                        var v = m_Position.Value;
                        if (0 == local0OrWorld1) {
                            obj.transform.localPosition = new UnityEngine.Vector3(v.x, v.y, v.z);
                        }
                        else {
                            obj.transform.position = new UnityEngine.Vector3(v.x, v.y, v.z);
                        }
                    }
                    if (m_Rotation.HaveValue) {
                        var v = m_Rotation.Value;
                        if (0 == local0OrWorld1)
                            obj.transform.localEulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                        else
                            obj.transform.eulerAngles = new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    if (m_Scale.HaveValue) {
                        var v = m_Scale.Value;
                        obj.transform.localScale = new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    return true;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                var callData = funcData.LowerOrderFunction;
                LoadCall(callData);
            }
            else if(funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position") {
                if (num == 3)
                    m_Position.InitFromDsl(callData);
                else
                    m_Position.InitFromDsl(callData.GetParam(0));
            } else if (id == "rotation") {
                if (num == 3)
                    m_Rotation.InitFromDsl(callData);
                else
                    m_Rotation.InitFromDsl(callData.GetParam(0));
            } else if (id == "scale") {
                if (num == 3)
                    m_Scale.InitFromDsl(callData);
                else
                    m_Scale.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private IStoryFunction<UnityEngine.Vector3> m_Position = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Rotation = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Scale = new StoryValue<UnityEngine.Vector3>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// addtransform(name, world_or_local){
    ///     position(vector3(x,y,z));
    ///     rotation(vector3(x,y,z));
    ///     scale(vector3(x,y,z));
    /// };
    /// </summary>
    internal class AddTransformCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            AddTransformCommand cmd = new AddTransformCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_LocalOrWorld = m_LocalOrWorld.Clone();
            cmd.m_Position = m_Position.Clone();
            cmd.m_Rotation = m_Rotation.Clone();
            cmd.m_Scale = m_Scale.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);
            m_Position.Evaluate(instance, handler, iterator, args);
            m_Rotation.Evaluate(instance, handler, iterator, args);
            m_Scale.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int local0OrWorld1 = m_LocalOrWorld.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                }
                else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    if (m_Position.HaveValue) {
                        var v = m_Position.Value;
                        if (0 == local0OrWorld1) {
                            obj.transform.localPosition += new UnityEngine.Vector3(v.x, v.y, v.z);
                        }
                        else {
                            obj.transform.position += new UnityEngine.Vector3(v.x, v.y, v.z);
                        }
                    }
                    if (m_Rotation.HaveValue) {
                        var v = m_Rotation.Value;
                        if (0 == local0OrWorld1)
                            obj.transform.localEulerAngles += new UnityEngine.Vector3(v.x, v.y, v.z);
                        else
                            obj.transform.eulerAngles += new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    if (m_Scale.HaveValue) {
                        var v = m_Scale.Value;
                        obj.transform.localScale += new UnityEngine.Vector3(v.x, v.y, v.z);
                    }
                    return true;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                var callData = funcData.LowerOrderFunction;
                LoadCall(callData);
            }
            else if (funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            int num = callData.GetParamNum();
            if (id == "position") {
                if (num == 3)
                    m_Position.InitFromDsl(callData);
                else
                    m_Position.InitFromDsl(callData.GetParam(0));
            }
            else if (id == "rotation") {
                if (num == 3)
                    m_Rotation.InitFromDsl(callData);
                else
                    m_Rotation.InitFromDsl(callData.GetParam(0));
            }
            else if (id == "scale") {
                if (num == 3)
                    m_Scale.InitFromDsl(callData);
                else
                    m_Scale.InitFromDsl(callData.GetParam(0));
            }
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction<int> m_LocalOrWorld = new StoryValue<int>();
        private IStoryFunction<UnityEngine.Vector3> m_Position = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Rotation = new StoryValue<UnityEngine.Vector3>();
        private IStoryFunction<UnityEngine.Vector3> m_Scale = new StoryValue<UnityEngine.Vector3>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// destroygameobject(path);
    /// </summary>
    internal class DestroyGameObjectCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            DestroyGameObjectCommand cmd = new DestroyGameObjectCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var pathVal = m_ObjPath.Value;
            string path = pathVal.IsString ? pathVal.StringVal : null;
            if (null != path) {
                var obj = UnityEngine.GameObject.Find(path);
                if (null != obj) {
                    obj.transform.SetParent(null);
                    
                }
            } else {
                var obj = pathVal.IsObject ? pathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null != obj) {
                    obj.transform.SetParent(null);
                    StoryScriptUtility.DestroyObject(obj);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }

        private IStoryFunction m_ObjPath = new StoryValue();
    }
    /// <summary>
    /// setparent(objpath,parent,stay_world_pos);
    /// </summary>
    internal class SetParentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetParentCommand cmd = new SetParentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Parent = m_Parent.Clone();
            cmd.m_StayWorldPos = m_StayWorldPos.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
            m_ParentObject = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Parent.Evaluate(instance, handler, iterator, args);
            m_StayWorldPos.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            var parentVal = m_Parent.Value;
            int stayWorldPos = m_StayWorldPos.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = null;
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    string parentPath = parentVal.IsString ? parentVal.StringVal : null;
                    if (null != parentPath) {
                        if (string.IsNullOrEmpty(parentPath)) {
                            obj.transform.SetParent(null, stayWorldPos != 0);
                        } else {
                            var pobj = UnityEngine.GameObject.Find(parentPath);
                            if (null != pobj) {
                                m_ParentObject = pobj;
                                obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                            }
                        }
                    } else {
                        UnityEngine.GameObject pobj = parentVal.IsObject ? parentVal.ObjectVal as UnityEngine.GameObject : null;
                        if (null != pobj) {
                            m_ParentObject = pobj;
                            obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                        } else {
                            try {
                                int id = parentVal.GetInt();
                                if (id < 0) {
                                    m_ParentObject = null;
                                    obj.transform.SetParent(null, stayWorldPos != 0);
                                } else {
                                    pobj = null;
                                    if (null != pobj) {
                                        m_ParentObject = pobj;
                                        obj.transform.SetParent(pobj.transform, stayWorldPos != 0);
                                    }
                                }
                            } catch {
                            }
                        }
                    }
                    return true;
                }
            } else if (null != m_Object) {
                if (null == m_Object.transform.parent && null == m_ParentObject) {
                    return false;
                } else if (null != m_Object.transform.parent && null != m_ParentObject && m_Object.transform.parent.gameObject == m_ParentObject) {
                    return false;
                } else if (null == m_ParentObject) {
                    m_Object.transform.SetParent(null, stayWorldPos != 0);
                    return true;
                } else {
                    m_Object.transform.SetParent(m_ParentObject.transform, stayWorldPos != 0);
                    return true;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 2) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Parent.InitFromDsl(callData.GetParam(1));
                m_StayWorldPos.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction m_Parent = new StoryValue();
        private IStoryFunction<int> m_StayWorldPos = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
        private UnityEngine.GameObject m_ParentObject = null;
    }
    /// <summary>
    /// setactive(objpath,1_or_0);
    /// </summary>
    internal class SetActiveCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetActiveCommand cmd = new SetActiveCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Active = m_Active.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Active.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int active = m_Active.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = null;
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    obj.SetActive(active != 0);
                    m_Object = obj;
                    return true;
                }
            } else if (null != m_Object) {
                if (active != 0 && m_Object.activeSelf || active == 0 && !m_Object.activeSelf) {
                    return false;
                } else {
                    m_Object.SetActive(active != 0);
                    return false;
                }
            }        
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Active.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction<int> m_Active = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// setvisible(objpath,1_or_0);
    /// </summary>
    internal class SetVisibleCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            SetVisibleCommand cmd = new SetVisibleCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Visible = m_Visible.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
            m_Handled = false;
            m_Object = null;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Visible.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objVal = m_ObjPath.Value;
            int visible = m_Visible.Value;
            string objPath = objVal.IsString ? objVal.StringVal : null;
            if (!m_Handled) {
                m_Handled = true;
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    obj = objVal.IsObject ? objVal.ObjectVal as UnityEngine.GameObject : null;
                    if (null == obj) {
                        try {
                            int id = objVal.GetInt();
                            obj = null;
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    m_Object = obj;
                    var renderers = obj.GetComponentsInChildren<UnityEngine.Renderer>();
                    if (null != renderers) {
                        for (int i = 0; i < renderers.Length; ++i) {
                            renderers[i].enabled = visible != 0;
                        }
                    }
                    return true;
                }
            } else if (null != m_Object) {
                var renderers = m_Object.GetComponentsInChildren<UnityEngine.Renderer>();
                if (null != renderers) {
                    for (int i = 0; i < renderers.Length; ++i) {
                        var renderer = renderers[i];
                        if (visible != 0 && renderer.isVisible || visible == 0 && !renderer.isVisible) {
                            continue;
                        }
                        else {
                            renderer.enabled = visible != 0;
                        }
                    }
                    return false;
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Visible.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction<int> m_Visible = new StoryValue<int>();

        private bool m_Handled = false;
        private UnityEngine.GameObject m_Object = null;
    }
    /// <summary>
    /// addcomponent(objpath,type)[obj("varname")];
    /// </summary>
    internal class AddComponentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            AddComponentCommand cmd = new AddComponentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_ComponentType = m_ComponentType.Clone();
            cmd.m_HaveObj = m_HaveObj;
            cmd.m_ObjVarName = m_ObjVarName.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            if (m_HaveObj) {
                m_ObjVarName.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objPathVal = m_ObjPath.Value;
            var componentType = m_ComponentType.Value;
            UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                        obj = null;
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                UnityEngine.Component component = null;
                Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                if (null != t) {
                    component = obj.AddComponent(t);
                } else {
                    string name = componentType.IsString ? componentType.StringVal : null;
                    if (null != name) {
                        t = StoryScriptUtility.GetType(name);
                        component = obj.AddComponent(t);
                    }
                }
                if (m_HaveObj) {
                    string varName = m_ObjVarName.Value;
                    instance.SetVariable(varName, component);
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_ComponentType.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }
        protected override bool Load(Dsl.StatementData statementData)
        {
            if (statementData.Functions.Count == 2) {
                Dsl.FunctionData first = statementData.First.AsFunction;
                Dsl.FunctionData second = statementData.Second.AsFunction;
                if (null != first && null != second) {
                    Load(first);
                    LoadVarName(second);
                }
            }
            return true;
        }
        private void LoadVarName(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "obj" && callData.GetParamNum() == 1) {
                m_ObjVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObj = true;
            }
        }
        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction m_ComponentType = new StoryValue();
        private bool m_HaveObj = false;
        private IStoryFunction<string> m_ObjVarName = new StoryValue<string>();
    }
    /// <summary>
    /// removecomponent(objpath,type);
    /// </summary>
    internal class RemoveComponentCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            RemoveComponentCommand cmd = new RemoveComponentCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_ComponentType = m_ComponentType.Clone();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var objPathVal = m_ObjPath.Value;
            var componentType = m_ComponentType.Value;
            UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                        obj = null;
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                //UnityEngine.Component component = null;
                Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                if (null != t) {
                    var comp = obj.GetComponent(t);
                    StoryScriptUtility.DestroyObject(comp);
                } else {
                    string name = componentType.IsString ? componentType.StringVal : null;
                    if (null != name) {
                        t = StoryScriptUtility.GetType(name);
                        var comp = obj.GetComponent(t);
                        StoryScriptUtility.DestroyObject(comp);
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_ComponentType.InitFromDsl(callData.GetParam(1));
            }
            return true;
        }

        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction m_ComponentType = new StoryValue();
    }
    /// <summary>
    /// openurl(url);
    /// </summary>
    internal class OpenUrlCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            OpenUrlCommand cmd = new OpenUrlCommand();
            cmd.m_Url = m_Url.Clone();
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_Url.Evaluate(instance, handler, iterator, args);
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            UnityEngine.Application.OpenURL(m_Url.Value);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                m_Url.InitFromDsl(callData.GetParam(0));
            }
            return true;
        }
        private IStoryFunction<string> m_Url = new StoryValue<string>();
    }
    /// <summary>
    /// quit();
    /// </summary>
    internal class QuitCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            QuitCommand cmd = new QuitCommand();
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
        
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            UnityEngine.Application.Quit();
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            return true;
        }
    }
    /// <summary>
    /// gameobjectanimation(obj, anim[, normalized_time]);
    /// </summary>
    internal class GameObjectAnimationCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            GameObjectAnimationCommand cmd = new GameObjectAnimationCommand();
            cmd.m_ParamNum = m_ParamNum;
            cmd.m_ObjPath = m_ObjPath.Clone();
            cmd.m_Anim = m_Anim.Clone();
            cmd.m_Time = m_Time.Clone();
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_Anim.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_Time.Evaluate(instance, handler, iterator, args);
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var o = m_ObjPath.Value;
            string objPath = o.IsString ? o.StringVal : null;
            UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
            if (null == uobj) {
                if (null != objPath) {
                    uobj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int objId = o.GetInt();
                        uobj = null;
                    } catch {
                        uobj = null;
                    }
                }
            }
            if (null != uobj) {
                string anim = m_Anim.Value;
                var animators = uobj.GetComponentsInChildren<UnityEngine.Animator>();
                if (null != animators) {
                    for (int i = 0; i < animators.Length; ++i) {
                        animators[i].Play(anim);
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ParamNum = callData.GetParamNum();
            if (m_ParamNum > 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
                m_Anim.InitFromDsl(callData.GetParam(1));
            }
            if (m_ParamNum > 2) {
                m_Time.InitFromDsl(callData.GetParam(2));
            }
            return true;
        }
        private int m_ParamNum = 0;
        private IStoryFunction m_ObjPath = new StoryValue();
        private IStoryFunction<string> m_Anim = new StoryValue<string>();
        private IStoryFunction<float> m_Time = new StoryValue<float>();
    }
    /// <summary>
    /// gameobjectanimationparam(obj)
    /// {
    ///     float(name,val);
    ///     int(name,val);
    ///     bool(name,val);
    ///     trigger(name,val);
    /// };
    /// </summary>
    internal class GameObjectAnimationParamCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            GameObjectAnimationParamCommand cmd = new GameObjectAnimationParamCommand();
            cmd.m_ObjPath = m_ObjPath.Clone();
            for (int i = 0; i < m_Params.Count; ++i) {
                ParamInfo param = new ParamInfo();
                param.CopyFrom(m_Params[i]);
                cmd.m_Params.Add(param);
            }
            return cmd;
        }
        protected override void ResetState()
        {
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
                pair.Key.Evaluate(instance, handler, iterator, args);
                pair.Value.Evaluate(instance, handler, iterator, args);
            }

            for (int i = 0; i < m_Params.Count; ++i) {
                var pair = m_Params[i];
            }
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            var o = m_ObjPath.Value;
            string objPath = o.IsString ? o.StringVal : null;
            UnityEngine.GameObject obj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
            if (null == obj) {
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                } else {
                    try {
                        int objId = o.GetInt();
                        obj = null;
                    } catch {
                        obj = null;
                    }
                }
            }
            if (null != obj) {
                UnityEngine.Animator animator = obj.GetComponentInChildren<UnityEngine.Animator>();
                if (null != animator) {
                    for (int i = 0; i < m_Params.Count; ++i) {
                        var param = m_Params[i];
                        string type = param.Type;
                        string key = param.Key.Value;
                        var val = param.Value.Value;
                        if (type == "int") {
                            int v = val.GetInt();
                            animator.SetInteger(key, v);
                        } else if (type == "float") {
                            float v = val.GetFloat();
                            animator.SetFloat(key, v);
                        } else if (type == "bool") {
                            bool v = val.GetBool();
                            animator.SetBool(key, v);
                        } else if (type == "trigger") {
                            string v = val.ToString();
                            if (v == "false") {
                                animator.ResetTrigger(key);
                            } else {
                                animator.SetTrigger(key);
                            }
                        }
                    }
                }
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData funcData)
        {
            if (funcData.IsHighOrder) {
                LoadCall(funcData.LowerOrderFunction);
            }
            else if (funcData.HaveParam()) {
                LoadCall(funcData);
            }
            if (funcData.HaveStatement()) {
                for (int i = 0; i < funcData.GetParamNum(); ++i) {
                    Dsl.ISyntaxComponent statement = funcData.GetParam(i);
                    Dsl.FunctionData stCall = statement as Dsl.FunctionData;
                    if (null != stCall && stCall.GetParamNum() >= 2) {
                        string id = stCall.GetId();
                        ParamInfo param = new ParamInfo(id, stCall.GetParam(0), stCall.GetParam(1));
                        m_Params.Add(param);
                    }
                }
            }
            return true;
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1) {
                m_ObjPath.InitFromDsl(callData.GetParam(0));
            }
        }
        private class ParamInfo
        {
            internal string Type;
            internal IStoryFunction<string> Key;
            internal IStoryFunction Value;
            internal ParamInfo()
            {
                Init();
            }
            internal ParamInfo(string type, Dsl.ISyntaxComponent keyDsl, Dsl.ISyntaxComponent valDsl)
                : this()
            {
                Type = type;
                Key.InitFromDsl(keyDsl);
                Value.InitFromDsl(valDsl);
            }
            internal void CopyFrom(ParamInfo other)
            {
                Type = other.Type;
                Key = other.Key.Clone();
                Value = other.Value.Clone();
            }
            private void Init()
            {
                Type = string.Empty;
                Key = new StoryValue<string>();
                Value = new StoryValue();
            }
        }
        private IStoryFunction m_ObjPath = new StoryValue();
        private List<ParamInfo> m_Params = new List<ParamInfo>();
    }
    /// <summary>
    /// loadscp(dsl_script_file);
    /// or
    /// loadscp("name", func(param1, param2, ...));
    /// or
    /// loadscp(name => func(param1, param2, ...));
    /// </summary>
    internal sealed class LoadScriptCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            LoadScriptCommand cmd = new LoadScriptCommand();
            cmd.m_ArgNum = m_ArgNum;
            if (string.IsNullOrEmpty(m_Name))
                cmd.m_FileOrId = m_FileOrId.Clone();
            cmd.m_Name = m_Name;
            cmd.m_FuncCall = m_FuncCall;
            cmd.m_Params = m_Params;
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            if (string.IsNullOrEmpty(m_Name))
                m_FileOrId.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string f = m_FileOrId.Value;
            if (m_ArgNum == 1) {
                if (string.IsNullOrEmpty(m_Name)) {
                    Main.LoadScript(f);
                }
                else {
                    f = m_Name;
                    var func = new Dsl.FunctionData();
                    func.AddParam(m_FuncCall);
                    Main.EvalAsFunc(f, func, m_Params);
                }
            }
            else if (m_ArgNum == 2 && null != m_FuncCall) {
                var func = new Dsl.FunctionData();
                func.AddParam(m_FuncCall);
                Main.EvalAsFunc(f, func, m_Params);
            }
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            m_ArgNum = callData.GetParamNum();
            if (m_ArgNum > 0) {
                var p0 = callData.GetParam(0);
                var func = p0 as Dsl.FunctionData;
                if (null != func && func.IsOperatorParamClass() && func.GetId() == "=>") {
                    m_Name = func.GetParamId(0);
                    m_FuncCall = func.GetParam(1) as Dsl.FunctionData;
                    if (null != m_FuncCall) {
                        foreach (var p in m_FuncCall.Params) {
                            m_Params.Add(p.GetId());
                        }
                    }
                }
                else {
                    m_FileOrId.InitFromDsl(p0);
                    if (m_ArgNum > 1) {
                        m_FuncCall = callData.GetParam(1) as Dsl.FunctionData;
                        if (null != m_FuncCall) {
                            foreach (var p in m_FuncCall.Params) {
                                m_Params.Add(p.GetId());
                            }
                        }
                    }
                }
            }
            return true;
        }

        private int m_ArgNum = 0;
        private IStoryFunction<string> m_FileOrId = new StoryValue<string>();
        private string m_Name = string.Empty;
        private Dsl.FunctionData m_FuncCall;
        private List<string> m_Params = new List<string>();
    }
    /// <summary>
    /// callscp("func", arg1, arg2, ...);
    /// or
    /// callscp(func(arg1, arg2, ...));
    /// </summary>
    internal class CallScriptCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            CallScriptCommand cmd = new CallScriptCommand();
            cmd.m_FuncName = m_FuncName;
            if(string.IsNullOrEmpty(m_FuncName))
                cmd.m_Func = m_Func.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                cmd.m_Args.Add(val.Clone());
            }
            return cmd;
        }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            if (string.IsNullOrEmpty(m_FuncName))
                m_Func.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
        }

        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            string func = m_FuncName;
            if (string.IsNullOrEmpty(m_FuncName))
                func = m_Func.Value;
            ArrayList arglist = new ArrayList();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction val = m_Args[i];
                arglist.Add(val.Value.GetObject());
            }
            object[] args = arglist.ToArray();
            Main.Call(func, args);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                var p0 = callData.GetParam(0);
                var func = p0 as Dsl.FunctionData;
                if (null != func) {
                    m_FuncName = func.GetId();
                    for (int i = 1; i < func.GetParamNum(); ++i) {
                        StoryValue val = new StoryValue();
                        val.InitFromDsl(func.GetParam(i));
                        m_Args.Add(val);
                    }
                }
                else {
                    m_Func.InitFromDsl(p0);
                    for (int i = 1; i < callData.GetParamNum(); ++i) {
                        StoryValue val = new StoryValue();
                        val.InitFromDsl(callData.GetParam(i));
                        m_Args.Add(val);
                    }
                }
            }
            return true;
        }

        private string m_FuncName = string.Empty;
        private IStoryFunction<string> m_Func = new StoryValue<string>();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
    }
    /// <summary>
    /// evalscp(code_string);
    /// or
    /// evalscp(code_dsl);
    /// </summary>
    internal class EvalScriptCommand : AbstractStoryCommand
    {
        protected override IStoryCommand CloneCommand()
        {
            EvalScriptCommand cmd = new EvalScriptCommand();
            cmd.m_IsString = m_IsString;
            if (m_IsString)
                cmd.m_Code = m_Code.Clone();
            cmd.m_Exps = m_Exps;
            return cmd;
        }
        protected override void ResetState()
        { }
        protected override void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            if(m_IsString)
                m_Code.Evaluate(instance, handler, iterator, args);
        }
        protected override bool ExecCommand(StoryInstance instance, StoryMessageHandler handler, long delta)
        {
            if (m_IsString)
                Main.EvalAndRun(m_Code.Value);
            else
                Main.EvalAndRun(m_Exps);
            return false;
        }
        protected override bool Load(Dsl.FunctionData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0) {
                var vd = callData.GetParam(0) as Dsl.ValueData;
                if (null != vd && vd.IsString()) {
                    m_IsString = true;
                    m_Code.InitFromDsl(vd);
                }
                else {
                    m_IsString = false;
                    foreach(var p in callData.Params) {
                        m_Exps.Add(p);
                    }
                }
            }
            return true;
        }

        private bool m_IsString = false;
        private IStoryFunction<string> m_Code = new StoryValue<string>();
        private List<ISyntaxComponent> m_Exps = new List<ISyntaxComponent>();
    }
}
