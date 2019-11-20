using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavaClass
{
    public JavaClass(string _class)
    {
        m_Class = _class;
        m_AndroidJavaClass = new AndroidJavaClass(_class);
    }
    public object Call(string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        try {
            ret = m_AndroidJavaClass.Call<object>(method, _args);
        }
        catch {
            m_AndroidJavaClass.Call(method, _args);
        }
        return ret;
    }
    public object Get(string prop)
    {
        return m_AndroidJavaClass.Get<object>(prop);
    }
    public void Set(string prop, object val)
    {
        m_AndroidJavaClass.Set<object>(prop, val);
    }
    public object CallStatic(string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        try {
            ret = m_AndroidJavaClass.CallStatic<object>(method, _args);
        }
        catch {
            m_AndroidJavaClass.CallStatic(method, _args);
        }
        return ret;
    }
    public object GetStatic(string prop)
    {
        return m_AndroidJavaClass.GetStatic<object>(prop);
    }
    public void SetStatic(string prop, object val)
    {
        m_AndroidJavaClass.SetStatic<object>(prop, val);
    }
    public AndroidJavaClass GetClass()
    {
        return m_AndroidJavaClass;
    }

    private string m_Class;
    private AndroidJavaClass m_AndroidJavaClass;
}

public class JavaObject
{
    public JavaObject(string _class, params object[] args)
    {
        m_Class = _class;
        m_AndroidJavaObject = new AndroidJavaObject(_class, args);
    }
    public object Call(string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        try {
            ret = m_AndroidJavaObject.Call<object>(method, _args);
        }
        catch {
            m_AndroidJavaObject.Call(method, _args);
        }
        return ret;
    }
    public object Get(string prop)
    {
        return m_AndroidJavaObject.Get<object>(prop);
    }
    public void Set(string prop, object val)
    {
        m_AndroidJavaObject.Set<object>(prop, val);
    }
    public object CallStatic(string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        try {
            ret = m_AndroidJavaObject.CallStatic<object>(method, _args);
        }
        catch {
            m_AndroidJavaObject.CallStatic(method, _args);
        }
        return ret;
    }
    public object GetStatic(string prop)
    {
        return m_AndroidJavaObject.GetStatic<object>(prop);
    }
    public void SetStatic(string prop, object val)
    {
        m_AndroidJavaObject.SetStatic<object>(prop, val);
    }
    public AndroidJavaObject GetObject()
    {
        return m_AndroidJavaObject;
    }

    private string m_Class;
    private AndroidJavaObject m_AndroidJavaObject;
}

public class JavaProxy : AndroidJavaProxy
{
    public override AndroidJavaObject Invoke(string methodName, object[] args)
    {
        var al = new ArrayList(args);
        al.Insert(0, methodName);
        var r = Main.Call(m_InvokeMethod, al.ToArray()) as JavaObject;
        if (null != r) {
            return r.GetObject();
        }
        return null;
    }
    public override AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
    {
        var al = new ArrayList(javaArgs);
        al.Insert(0, methodName);
        var r = Main.Call(m_InvokeMethod, al.ToArray()) as JavaObject;
        if (null != r) {
            return r.GetObject();
        }
        return null;
    }
    public JavaProxy(string _class, string invokeMethod):base(new AndroidJavaClass(_class))
    {
        m_InvokeMethod = invokeMethod;
    }

    private string m_InvokeMethod;
}