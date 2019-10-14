using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectcClass
{
    public ObjectcClass(string _class)
    {
        m_Class = _class;
    }
    public int Call(string method, params object[] args)
    {
#if UNITY_IOS
        int len = args.Length;
        return CallClass(m_Class, method, args, len);
#else
        return 0;
#endif
    }

    private string m_Class;

#if UNITY_IOS
    [DllImport("__Internal")]
    static extern int CallClass(string _class, string method, object[] args, int num);
#endif
}

public class ObjectcObject
{
    public ObjectcObject(int objId)
    {
        m_ObjId = objId;
    }
    public int Call(string method, params object[] args)
    {
#if UNITY_IOS
        int len = args.Length;
        return CallInstance(m_ObjId, method, args, len);
#else
        return 0;
#endif
    }

    private int m_ObjId;

#if UNITY_IOS
    [DllImport("__Internal")]
    static extern int CallInstance(int objId, string method, ArgInfo[] args, int num);
#endif
}