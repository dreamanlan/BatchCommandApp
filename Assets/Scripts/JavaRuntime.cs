using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavaClass
{
    public JavaClass(AndroidJavaClass obj)
    {
        m_Class = obj.ToString();
        m_AndroidJavaClass = obj;
    }
    public JavaClass(string _class)
    {
        m_Class = _class;
        m_AndroidJavaClass = new AndroidJavaClass(_class);
    }
    public object Call(string type, string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        if (type == "sbyte")
            ret = m_AndroidJavaClass.Call<sbyte>(method, _args);
        else if (type == "byte")
            ret = m_AndroidJavaClass.Call<byte>(method, _args);
        else if (type == "bool")
            ret = m_AndroidJavaClass.Call<bool>(method, _args);
        else if (type == "char")
            ret = m_AndroidJavaClass.Call<char>(method, _args);
        else if (type == "short")
            ret = m_AndroidJavaClass.Call<short>(method, _args);
        else if (type == "ushort")
            ret = m_AndroidJavaClass.Call<ushort>(method, _args);
        else if (type == "int")
            ret = m_AndroidJavaClass.Call<int>(method, _args);
        else if (type == "uint")
            ret = m_AndroidJavaClass.Call<uint>(method, _args);
        else if (type == "long")
            ret = m_AndroidJavaClass.Call<long>(method, _args);
        else if (type == "ulong")
            ret = m_AndroidJavaClass.Call<ulong>(method, _args);
        else if (type == "float")
            ret = m_AndroidJavaClass.Call<float>(method, _args);
        else if (type == "double")
            ret = m_AndroidJavaClass.Call<double>(method, _args);
        else if (type == "string")
            ret = m_AndroidJavaClass.Call<string>(method, _args);
        else if (type == "object")
            ret = m_AndroidJavaClass.Call<object>(method, _args);
        else if (type == "array")
            ret = m_AndroidJavaClass.Call<object[]>(method, _args);
        else if (type == "androidjavaclass")
            ret = m_AndroidJavaClass.Call<AndroidJavaClass>(method, _args);
        else if (type == "androidjavaobject")
            ret = m_AndroidJavaClass.Call<AndroidJavaObject>(method, _args);
        else
            m_AndroidJavaClass.Call(method, _args);
        return ret;
    }
    public object Get(string type, string prop)
    {
        if (type == "sbyte")
            return m_AndroidJavaClass.Get<sbyte>(prop);
        else if (type == "byte")
            return m_AndroidJavaClass.Get<byte>(prop);
        else if (type == "bool")
            return m_AndroidJavaClass.Get<bool>(prop);
        else if (type == "char")
            return m_AndroidJavaClass.Get<char>(prop);
        else if (type == "short")
            return m_AndroidJavaClass.Get<short>(prop);
        else if (type == "ushort")
            return m_AndroidJavaClass.Get<ushort>(prop);
        else if (type == "int")
            return m_AndroidJavaClass.Get<int>(prop);
        else if (type == "uint")
            return m_AndroidJavaClass.Get<uint>(prop);
        else if (type == "long")
            return m_AndroidJavaClass.Get<long>(prop);
        else if (type == "ulong")
            return m_AndroidJavaClass.Get<ulong>(prop);
        else if (type == "float")
            return m_AndroidJavaClass.Get<float>(prop);
        else if (type == "double")
            return m_AndroidJavaClass.Get<double>(prop);
        else if (type == "string")
            return m_AndroidJavaClass.Get<string>(prop);
        else if (type == "array")
            return m_AndroidJavaClass.Get<object[]>(prop);
        else if (type == "androidjavaclass")
            return m_AndroidJavaClass.Get<AndroidJavaClass>(prop);
        else if (type == "androidjavaobject")
            return m_AndroidJavaClass.Get<AndroidJavaObject>(prop);
        else
            return m_AndroidJavaClass.Get<object>(prop);
    }
    public void Set(string type, string prop, object val)
    {
        if (type == "sbyte")
            m_AndroidJavaClass.Set<sbyte>(prop, (sbyte)System.Convert.ChangeType(val, typeof(sbyte)));
        else if (type == "byte")
            m_AndroidJavaClass.Set<byte>(prop, (byte)System.Convert.ChangeType(val, typeof(byte)));
        else if (type == "bool")
            m_AndroidJavaClass.Set<bool>(prop, (bool)System.Convert.ChangeType(val, typeof(bool)));
        else if (type == "char")
            m_AndroidJavaClass.Set<char>(prop, (char)System.Convert.ChangeType(val, typeof(char)));
        else if (type == "short")
            m_AndroidJavaClass.Set<short>(prop, (short)System.Convert.ChangeType(val, typeof(short)));
        else if (type == "ushort")
            m_AndroidJavaClass.Set<ushort>(prop, (ushort)System.Convert.ChangeType(val, typeof(ushort)));
        else if (type == "int")
            m_AndroidJavaClass.Set<int>(prop, (int)System.Convert.ChangeType(val, typeof(int)));
        else if (type == "uint")
            m_AndroidJavaClass.Set<uint>(prop, (uint)System.Convert.ChangeType(val, typeof(uint)));
        else if (type == "long")
            m_AndroidJavaClass.Set<long>(prop, (long)System.Convert.ChangeType(val, typeof(long)));
        else if (type == "ulong")
            m_AndroidJavaClass.Set<ulong>(prop, (ulong)System.Convert.ChangeType(val, typeof(ulong)));
        else if (type == "float")
            m_AndroidJavaClass.Set<float>(prop, (float)System.Convert.ChangeType(val, typeof(float)));
        else if (type == "double")
            m_AndroidJavaClass.Set<double>(prop, (double)System.Convert.ChangeType(val, typeof(double)));
        else if (type == "string")
            m_AndroidJavaClass.Set<string>(prop, val as string);
        else if (type == "array")
            m_AndroidJavaClass.Set<object[]>(prop, val as object[]);
        else if (type == "androidjavaclass")
            m_AndroidJavaClass.Set<AndroidJavaClass>(prop, val as AndroidJavaClass);
        else if (type == "androidjavaobject")
            m_AndroidJavaClass.Set<AndroidJavaObject>(prop, val as AndroidJavaObject);
        else if (type == "javaclass")
            m_AndroidJavaClass.Set<AndroidJavaClass>(prop, (val as JavaClass).GetClass());
        else if (type == "javaobject")
            m_AndroidJavaClass.Set<AndroidJavaObject>(prop, (val as JavaObject).GetObject());
        else
            m_AndroidJavaClass.Set<object>(prop, val);
    }
    public object CallStatic(string type, string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        if (type == "sbyte")
            ret = m_AndroidJavaClass.CallStatic<sbyte>(method, _args);
        else if (type == "byte")
            ret = m_AndroidJavaClass.CallStatic<byte>(method, _args);
        else if (type == "bool")
            ret = m_AndroidJavaClass.CallStatic<bool>(method, _args);
        else if (type == "char")
            ret = m_AndroidJavaClass.CallStatic<char>(method, _args);
        else if (type == "short")
            ret = m_AndroidJavaClass.CallStatic<short>(method, _args);
        else if (type == "ushort")
            ret = m_AndroidJavaClass.CallStatic<ushort>(method, _args);
        else if (type == "int")
            ret = m_AndroidJavaClass.CallStatic<int>(method, _args);
        else if (type == "uint")
            ret = m_AndroidJavaClass.CallStatic<uint>(method, _args);
        else if (type == "long")
            ret = m_AndroidJavaClass.CallStatic<long>(method, _args);
        else if (type == "ulong")
            ret = m_AndroidJavaClass.CallStatic<ulong>(method, _args);
        else if (type == "float")
            ret = m_AndroidJavaClass.CallStatic<float>(method, _args);
        else if (type == "double")
            ret = m_AndroidJavaClass.CallStatic<double>(method, _args);
        else if (type == "string")
            ret = m_AndroidJavaClass.CallStatic<string>(method, _args);
        else if (type == "object")
            ret = m_AndroidJavaClass.CallStatic<object>(method, _args);
        else if (type == "array")
            ret = m_AndroidJavaClass.CallStatic<object[]>(method, _args);
        else if (type == "androidjavaclass")
            ret = m_AndroidJavaClass.CallStatic<AndroidJavaClass>(method, _args);
        else if (type == "androidjavaobject")
            ret = m_AndroidJavaClass.CallStatic<AndroidJavaObject>(method, _args);
        else
            m_AndroidJavaClass.CallStatic(method, _args);
        return ret;
    }
    public object GetStatic(string type, string prop)
    {
        if (type == "sbyte")
            return m_AndroidJavaClass.GetStatic<sbyte>(prop);
        else if (type == "byte")
            return m_AndroidJavaClass.GetStatic<byte>(prop);
        else if (type == "bool")
            return m_AndroidJavaClass.GetStatic<bool>(prop);
        else if (type == "char")
            return m_AndroidJavaClass.GetStatic<char>(prop);
        else if (type == "short")
            return m_AndroidJavaClass.GetStatic<short>(prop);
        else if (type == "ushort")
            return m_AndroidJavaClass.GetStatic<ushort>(prop);
        else if (type == "int")
            return m_AndroidJavaClass.GetStatic<int>(prop);
        else if (type == "uint")
            return m_AndroidJavaClass.GetStatic<uint>(prop);
        else if (type == "long")
            return m_AndroidJavaClass.GetStatic<long>(prop);
        else if (type == "ulong")
            return m_AndroidJavaClass.GetStatic<ulong>(prop);
        else if (type == "float")
            return m_AndroidJavaClass.GetStatic<float>(prop);
        else if (type == "double")
            return m_AndroidJavaClass.GetStatic<double>(prop);
        else if (type == "string")
            return m_AndroidJavaClass.GetStatic<string>(prop);
        else if (type == "array")
            return m_AndroidJavaClass.GetStatic<object[]>(prop);
        else if (type == "androidjavaclass")
            return m_AndroidJavaClass.GetStatic<AndroidJavaClass>(prop);
        else if (type == "androidjavaobject")
            return m_AndroidJavaClass.GetStatic<AndroidJavaObject>(prop);
        else
            return m_AndroidJavaClass.GetStatic<object>(prop);
    }
    public void SetStatic(string type, string prop, object val)
    {
        if (type == "sbyte")
            m_AndroidJavaClass.SetStatic<sbyte>(prop, (sbyte)System.Convert.ChangeType(val, typeof(sbyte)));
        else if (type == "byte")
            m_AndroidJavaClass.SetStatic<byte>(prop, (byte)System.Convert.ChangeType(val, typeof(byte)));
        else if (type == "bool")
            m_AndroidJavaClass.SetStatic<bool>(prop, (bool)System.Convert.ChangeType(val, typeof(bool)));
        else if (type == "char")
            m_AndroidJavaClass.SetStatic<char>(prop, (char)System.Convert.ChangeType(val, typeof(char)));
        else if (type == "short")
            m_AndroidJavaClass.SetStatic<short>(prop, (short)System.Convert.ChangeType(val, typeof(short)));
        else if (type == "ushort")
            m_AndroidJavaClass.SetStatic<ushort>(prop, (ushort)System.Convert.ChangeType(val, typeof(ushort)));
        else if (type == "int")
            m_AndroidJavaClass.SetStatic<int>(prop, (int)System.Convert.ChangeType(val, typeof(int)));
        else if (type == "uint")
            m_AndroidJavaClass.SetStatic<uint>(prop, (uint)System.Convert.ChangeType(val, typeof(uint)));
        else if (type == "long")
            m_AndroidJavaClass.SetStatic<long>(prop, (long)System.Convert.ChangeType(val, typeof(long)));
        else if (type == "ulong")
            m_AndroidJavaClass.SetStatic<ulong>(prop, (ulong)System.Convert.ChangeType(val, typeof(ulong)));
        else if (type == "float")
            m_AndroidJavaClass.SetStatic<float>(prop, (float)System.Convert.ChangeType(val, typeof(float)));
        else if (type == "double")
            m_AndroidJavaClass.SetStatic<double>(prop, (double)System.Convert.ChangeType(val, typeof(double)));
        else if (type == "string")
            m_AndroidJavaClass.SetStatic<string>(prop, val as string);
        else if (type == "array")
            m_AndroidJavaClass.SetStatic<object[]>(prop, val as object[]);
        else if (type == "androidjavaclass")
            m_AndroidJavaClass.SetStatic<AndroidJavaClass>(prop, val as AndroidJavaClass);
        else if (type == "androidjavaobject")
            m_AndroidJavaClass.SetStatic<AndroidJavaObject>(prop, val as AndroidJavaObject);
        else if (type == "javaclass")
            m_AndroidJavaClass.SetStatic<AndroidJavaClass>(prop, (val as JavaClass).GetClass());
        else if (type == "javaobject")
            m_AndroidJavaClass.SetStatic<AndroidJavaObject>(prop, (val as JavaObject).GetObject());
        else
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
    public JavaObject(AndroidJavaObject obj)
    {
        m_Class = obj.ToString();
        m_AndroidJavaObject = obj;
    }
    public JavaObject(string _class, params object[] args)
    {
        m_Class = _class;
        m_AndroidJavaObject = new AndroidJavaObject(_class, args);
    }
    public object Call(string type, string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        if (type == "sbyte")
            ret = m_AndroidJavaObject.Call<sbyte>(method, _args);
        else if (type == "byte")
            ret = m_AndroidJavaObject.Call<byte>(method, _args);
        else if (type == "bool")
            ret = m_AndroidJavaObject.Call<bool>(method, _args);
        else if (type == "char")
            ret = m_AndroidJavaObject.Call<char>(method, _args);
        else if (type == "short")
            ret = m_AndroidJavaObject.Call<short>(method, _args);
        else if (type == "ushort")
            ret = m_AndroidJavaObject.Call<ushort>(method, _args);
        else if (type == "int")
            ret = m_AndroidJavaObject.Call<int>(method, _args);
        else if (type == "uint")
            ret = m_AndroidJavaObject.Call<uint>(method, _args);
        else if (type == "long")
            ret = m_AndroidJavaObject.Call<long>(method, _args);
        else if (type == "ulong")
            ret = m_AndroidJavaObject.Call<ulong>(method, _args);
        else if (type == "float")
            ret = m_AndroidJavaObject.Call<float>(method, _args);
        else if (type == "double")
            ret = m_AndroidJavaObject.Call<double>(method, _args);
        else if (type == "string")
            ret = m_AndroidJavaObject.Call<string>(method, _args);
        else if (type == "object")
            ret = m_AndroidJavaObject.Call<object>(method, _args);
        else if (type == "array")
            ret = m_AndroidJavaObject.Call<object[]>(method, _args);
        else if (type == "androidjavaclass")
            ret = m_AndroidJavaObject.Call<AndroidJavaClass>(method, _args);
        else if (type == "androidjavaobject")
            ret = m_AndroidJavaObject.Call<AndroidJavaObject>(method, _args);
        else
            m_AndroidJavaObject.Call(method, _args);
        return ret;
    }
    public object Get(string type, string prop)
    {
        if (type == "sbyte")
            return m_AndroidJavaObject.Get<sbyte>(prop);
        else if (type == "byte")
            return m_AndroidJavaObject.Get<byte>(prop);
        else if (type == "bool")
            return m_AndroidJavaObject.Get<bool>(prop);
        else if (type == "char")
            return m_AndroidJavaObject.Get<char>(prop);
        else if (type == "short")
            return m_AndroidJavaObject.Get<short>(prop);
        else if (type == "ushort")
            return m_AndroidJavaObject.Get<ushort>(prop);
        else if (type == "int")
            return m_AndroidJavaObject.Get<int>(prop);
        else if (type == "uint")
            return m_AndroidJavaObject.Get<uint>(prop);
        else if (type == "long")
            return m_AndroidJavaObject.Get<long>(prop);
        else if (type == "ulong")
            return m_AndroidJavaObject.Get<ulong>(prop);
        else if (type == "float")
            return m_AndroidJavaObject.Get<float>(prop);
        else if (type == "double")
            return m_AndroidJavaObject.Get<double>(prop);
        else if (type == "string")
            return m_AndroidJavaObject.Get<string>(prop);
        else if (type == "array")
            return m_AndroidJavaObject.Get<object[]>(prop);
        else if (type == "androidjavaclass")
            return m_AndroidJavaObject.Get<AndroidJavaClass>(prop);
        else if (type == "androidjavaobject")
            return m_AndroidJavaObject.Get<AndroidJavaObject>(prop);
        else
            return m_AndroidJavaObject.Get<object>(prop);
    }
    public void Set(string type, string prop, object val)
    {
        if (type == "sbyte")
            m_AndroidJavaObject.Set<sbyte>(prop, (sbyte)System.Convert.ChangeType(val, typeof(sbyte)));
        else if (type == "byte")
            m_AndroidJavaObject.Set<byte>(prop, (byte)System.Convert.ChangeType(val, typeof(byte)));
        else if (type == "bool")
            m_AndroidJavaObject.Set<bool>(prop, (bool)System.Convert.ChangeType(val, typeof(bool)));
        else if (type == "char")
            m_AndroidJavaObject.Set<char>(prop, (char)System.Convert.ChangeType(val, typeof(char)));
        else if (type == "short")
            m_AndroidJavaObject.Set<short>(prop, (short)System.Convert.ChangeType(val, typeof(short)));
        else if (type == "ushort")
            m_AndroidJavaObject.Set<ushort>(prop, (ushort)System.Convert.ChangeType(val, typeof(ushort)));
        else if (type == "int")
            m_AndroidJavaObject.Set<int>(prop, (int)System.Convert.ChangeType(val, typeof(int)));
        else if (type == "uint")
            m_AndroidJavaObject.Set<uint>(prop, (uint)System.Convert.ChangeType(val, typeof(uint)));
        else if (type == "long")
            m_AndroidJavaObject.Set<long>(prop, (long)System.Convert.ChangeType(val, typeof(long)));
        else if (type == "ulong")
            m_AndroidJavaObject.Set<ulong>(prop, (ulong)System.Convert.ChangeType(val, typeof(ulong)));
        else if (type == "float")
            m_AndroidJavaObject.Set<float>(prop, (float)System.Convert.ChangeType(val, typeof(float)));
        else if (type == "double")
            m_AndroidJavaObject.Set<double>(prop, (double)System.Convert.ChangeType(val, typeof(double)));
        else if (type == "string")
            m_AndroidJavaObject.Set<string>(prop, val as string);
        else if (type == "array")
            m_AndroidJavaObject.Set<object[]>(prop, val as object[]);
        else if (type == "androidjavaclass")
            m_AndroidJavaObject.Set<AndroidJavaClass>(prop, val as AndroidJavaClass);
        else if (type == "androidjavaobject")
            m_AndroidJavaObject.Set<AndroidJavaObject>(prop, val as AndroidJavaObject);
        else if (type == "javaclass")
            m_AndroidJavaObject.Set<AndroidJavaClass>(prop, (val as JavaClass).GetClass());
        else if (type == "javaobject")
            m_AndroidJavaObject.Set<AndroidJavaObject>(prop, (val as JavaObject).GetObject());
        else
            m_AndroidJavaObject.Set<object>(prop, val);
    }
    public object CallStatic(string type, string method, IList args)
    {
        object[] _args = new object[args.Count];
        args.CopyTo(_args, 0);
        object ret = null;
        if (type == "sbyte")
            ret = m_AndroidJavaObject.CallStatic<sbyte>(method, _args);
        else if (type == "byte")
            ret = m_AndroidJavaObject.CallStatic<byte>(method, _args);
        else if (type == "bool")
            ret = m_AndroidJavaObject.CallStatic<bool>(method, _args);
        else if (type == "char")
            ret = m_AndroidJavaObject.CallStatic<char>(method, _args);
        else if (type == "short")
            ret = m_AndroidJavaObject.CallStatic<short>(method, _args);
        else if (type == "ushort")
            ret = m_AndroidJavaObject.CallStatic<ushort>(method, _args);
        else if (type == "int")
            ret = m_AndroidJavaObject.CallStatic<int>(method, _args);
        else if (type == "uint")
            ret = m_AndroidJavaObject.CallStatic<uint>(method, _args);
        else if (type == "long")
            ret = m_AndroidJavaObject.CallStatic<long>(method, _args);
        else if (type == "ulong")
            ret = m_AndroidJavaObject.CallStatic<ulong>(method, _args);
        else if (type == "float")
            ret = m_AndroidJavaObject.CallStatic<float>(method, _args);
        else if (type == "double")
            ret = m_AndroidJavaObject.CallStatic<double>(method, _args);
        else if (type == "string")
            ret = m_AndroidJavaObject.CallStatic<string>(method, _args);
        else if (type == "object")
            ret = m_AndroidJavaObject.CallStatic<object>(method, _args);
        else if (type == "array")
            ret = m_AndroidJavaObject.CallStatic<object[]>(method, _args);
        else if (type == "androidjavaclass")
            ret = m_AndroidJavaObject.CallStatic<AndroidJavaClass>(method, _args);
        else if (type == "androidjavaobject")
            ret = m_AndroidJavaObject.CallStatic<AndroidJavaObject>(method, _args);
        else
            m_AndroidJavaObject.CallStatic(method, _args);
        return ret;
    }
    public object GetStatic(string type, string prop)
    {
        if (type == "sbyte")
            return m_AndroidJavaObject.GetStatic<sbyte>(prop);
        else if (type == "byte")
            return m_AndroidJavaObject.GetStatic<byte>(prop);
        else if (type == "bool")
            return m_AndroidJavaObject.GetStatic<bool>(prop);
        else if (type == "char")
            return m_AndroidJavaObject.GetStatic<char>(prop);
        else if (type == "short")
            return m_AndroidJavaObject.GetStatic<short>(prop);
        else if (type == "ushort")
            return m_AndroidJavaObject.GetStatic<ushort>(prop);
        else if (type == "int")
            return m_AndroidJavaObject.GetStatic<int>(prop);
        else if (type == "uint")
            return m_AndroidJavaObject.GetStatic<uint>(prop);
        else if (type == "long")
            return m_AndroidJavaObject.GetStatic<long>(prop);
        else if (type == "ulong")
            return m_AndroidJavaObject.GetStatic<ulong>(prop);
        else if (type == "float")
            return m_AndroidJavaObject.GetStatic<float>(prop);
        else if (type == "double")
            return m_AndroidJavaObject.GetStatic<double>(prop);
        else if (type == "string")
            return m_AndroidJavaObject.GetStatic<string>(prop);
        else if (type == "array")
            return m_AndroidJavaObject.GetStatic<object[]>(prop);
        else if (type == "androidjavaclass")
            return m_AndroidJavaObject.GetStatic<AndroidJavaClass>(prop);
        else if (type == "androidjavaobject")
            return m_AndroidJavaObject.GetStatic<AndroidJavaObject>(prop);
        else
            return m_AndroidJavaObject.GetStatic<object>(prop);
    }
    public void SetStatic(string type, string prop, object val)
    {
        if (type == "sbyte")
            m_AndroidJavaObject.SetStatic<sbyte>(prop, (sbyte)System.Convert.ChangeType(val, typeof(sbyte)));
        else if (type == "byte")
            m_AndroidJavaObject.SetStatic<byte>(prop, (byte)System.Convert.ChangeType(val, typeof(byte)));
        else if (type == "bool")
            m_AndroidJavaObject.SetStatic<bool>(prop, (bool)System.Convert.ChangeType(val, typeof(bool)));
        else if (type == "char")
            m_AndroidJavaObject.SetStatic<char>(prop, (char)System.Convert.ChangeType(val, typeof(char)));
        else if (type == "short")
            m_AndroidJavaObject.SetStatic<short>(prop, (short)System.Convert.ChangeType(val, typeof(short)));
        else if (type == "ushort")
            m_AndroidJavaObject.SetStatic<ushort>(prop, (ushort)System.Convert.ChangeType(val, typeof(ushort)));
        else if (type == "int")
            m_AndroidJavaObject.SetStatic<int>(prop, (int)System.Convert.ChangeType(val, typeof(int)));
        else if (type == "uint")
            m_AndroidJavaObject.SetStatic<uint>(prop, (uint)System.Convert.ChangeType(val, typeof(uint)));
        else if (type == "long")
            m_AndroidJavaObject.SetStatic<long>(prop, (long)System.Convert.ChangeType(val, typeof(long)));
        else if (type == "ulong")
            m_AndroidJavaObject.SetStatic<ulong>(prop, (ulong)System.Convert.ChangeType(val, typeof(ulong)));
        else if (type == "float")
            m_AndroidJavaObject.SetStatic<float>(prop, (float)System.Convert.ChangeType(val, typeof(float)));
        else if (type == "double")
            m_AndroidJavaObject.SetStatic<double>(prop, (double)System.Convert.ChangeType(val, typeof(double)));
        else if (type == "string")
            m_AndroidJavaObject.SetStatic<string>(prop, val as string);
        else if (type == "array")
            m_AndroidJavaObject.SetStatic<object[]>(prop, val as object[]);
        else if (type == "androidjavaclass")
            m_AndroidJavaObject.SetStatic<AndroidJavaClass>(prop, val as AndroidJavaClass);
        else if (type == "androidjavaobject")
            m_AndroidJavaObject.SetStatic<AndroidJavaObject>(prop, val as AndroidJavaObject);
        else if (type == "javaclass")
            m_AndroidJavaObject.SetStatic<AndroidJavaClass>(prop, (val as JavaClass).GetClass());
        else if (type == "javaobject")
            m_AndroidJavaObject.SetStatic<AndroidJavaObject>(prop, (val as JavaObject).GetObject());
        else
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