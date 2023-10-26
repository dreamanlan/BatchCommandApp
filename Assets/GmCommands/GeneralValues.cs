using System;
using System.Collections.Generic;
using StoryScript;
using System.IO;
using System.Collections;
using Dsl;

namespace GmCommands
{
    internal sealed class GetTimeValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetTimeValue val = new GetTimeValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            TryUpdateValue(instance);
        }
        public void Analyze(StoryInstance instance)
        {
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = UnityEngine.Time.time;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetTimeScaleValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetTimeScaleValue val = new GetTimeScaleValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            TryUpdateValue(instance);
        }
        public void Analyze(StoryInstance instance)
        {
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = UnityEngine.Time.timeScale;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetTimeSinceStartupValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetTimeSinceStartupValue val = new GetTimeSinceStartupValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            TryUpdateValue(instance);
        }
        public void Analyze(StoryInstance instance)
        {
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = UnityEngine.Time.realtimeSinceStartup;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsActiveValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            IsActiveValue val = new IsActiveValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string objPath = o.IsString ? o.StringVal : null;
                UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
                if (null != objPath) {
                    UnityEngine.GameObject obj = UnityEngine.GameObject.Find(objPath);
                    if (null != obj) {
                        m_Value = obj.activeSelf ? 1 : 0;
                    } else {
                        m_Value = 0;
                    }
                } else if (null != uobj) {
                    m_Value = uobj.activeSelf ? 1 : 0;
                } else {
                    try {
                        int objId = o.GetInt();
                        m_Value = 0;
                    } catch {
                        m_Value = 0;
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsReallyActiveValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            IsReallyActiveValue val = new IsReallyActiveValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string objPath = o.IsString ? o.StringVal : null;
                UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
                if (null != objPath) {
                    UnityEngine.GameObject obj = UnityEngine.GameObject.Find(objPath);
                    if (null != obj) {
                        m_Value = obj.activeInHierarchy ? 1 : 0;
                    } else {
                        m_Value = 0;
                    }
                } else if (null != uobj) {
                    m_Value = uobj.activeInHierarchy ? 1 : 0;
                } else {
                    try {
                        int objId = o.GetInt();
                        m_Value = 0;
                    } catch {
                        m_Value = 0;
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsVisibleValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            IsVisibleValue val = new IsVisibleValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string objPath = o.IsString ? o.StringVal : null;
                UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
                if (null != uobj) {
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
                    var renderer = uobj.GetComponentInChildren<UnityEngine.Renderer>();
                    if (null != renderer) {
                        m_Value = renderer.isVisible ? 1 : 0;
                    } else {
                        m_Value = 0;
                    }
                } else {
                    m_Value = 0;
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentValue val = new GetComponentValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    } else {
                        try {
                            int objId = objPath.GetInt();
                            obj = null;
                        } catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        UnityEngine.Component component = obj.GetComponent(t);
                        m_Value = component;
                    } else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            UnityEngine.Component component = obj.GetComponent(name);
                            m_Value = component;
                        }
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentInParentValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentInParentValue val = new GetComponentInParentValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    }
                    else {
                        try {
                            int objId = objPath.GetInt();
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        UnityEngine.Component component = obj.GetComponentInParent(t);
                        m_Value = component;
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = Utility.GetType(name);
                            if (null != t) {
                                UnityEngine.Component component = obj.GetComponentInParent(t);
                                m_Value = component;
                            } else {
                                m_Value = BoxedValue.NullObject;
                            }
                        }
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentInChildrenValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
                if (m_ParamNum > 2) {
                    m_IncludeInactive.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentInChildrenValue val = new GetComponentInChildrenValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_IncludeInactive = m_IncludeInactive.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_IncludeInactive.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                int includeInactive = 1;
                if (m_ParamNum > 2) {
                    includeInactive = m_IncludeInactive.Value;
                }
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    }
                    else {
                        try {
                            int objId = objPath.GetInt();
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        UnityEngine.Component component = obj.GetComponentInChildren(t, includeInactive != 0);
                        m_Value = component;
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = Utility.GetType(name);
                            if (null != t) {
                                UnityEngine.Component component = obj.GetComponentInChildren(t, includeInactive != 0);
                                m_Value = component;
                            } else {
                                m_Value = BoxedValue.NullObject;
                            }
                        }
                    }
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private IStoryValue<int> m_IncludeInactive = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentsValue val = new GetComponentsValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    }
                    else {
                        try {
                            int objId = objPath.GetInt();
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        var comps = obj.GetComponents(t);
                        if (null != comps)
                            m_Value = comps;
                        else
                            m_Value = new List<UnityEngine.Component>();
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = Utility.GetType(name);
                            if (null != t) {
                                var comps = obj.GetComponents(t);
                                if (null != comps)
                                    m_Value = comps;
                                else
                                    m_Value = new List<UnityEngine.Component>();
                            } else {
                                m_Value = new List<UnityEngine.Component>();
                            }
                        }
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsInParentValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
                if (m_ParamNum > 2) {
                    m_IncludeInactive.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentsInParentValue val = new GetComponentsInParentValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_IncludeInactive = m_IncludeInactive.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_IncludeInactive.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                int includeInactive = 1;
                if (m_ParamNum > 2) {
                    includeInactive = m_IncludeInactive.Value;
                }
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    }
                    else {
                        try {
                            int objId = (int)objPath;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        var comps = obj.GetComponentsInParent(t, includeInactive != 0);
                        if (null != comps)
                            m_Value = comps;
                        else
                            m_Value = new List<UnityEngine.Component>();
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = Utility.GetType(name);
                            if (null != t) {
                                var comps = obj.GetComponentsInParent(t, includeInactive != 0);
                                if (null != comps)
                                    m_Value = comps;
                                else
                                    m_Value = new List<UnityEngine.Component>();
                            }
                            else {
                                m_Value = new List<UnityEngine.Component>();
                            }
                        }
                    }
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private IStoryValue<int> m_IncludeInactive = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsInChildrenValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ComponentType.InitFromDsl(callData.GetParam(1));
                }
                if (m_ParamNum > 2) {
                    m_IncludeInactive.InitFromDsl(callData.GetParam(2));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetComponentsInChildrenValue val = new GetComponentsInChildrenValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ComponentType = m_ComponentType.Clone();
            val.m_IncludeInactive = m_IncludeInactive.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ComponentType.Evaluate(instance, handler, iterator, args);
            if (m_ParamNum > 2) {
                m_IncludeInactive.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ComponentType.HaveValue) {
                m_HaveValue = true;
                var objPath = m_ObjPath.Value;
                var componentType = m_ComponentType.Value;
                int includeInactive = 1;
                if (m_ParamNum > 2) {
                    includeInactive = m_IncludeInactive.Value;
                }
                UnityEngine.GameObject obj = objPath.IsObject ? objPath.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string path = objPath.IsString ? objPath.StringVal : null;
                    if (null != path) {
                        obj = UnityEngine.GameObject.Find(path);
                    }
                    else {
                        try {
                            int objId = objPath.GetInt();
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    Type t = componentType.IsObject ? componentType.ObjectVal as Type : null;
                    if (null != t) {
                        var comps = obj.GetComponentsInChildren(t, includeInactive != 0);
                        if (null != comps)
                            m_Value = comps;
                        else
                            m_Value = new List<UnityEngine.Component>();
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = Utility.GetType(name);
                            if (null != t) {
                                var comps = obj.GetComponentsInChildren(t, includeInactive != 0);
                                if (null != comps)
                                    m_Value = comps;
                                else
                                    m_Value = new List<UnityEngine.Component>();
                            }
                            else {
                                m_Value = new List<UnityEngine.Component>();
                            }
                        }
                    }
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue m_ComponentType = new StoryValue();
        private IStoryValue<int> m_IncludeInactive = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetGameObjectValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData funcData = param as Dsl.FunctionData;
            if (null != funcData) {
                Load(funcData);
            }
        }
        public IStoryValue Clone()
        {
            GetGameObjectValue val = new GetGameObjectValue();
            val.m_ObjPath = m_ObjPath.Clone();
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                val.m_DisableComponents.Add(m_DisableComponents[i].Clone());
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                val.m_RemoveComponents.Add(m_RemoveComponents[i].Clone());
            }
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_DisableComponents.Count; ++i) {
                m_DisableComponents[i].Evaluate(instance, handler, iterator, args);
            }
            for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                m_RemoveComponents[i].Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string objPath = o.IsString ? o.StringVal : null;

                StrList disables = new StrList();
                for (int i = 0; i < m_DisableComponents.Count; ++i) {
                    disables.Add(m_DisableComponents[i].Value);
                }
                StrList removes = new StrList();
                for (int i = 0; i < m_RemoveComponents.Count; ++i) {
                    removes.Add(m_RemoveComponents[i].Value);
                }
                UnityEngine.GameObject obj = null;
                if (null != objPath) {
                    obj = UnityEngine.GameObject.Find(objPath);
                    if (null != obj) {
                        m_Value = obj;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    try {
                        int objId = o.GetInt();
                        obj = null;
                        m_Value = obj;
                    } catch {
                        m_Value = BoxedValue.NullObject;
                    }
                }
                if (null != obj) {
                    foreach (string disable in disables) {
                        var type = Utility.GetType(disable);
                        if (null != type) {
                            var comps = obj.GetComponentsInChildren(type);
                            for (int i = 0; i < comps.Length; ++i) {
                                var t = comps[i].GetType();
                                t.InvokeMember("enabled", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, comps[i], new object[] { false });
                            }
                        }
                    }
                    foreach (string remove in removes) {
                        var type = Utility.GetType(remove);
                        if (null != type) {
                            var comps = obj.GetComponentsInChildren(type);
                            for (int i = 0; i < comps.Length; ++i) {
                                Utility.DestroyObject(comps[i]);
                            }
                        }
                    }
                }
            }
        }
        private void Load(Dsl.FunctionData funcData)
        {
            var callData = funcData.ThisOrLowerOrderCall;
            if (callData.IsValid()) {
                LoadCall(callData);
            }
            if (funcData.HaveStatement()) {
                foreach (var comp in funcData.Params) {
                    var cd = comp as Dsl.FunctionData;
                    if (null != cd) {
                        LoadOptional(cd);
                    }
                }
            }
        }
        private void LoadCall(Dsl.FunctionData callData)
        {
            if (callData.GetId() == "getgameobject") {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        private void LoadOptional(Dsl.FunctionData callData)
        {
            string id = callData.GetId();
            if (id == "disable") {
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

        private IStoryValue m_ObjPath = new StoryValue();
        private List<IStoryValue<string>> m_DisableComponents = new List<IStoryValue<string>>();
        private List<IStoryValue<string>> m_RemoveComponents = new List<IStoryValue<string>>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetParentValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetParentValue val = new GetParentValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string objPath = o.IsString ? o.StringVal : null;
                UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
                if (null != objPath) {
                    var obj = UnityEngine.GameObject.Find(objPath);
                    if (null != obj && null != obj.transform.parent) {
                        m_Value = obj.transform.parent.gameObject;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else if (null != uobj) {
                    if (null != uobj.transform.parent) {
                        m_Value = uobj.transform.parent.gameObject;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    try {
                        int objId = o.GetInt();
                        m_Value = BoxedValue.NullObject;
                    } catch {
                        m_Value = BoxedValue.NullObject;
                    }
                }
            }
        }
        private IStoryValue m_ObjPath = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetChildValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 1) {
                    m_ObjPath.InitFromDsl(callData.GetParam(0));
                    m_ChildPath.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetChildValue val = new GetChildValue();
            val.m_ObjPath = m_ObjPath.Clone();
            val.m_ChildPath = m_ChildPath.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjPath.Evaluate(instance, handler, iterator, args);
            m_ChildPath.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjPath.HaveValue && m_ChildPath.HaveValue) {
                m_HaveValue = true;
                var o = m_ObjPath.Value;
                string childPath = m_ChildPath.Value;
                string objPath = o.IsString ? o.StringVal : null;
                UnityEngine.GameObject uobj = o.IsObject ? o.ObjectVal as UnityEngine.GameObject : null;
                if (null != objPath) {
                    var obj = UnityEngine.GameObject.Find(objPath);
                    if (null != obj) {
                        var t = Utility.FindChildRecursive(obj.transform, childPath);
                        if (null != t) {
                            m_Value = t.gameObject;
                        } else {
                            m_Value = BoxedValue.NullObject;
                        }
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else if (null != uobj) {
                    var t = Utility.FindChildRecursive(uobj.transform, childPath);
                    if (null != t) {
                        m_Value = t.gameObject;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    try {
                        int objId = o.GetInt();
                        m_Value = BoxedValue.NullObject;
                    } catch {
                        m_Value = BoxedValue.NullObject;
                    }
                }
            }
        }

        private IStoryValue m_ObjPath = new StoryValue();
        private IStoryValue<string> m_ChildPath = new StoryValue<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUnityTypeValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_TypeName.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetUnityTypeValue val = new GetUnityTypeValue();
            val.m_TypeName = m_TypeName.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_TypeName.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_TypeName.HaveValue) {
                m_HaveValue = true;
                string typeName = m_TypeName.Value;
                if (null != typeName) {
                    if (!typeName.StartsWith("UnityEngine.")) {
                        typeName = string.Format("UnityEngine.{0},UnityEngine", typeName);
                    }
                    Type t = Type.GetType(typeName);
                    if (null != t) {
                        m_Value = t;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }
        private IStoryValue<string> m_TypeName = new StoryValue<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUnityUiTypeValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_TypeName.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetUnityUiTypeValue val = new GetUnityUiTypeValue();
            val.m_TypeName = m_TypeName.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_TypeName.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_TypeName.HaveValue) {
                m_HaveValue = true;
                string typeName = m_TypeName.Value;
                if (null != typeName) {
                    if (!typeName.StartsWith("UnityEngine.UI.")) {
                        typeName = string.Format("UnityEngine.UI.{0},UnityEngine.UI", typeName);
                    }
                    Type t = Type.GetType(typeName);
                    if (null != t) {
                        m_Value = t;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }
        private IStoryValue<string> m_TypeName = new StoryValue<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUserTypeValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    m_TypeName.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetUserTypeValue val = new GetUserTypeValue();
            val.m_TypeName = m_TypeName.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_TypeName.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_TypeName.HaveValue) {
                m_HaveValue = true;
                string typeName = m_TypeName.Value;
                if (null != typeName) {
                    typeName = string.Format("{0},Assembly-CSharp", typeName);
                    Type t = Type.GetType(typeName);
                    if (null != t) {
                        m_Value = t;
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else {
                    m_Value = BoxedValue.NullObject;
                }
            }
        }
        private IStoryValue<string> m_TypeName = new StoryValue<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetPositionValue val = new GetPositionValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt;
                }
                else {
                    m_Value = UnityEngine.Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionXValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetPositionXValue val = new GetPositionXValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.x;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionYValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetPositionYValue val = new GetPositionYValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.y;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionZValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetPositionZValue val = new GetPositionZValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localPosition;
                    else
                        pt = obj.transform.position;
                    m_Value = pt.z;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetRotationValue val = new GetRotationValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt;
                }
                else {
                    m_Value = UnityEngine.Vector3.zero;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationXValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetRotationXValue val = new GetRotationXValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.x;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationYValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetRotationYValue val = new GetRotationYValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.y;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationZValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() >= 1) {
                m_ParamNum = callData.GetParamNum();
                m_ObjId.InitFromDsl(callData.GetParam(0));
                if (m_ParamNum > 1)
                    m_LocalOrWorld.InitFromDsl(callData.GetParam(1));
            }
        }
        public IStoryValue Clone()
        {
            GetRotationZValue val = new GetRotationZValue();
            val.m_ParamNum = m_ParamNum;
            val.m_ObjId = m_ObjId.Clone();
            val.m_LocalOrWorld = m_LocalOrWorld.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);
            m_LocalOrWorld.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                int worldOrLocal = m_LocalOrWorld.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    if (0 == worldOrLocal)
                        pt = obj.transform.localEulerAngles;
                    else
                        pt = obj.transform.eulerAngles;
                    m_Value = pt.z;
                }
                else {
                    m_Value = 0.0f;
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryValue m_ObjId = new StoryValue();
        private IStoryValue<int> m_LocalOrWorld = new StoryValue<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            GetScaleValue val = new GetScaleValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt;
                }
                else {
                    m_Value = new UnityEngine.Vector3(1, 1, 1);
                }
            }
        }

        private IStoryValue m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleXValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            GetScaleXValue val = new GetScaleXValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.x;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleYValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            GetScaleYValue val = new GetScaleYValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.y;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleZValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            GetScaleZValue val = new GetScaleZValue();
            val.m_ObjId = m_ObjId.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_ObjId.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_ObjId.HaveValue) {
                m_HaveValue = true;
                var objPathVal = m_ObjId.Value;
                UnityEngine.GameObject obj = objPathVal.IsObject ? objPathVal.ObjectVal as UnityEngine.GameObject : null;
                if (null == obj) {
                    string objPath = objPathVal.IsString ? objPathVal.StringVal : null;
                    if (null != objPath) {
                        obj = UnityEngine.GameObject.Find(objPath);
                    }
                    else {
                        try {
                            int id = objPathVal.IsInteger ? objPathVal.GetInt() : -1;
                            obj = null;
                        }
                        catch {
                            obj = null;
                        }
                    }
                }
                if (null != obj) {
                    UnityEngine.Vector3 pt;
                    pt = obj.transform.localScale;
                    m_Value = pt.z;
                }
                else {
                    m_Value = 1.0f;
                }
            }
        }

        private IStoryValue m_ObjId = new StoryValue();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class Deg2RadValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Degree.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            Deg2RadValue val = new Deg2RadValue();
            val.m_Degree = m_Degree.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Degree.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_Degree.HaveValue) {
                float degree = m_Degree.Value;
                m_HaveValue = true;
                m_Value = degree * MathF.PI / 180.0f;
            }
        }

        private IStoryValue<float> m_Degree = new StoryValue<float>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class Rad2DegValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Radian.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryValue Clone()
        {
            Rad2DegValue val = new Rad2DegValue();
            val.m_Radian = m_Radian.Clone();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;

            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Radian.Evaluate(instance, handler, iterator, args);

            TryUpdateValue(instance);
        }
        public bool HaveValue
        {
            get
            {
                return m_HaveValue;
            }
        }
        public BoxedValue Value
        {
            get
            {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_Radian.HaveValue) {
                float radian = m_Radian.Value;
                m_HaveValue = true;
                m_Value = radian * 180.0f / MathF.PI;
            }
        }

        private IStoryValue<float> m_Radian = new StoryValue<float>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetFileNameValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetFileNameValue val = new GetFileNameValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            var path = m_Path.Value;
            m_Value = Path.GetFileName(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetDirectoryNameValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetDirectoryNameValue val = new GetDirectoryNameValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            var path = m_Path.Value;
            m_Value = Path.GetDirectoryName(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetExtensionValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 1) {
                    m_Path.InitFromDsl(callData.GetParam(0));
                }
            }
        }
        public IStoryValue Clone()
        {
            GetExtensionValue val = new GetExtensionValue();
            val.m_Path = m_Path.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Path.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            var path = m_Path.Value;
            m_Value = Path.GetExtension(path);
        }

        private IStoryValue<string> m_Path = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CombinePathValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                m_ParamNum = callData.GetParamNum();
                if (m_ParamNum == 2) {
                    m_Path1.InitFromDsl(callData.GetParam(0));
                    m_Path2.InitFromDsl(callData.GetParam(1));
                }
            }
        }
        public IStoryValue Clone()
        {
            CombinePathValue val = new CombinePathValue();
            val.m_Path1 = m_Path1.Clone();
            val.m_Path2 = m_Path2.Clone();
            val.m_ParamNum = m_ParamNum;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            m_Path1.Evaluate(instance, handler, iterator, args);
            m_Path2.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            var path1 = m_Path1.Value;
            var path2 = m_Path2.Value;
            m_Value = Path.Combine(path1, path2);
        }

        private IStoryValue<string> m_Path1 = new StoryValue<string>();
        private IStoryValue<string> m_Path2 = new StoryValue<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetStreamingAssetsValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetStreamingAssetsValue val = new GetStreamingAssetsValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = UnityEngine.Application.streamingAssetsPath;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPersistentPathValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryValue Clone()
        {
            GetPersistentPathValue val = new GetPersistentPathValue();
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;

            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            m_HaveValue = true;
            m_Value = UnityEngine.Application.persistentDataPath;
        }
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CallScriptValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
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
                        for (int i = 1; i < num; ++i) {
                            var arg = new StoryValue();
                            arg.InitFromDsl(callData.GetParam(i));
                            m_Args.Add(arg);
                        }
                    }
                }
            }
        }
        public IStoryValue Clone()
        {
            CallScriptValue val = new CallScriptValue();
            val.m_FuncName = m_FuncName;
            if (string.IsNullOrEmpty(m_FuncName))
                val.m_Func = m_Func.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue varg = m_Args[i];
                val.m_Args.Add(varg.Clone());
            }
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            if (string.IsNullOrEmpty(m_FuncName))
                m_Func.Evaluate(instance, handler, iterator, args);
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryValue val = m_Args[i];
                val.Evaluate(instance, handler, iterator, args);
            }
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            string funcName = m_FuncName;
            m_HaveValue = true;
            if (string.IsNullOrEmpty(m_FuncName) && m_Func.HaveValue) {
                funcName = m_Func.Value;
            }
            if (!string.IsNullOrEmpty(funcName)) {
                ArrayList al = new ArrayList();
                foreach (var varg in m_Args) {
                    al.Add(varg.Value.GetObject());
                }
                m_Value = BoxedValue.FromObject(Main.Call(funcName, al.ToArray()));
            }
            else {
                m_Value = BoxedValue.NullObject;
            }
        }

        private string m_FuncName = string.Empty;
        private IStoryValue<string> m_Func = new StoryValue<string>();
        private List<IStoryValue> m_Args = new List<IStoryValue>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class EvalScriptValue : IStoryValue
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
                int num = callData.GetParamNum();
                if (num > 0) {
                    var vd = callData.GetParam(0) as Dsl.ValueData;
                    if (null != vd && vd.IsString()) {
                        m_IsString = true;
                        m_Code.InitFromDsl(vd);
                    }
                    else {
                        m_IsString = false;
                        foreach (var p in callData.Params) {
                            m_Exps.Add(p);
                        }
                    }
                }
            }
        }
        public IStoryValue Clone()
        {
            EvalScriptValue val = new EvalScriptValue();
            val.m_IsString = m_IsString;
            if (m_IsString)
                val.m_Code = m_Code.Clone();
            val.m_Exps = m_Exps;
            val.m_HaveValue = m_HaveValue;
            val.m_Value = m_Value;
            return val;
        }
        public void Evaluate(StoryInstance instance, StoryMessageHandler handler, BoxedValue iterator, BoxedValueList args)
        {
            m_HaveValue = false;
            if (m_IsString)
                m_Code.Evaluate(instance, handler, iterator, args);
            TryUpdateValue(instance);
        }
        public bool HaveValue {
            get {
                return m_HaveValue;
            }
        }
        public BoxedValue Value {
            get {
                return m_Value;
            }
        }
        private void TryUpdateValue(StoryInstance instance)
        {
            if (m_Code.HaveValue) {
                m_HaveValue = true;
                if (m_IsString) {
                    var code = m_Code.Value;
                    var r = Main.EvalAndRun(code);
                    m_Value = VariantValue.ToBoxedValue(r);
                }
                else {
                    var r = Main.EvalAndRun(m_Exps);
                    m_Value = VariantValue.ToBoxedValue(r);
                }
            }
        }

        private bool m_IsString = false;
        private IStoryValue<string> m_Code = new StoryValue<string>();
        private List<ISyntaxComponent> m_Exps = new List<ISyntaxComponent>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
}
