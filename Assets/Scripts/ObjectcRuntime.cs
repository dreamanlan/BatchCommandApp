using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

enum ArgTypeEnum
{
    Int8 = 0,
    Uint8,
    Int16,
    Uint16,
    Int32,
    Uint32,
    Int64,
    Uint64,
    Bool,
    Float,
    Decimal,
    Double,
    String
}

struct ArgTypeInfo
{
    internal int ArgType;
    internal long LongVal;
    internal double DoubleVal;
    internal string StringVal;

    internal static ArgTypeInfo From(object val)
    {
        var t = val.GetType();
        ArgTypeEnum argtype;
        if(s_Type2ArgTypes.TryGetValue(t, out argtype)){
            switch(argtype){
                case ArgTypeEnum.String:
                    return new ArgTypeInfo { ArgType = (int)argtype, StringVal = val as string };
                case ArgTypeEnum.Float:
                case ArgTypeEnum.Decimal:
                case ArgTypeEnum.Double:
                    return new ArgTypeInfo { ArgType = (int)argtype, DoubleVal = (double)System.Convert.ChangeType(val, typeof(double)) };
                default:
                    return new ArgTypeInfo { ArgType = (int)argtype, LongVal = (long)System.Convert.ChangeType(val, typeof(long)) };
            }
        }else{
            return new ArgTypeInfo { ArgType=(int)ArgTypeEnum.Int32, LongVal = (long)System.Convert.ChangeType(val, typeof(long)) };
        }
    }

    static Dictionary<System.Type, ArgTypeEnum> s_Type2ArgTypes = new Dictionary<System.Type, ArgTypeEnum> {
        {typeof(sbyte), ArgTypeEnum.Int8},
        {typeof(byte), ArgTypeEnum.Uint8},
        {typeof(short), ArgTypeEnum.Int16},
        {typeof(ushort), ArgTypeEnum.Uint16},
        {typeof(int), ArgTypeEnum.Int32},
        {typeof(uint), ArgTypeEnum.Uint32},
        {typeof(long), ArgTypeEnum.Int64},
        {typeof(ulong), ArgTypeEnum.Uint64},
        {typeof(float), ArgTypeEnum.Float},
        {typeof(decimal), ArgTypeEnum.Decimal},
        {typeof(double), ArgTypeEnum.Double},
        {typeof(string), ArgTypeEnum.String}
    };
}

public class ObjectcClass
{
    public ObjectcClass(string _class)
    {
        m_Class = _class;
    }
    public int Call(string method, IList args)
    {
#if UNITY_IOS
        int len = args.Count;
        var vargs = new ArgTypeInfo[len];
        for(int i=0; i<len; ++i){
            vargs[i] = ArgTypeInfo.From(args[i]);
        }
        return CallClass(m_Class, method, vargs, len);
#else
        return 0;
#endif
    }
    public object GetValue(int id)
    {
#if UNITY_IOS
        ArgTypeInfo info = ObjectGet(id);
        switch (info.ArgType){
            case (int)ArgTypeEnum.String:
                return info.StringVal;
            case (int)ArgTypeEnum.Float:
            case (int)ArgTypeEnum.Decimal:
            case (int)ArgTypeEnum.Double:
                return info.DoubleVal;
            default:
                return info.LongVal;
        }
#else
        return null;
#endif
    }
    public int NewCallback()
    {
#if UNITY_IOS
        return NewDummyObject();
#else
        return 0;
#endif
    }
    public void CopyFile(string tempName)
    {
#if UNITY_IOS
        PickFile(tempName);
#endif
    }

    private string m_Class;

#if UNITY_IOS
    [DllImport("__Internal")]
    static extern int CallClass(string _class, string method, ArgTypeInfo[] args, int num);
    [DllImport("__Internal")]
    static extern ArgTypeInfo ObjectGet(int objId);
    [DllImport("__Internal")]
    static extern int NewDummyObject();
    [DllImport("__Internal")]
    static extern void PickFile(string tempName);
#endif
}

public class ObjectcObject
{
    public ObjectcObject(int objId)
    {
        m_ObjId = objId;
    }
    public int Call(string method, IList args)
    {
#if UNITY_IOS
        int len = args.Count;
        var vargs = new ArgTypeInfo[len];
        for(int i=0; i<len; ++i){
            vargs[i] = ArgTypeInfo.From(args[i]);
        }
        return CallInstance(m_ObjId, method, vargs, len);
#else
        return 0;
#endif
    }
    public object GetValue(int id)
    {
#if UNITY_IOS
        ArgTypeInfo info = ObjectGet(id);
        switch (info.ArgType){
            case (int)ArgTypeEnum.String:
                return info.StringVal;
            case (int)ArgTypeEnum.Float:
            case (int)ArgTypeEnum.Decimal:
            case (int)ArgTypeEnum.Double:
                return info.DoubleVal;
            default:
                return info.LongVal;
        }
#else
        return null;
#endif
    }
    public int NewCallback()
    {
#if UNITY_IOS
        return NewDummyObject();
#else
        return 0;
#endif
    }
    public void CopyFile(string tempName)
    {
#if UNITY_IOS
        PickFile(tempName);
#endif
    }

    private int m_ObjId;

#if UNITY_IOS
    [DllImport("__Internal")]
    static extern int CallInstance(int objId, string method, ArgTypeInfo[] args, int num);
    [DllImport("__Internal")]
    static extern ArgTypeInfo ObjectGet(int objId);
    [DllImport("__Internal")]
    static extern int NewDummyObject();
    [DllImport("__Internal")]
    static extern void PickFile(string tempName);
#endif
}