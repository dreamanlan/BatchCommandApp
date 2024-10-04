﻿using System;
using System.Collections.Generic;
using StoryScript;
using System.IO;
using System.Collections;
using Dsl;

namespace GmCommands
{
    internal sealed class GetTimeFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetTimeFunction val = new GetTimeFunction();
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
    internal sealed class GetTimeScaleFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetTimeScaleFunction val = new GetTimeScaleFunction();
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
    internal sealed class GetTimeSinceStartupValue : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
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
    internal sealed class IsActiveFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            IsActiveFunction val = new IsActiveFunction();
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsReallyActiveFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            IsReallyActiveFunction val = new IsReallyActiveFunction();
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class IsVisibleFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            IsVisibleFunction val = new IsVisibleFunction();
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentFunction val = new GetComponentFunction();
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentInParentFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentInParentFunction val = new GetComponentInParentFunction();
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
                        UnityEngine.Component component = obj.GetComponentInParent(t, includeInactive != 0);
                        m_Value = component;
                    }
                    else {
                        string name = componentType.IsString ? componentType.StringVal : null;
                        if (null != name) {
                            t = StoryScriptUtility.GetType(name);
                            if (null != t) {
                                UnityEngine.Component component = obj.GetComponentInParent(t, includeInactive != 0);
                                m_Value = component;
                            }
                            else {
                                m_Value = BoxedValue.NullObject;
                            }
                        }
                    }
                }
            }
        }

        private int m_ParamNum = 0;
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private IStoryFunction<int> m_IncludeInactive = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentInChildrenFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentInChildrenFunction val = new GetComponentInChildrenFunction();
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
                            t = StoryScriptUtility.GetType(name);
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private IStoryFunction<int> m_IncludeInactive = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentsFunction val = new GetComponentsFunction();
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
                            t = StoryScriptUtility.GetType(name);
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsInParentFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentsInParentFunction val = new GetComponentsInParentFunction();
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
                            t = StoryScriptUtility.GetType(name);
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private IStoryFunction<int> m_IncludeInactive = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetComponentsInChildrenFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetComponentsInChildrenFunction val = new GetComponentsInChildrenFunction();
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
                            t = StoryScriptUtility.GetType(name);
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction m_ComponentType = new StoryFunction();
        private IStoryFunction<int> m_IncludeInactive = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetGameObjectFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData funcData = param as Dsl.FunctionData;
            if (null != funcData) {
                Load(funcData);
            }
        }
        public IStoryFunction Clone()
        {
            GetGameObjectFunction val = new GetGameObjectFunction();
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
                    var p = new StoryFunction<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_DisableComponents.Add(p);
                }
            } else if (id == "remove") {
                for (int i = 0; i < callData.GetParamNum(); ++i) {
                    var p = new StoryFunction<string>();
                    p.InitFromDsl(callData.GetParam(i));
                    m_RemoveComponents.Add(p);
                }
            }
        }

        private IStoryFunction m_ObjPath = new StoryFunction();
        private List<IStoryFunction<string>> m_DisableComponents = new List<IStoryFunction<string>>();
        private List<IStoryFunction<string>> m_RemoveComponents = new List<IStoryFunction<string>>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetParentFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetParentFunction val = new GetParentFunction();
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
        private IStoryFunction m_ObjPath = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetChildFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetChildFunction val = new GetChildFunction();
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
                        var t = StoryScriptUtility.FindChildRecursive(obj.transform, childPath);
                        if (null != t) {
                            m_Value = t.gameObject;
                        } else {
                            m_Value = BoxedValue.NullObject;
                        }
                    } else {
                        m_Value = BoxedValue.NullObject;
                    }
                } else if (null != uobj) {
                    var t = StoryScriptUtility.FindChildRecursive(uobj.transform, childPath);
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

        private IStoryFunction m_ObjPath = new StoryFunction();
        private IStoryFunction<string> m_ChildPath = new StoryFunction<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUnityTypeFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetUnityTypeFunction val = new GetUnityTypeFunction();
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
        private IStoryFunction<string> m_TypeName = new StoryFunction<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUnityUiTypeFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetUnityUiTypeFunction val = new GetUnityUiTypeFunction();
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
        private IStoryFunction<string> m_TypeName = new StoryFunction<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetUserTypeFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetUserTypeFunction val = new GetUserTypeFunction();
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
        private IStoryFunction<string> m_TypeName = new StoryFunction<string>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetPositionFunction val = new GetPositionFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionXFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetPositionXFunction val = new GetPositionXFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionYFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetPositionYFunction val = new GetPositionYFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetPositionZFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetPositionZFunction val = new GetPositionZFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetRotationFunction val = new GetRotationFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationXFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetRotationXFunction val = new GetRotationXFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationYFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetRotationYFunction val = new GetRotationYFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetRotationZFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetRotationZFunction val = new GetRotationZFunction();
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
                int local0OrWorld1 = m_LocalOrWorld.Value;
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
                    if (0 == local0OrWorld1)
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
        private IStoryFunction m_ObjId = new StoryFunction();
        private IStoryFunction<int> m_LocalOrWorld = new StoryFunction<int>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleFunction val = new GetScaleFunction();
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

        private IStoryFunction m_ObjId = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleXFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleXFunction val = new GetScaleXFunction();
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

        private IStoryFunction m_ObjId = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleYFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleYFunction val = new GetScaleYFunction();
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

        private IStoryFunction m_ObjId = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetScaleZFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_ObjId.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            GetScaleZFunction val = new GetScaleZFunction();
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

        private IStoryFunction m_ObjId = new StoryFunction();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class Deg2RadFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Degree.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            Deg2RadFunction val = new Deg2RadFunction();
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

        private IStoryFunction<float> m_Degree = new StoryFunction<float>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class Rad2DegFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData && callData.GetParamNum() == 1) {
                m_Radian.InitFromDsl(callData.GetParam(0));
            }
        }
        public IStoryFunction Clone()
        {
            Rad2DegFunction val = new Rad2DegFunction();
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

        private IStoryFunction<float> m_Radian = new StoryFunction<float>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetFileNameFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetFileNameFunction val = new GetFileNameFunction();
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

        private IStoryFunction<string> m_Path = new StoryFunction<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetDirectoryNameFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetDirectoryNameFunction val = new GetDirectoryNameFunction();
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

        private IStoryFunction<string> m_Path = new StoryFunction<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetExtensionFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            GetExtensionFunction val = new GetExtensionFunction();
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

        private IStoryFunction<string> m_Path = new StoryFunction<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class CombinePathFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            CombinePathFunction val = new CombinePathFunction();
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

        private IStoryFunction<string> m_Path1 = new StoryFunction<string>();
        private IStoryFunction<string> m_Path2 = new StoryFunction<string>();
        private int m_ParamNum;
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class GetStreamingAssetsFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetStreamingAssetsFunction val = new GetStreamingAssetsFunction();
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
    internal sealed class GetPersistentPathFunction : IStoryFunction
    {
        public void InitFromDsl(Dsl.ISyntaxComponent param)
        {
            Dsl.FunctionData callData = param as Dsl.FunctionData;
            if (null != callData) {
            }
        }
        public IStoryFunction Clone()
        {
            GetPersistentPathFunction val = new GetPersistentPathFunction();
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
    internal sealed class CallScriptFunction : IStoryFunction
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
                            StoryFunction val = new StoryFunction();
                            val.InitFromDsl(func.GetParam(i));
                            m_Args.Add(val);
                        }
                    }
                    else {
                        m_Func.InitFromDsl(p0);
                        for (int i = 1; i < num; ++i) {
                            var arg = new StoryFunction();
                            arg.InitFromDsl(callData.GetParam(i));
                            m_Args.Add(arg);
                        }
                    }
                }
            }
        }
        public IStoryFunction Clone()
        {
            CallScriptFunction val = new CallScriptFunction();
            val.m_FuncName = m_FuncName;
            if (string.IsNullOrEmpty(m_FuncName))
                val.m_Func = m_Func.Clone();
            for (int i = 0; i < m_Args.Count; ++i) {
                IStoryFunction varg = m_Args[i];
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
                IStoryFunction val = m_Args[i];
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
        private IStoryFunction<string> m_Func = new StoryFunction<string>();
        private List<IStoryFunction> m_Args = new List<IStoryFunction>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
    internal sealed class EvalScriptFunction : IStoryFunction
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
        public IStoryFunction Clone()
        {
            EvalScriptFunction val = new EvalScriptFunction();
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
                    m_Value = r;
                }
                else {
                    var r = Main.EvalAndRun(m_Exps);
                    m_Value = r;
                }
            }
        }

        private bool m_IsString = false;
        private IStoryFunction<string> m_Code = new StoryFunction<string>();
        private List<ISyntaxComponent> m_Exps = new List<ISyntaxComponent>();
        private bool m_HaveValue;
        private BoxedValue m_Value;
    }
}
