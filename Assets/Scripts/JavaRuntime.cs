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
    public object Call(string method, params object[] args)
    {
        object ret = null;
        try {
            ret = m_AndroidJavaClass.CallStatic<object>(method, args);
        }
        catch {
            m_AndroidJavaClass.CallStatic(method, args);
        }
        return ret;
    }
    public object Get(string prop)
    {
        return m_AndroidJavaClass.GetStatic<object>(prop);
    }
    public void Set(string prop, object val)
    {
        m_AndroidJavaClass.SetStatic<object>(prop, val);
    }

    private string m_Class;
    private AndroidJavaClass m_AndroidJavaClass;
}

public class JavaObject
{
    public JavaObject(string _class)
    {
        m_Class = _class;
        m_AndroidJavaObject = new AndroidJavaObject(_class);
    }
    public object Call(string method, params object[] args)
    {
        object ret = null;
        try {
            ret = m_AndroidJavaObject.Call<object>(method, args);
        }
        catch {
            m_AndroidJavaObject.Call(method, args);
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

    private string m_Class;
    private AndroidJavaObject m_AndroidJavaObject;
}