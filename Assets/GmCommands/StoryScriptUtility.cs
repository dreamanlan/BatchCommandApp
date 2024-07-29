using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using StoryScript;

public static partial class StoryScriptUtility
{
    public static Transform FindChildRecursive(Transform parent, string bonePath)
    {
        Transform t = parent.Find(bonePath);
        if (null != t) {
            return t;
        } else {
            int ct = parent.childCount;
            for (int i = 0; i < ct; ++i) {
                t = FindChildRecursive(parent.GetChild(i), bonePath);
                if (null != t) {
                    return t;
                }
            }
        }
        return null;
    }
    public static bool IsPathMatch(UnityEngine.Transform tr, IList<string> names)
    {
        int ix = names.Count - 1;
        var p = tr;
        while (null != p && ix >= 0)
        {
            if (p.name == names[ix])
            {
                --ix;
            }
            p = p.parent;
        }
        return ix < 0;
    }
    public static string GetScenePath(UnityEngine.Transform tr, int up_level)
    {
        var sb = new StringBuilder();
        var stack = new Stack<string>();
        int lvl = 0;
        while (null != tr && lvl <= up_level)
        {
            stack.Push(tr.name);
            tr = tr.parent;
            ++lvl;
        }
        while (stack.Count > 0)
        {
            sb.Append('/');
            sb.Append(stack.Pop());
        }
        return sb.ToString();
    }
    public static GameObject GetGameObjectArg(ref BoxedValue arg)
    {
        GameObject gobj = null;
        if (arg.IsObject) {
            var gcomp = arg.ObjectVal as Component;
            if (null != gcomp) {
                gobj = gcomp.gameObject;
            }
            else {
                gobj = arg.ObjectVal as GameObject;
            }
        }
        else if (arg.IsString) {
            gobj = GameObject.Find(arg.AsString);
        }
        return gobj;
    }
    public static Renderer GetRendererArg(ref BoxedValue arg)
    {
        if (arg.IsObject) {
            var r = arg.ObjectVal as Renderer;
            if (null != r)
                return r;
        }
        GameObject gobj = GetGameObjectArg(ref arg);
        if (null != gobj) {
            var r = gobj.GetComponent<Renderer>();
            if (null == r) {
                r = gobj.GetComponent<MeshRenderer>();
            }
            if (null == r) {
                r = gobj.GetComponent<SkinnedMeshRenderer>();
            }
            return r;
        }
        return null;
    }
    public static Material GetMaterialArg(ref BoxedValue arg)
    {
        if (arg.IsObject) {
            var m = arg.ObjectVal as Material;
            if (null != m)
                return m;
        }
        var r = GetRendererArg(ref arg);
        if (null != r) {
            return r.material;
        }
        return null;
    }
    public static Material[] GetMaterialsArg(ref BoxedValue arg)
    {
        if (arg.IsObject) {
            var ms = arg.ObjectVal as Material[];
            if (null != ms)
                return ms;
        }
        var r = GetRendererArg(ref arg);
        if (null != r) {
            return r.materials;
        }
        return null;
    }
    public static Material GetSharedMaterialArg(ref BoxedValue arg)
    {
        if (arg.IsObject) {
            var m = arg.ObjectVal as Material;
            if (null != m)
                return m;
        }
        var r = GetRendererArg(ref arg);
        if (null != r) {
            return r.sharedMaterial;
        }
        return null;
    }
    public static Material[] GetSharedMaterialsArg(ref BoxedValue arg)
    {
        if (arg.IsObject) {
            var ms = arg.ObjectVal as Material[];
            if (null != ms)
                return ms;
        }
        var r = GetRendererArg(ref arg);
        if (null != r) {
            return r.sharedMaterials;
        }
        return null;
    }
    public static Color GetColor(string name)
    {
        var fi = typeof(Color).GetField(name);
        if (null != fi) {
            return (Color)fi.GetValue(null);
        }
        return Color.black;
    }

    public static void DrawGizmosCircle(Vector3 center, float radius, int step = 16)
    {
        for (int i = 1; i < step + 1; i++) {
            Vector3 pos0 = center + Quaternion.AngleAxis(360.0f * (i - 1) / step, Vector3.up) * Vector3.forward * radius;
            Vector3 pos1 = center + Quaternion.AngleAxis(360.0f * i / step, Vector3.up) * Vector3.forward * radius;
            Gizmos.DrawLine(pos0, pos1);
        }
    }
    public static void DrawGizmosArraw(Vector3 start, Vector3 end, float size = 0.5f)
    {
        Vector3 dir = (end - start).normalized;
        float length = (end - start).magnitude;
        Gizmos.DrawLine(start, end);
        Vector3 left = Quaternion.AngleAxis(45, Vector3.up) * (-dir);
        Gizmos.DrawLine(end, end + left * size);
        Vector3 right = Quaternion.AngleAxis(-45, Vector3.up) * (-dir);
        Gizmos.DrawLine(end, end + right * size);
    }
    public static void DestroyObject(UnityEngine.Object obj)
    {
        if (obj != null) {
            if (Application.isPlaying) {
                if (Application.isEditor) {
                    if(obj is GameObject)
                        UnityEngine.Object.Destroy(obj);
                }
                else {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }
    }
    public static void DestroyObjectFull(UnityEngine.Object obj)
    {
        if (obj != null) {
            if (Application.isEditor && !Application.isPlaying) {
                UnityEngine.Object.DestroyImmediate(obj);
            } else {
                UnityEngine.Object.Destroy(obj);
            }
        }
    }

    public static void SendMessage(string objname, string msg, object arg)
    {
        SendMessage(objname, msg, arg, false);
    }
    public static void SendMessage(string objname, string msg, object arg, bool needReceiver)
    {
        SendMessageImpl(objname, msg, arg, needReceiver);
    }
    public static void SendMessageWithTag(string objtag, string msg, object arg)
    {
        SendMessageWithTag(objtag, msg, arg, false);
    }
    public static void SendMessageWithTag(string objtag, string msg, object arg, bool needReceiver)
    {
        SendMessageWithTagImpl(objtag, msg, arg, needReceiver);
    }
    public static Type GetType(string type)
    {
        Type ret = null;
        try {
            ret = Type.GetType("UnityEngine." + type + ", UnityEngine");
            if (null == ret) {
                ret = Type.GetType("UnityEngine.UI." + type + ", UnityEngine.UI");
            }
            if (null == ret) {
                ret = Type.GetType(type + ", Assembly-CSharp");
            }
            if (null == ret) {
                ret = Type.GetType(type);
            }
            if (null == ret) {
                Debug.LogWarningFormat("null == Type.GetType({0})", type);
            }
        } catch (Exception ex) {
            Debug.LogErrorFormat("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
        return ret;
    }

    private static void SendMessageImpl(string objname, string msg, object arg, bool needReceiver)
    {
        GameObject obj = GameObject.Find(objname);
        if (null != obj) {
            try {
                obj.SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                if (msg.CompareTo("LogToConsole") != 0)
                    Debug.LogFormat("SendMessage {0} {1} {2} {3}", objname, msg, arg, needReceiver);
            } catch (Exception ex) {
                Debug.LogErrorFormat("SendMessage({0} {1} {2} {3}) Exception {4}\n{5}", objname, msg, arg, needReceiver, ex.Message, ex.StackTrace);
            }
        }
    }
    private static void SendMessageWithTagImpl(string objtag, string msg, object arg, bool needReceiver)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(objtag);
        if (null != objs) {
            for (int i = 0; i < objs.Length; i++) {
                try {
                    objs[i].SendMessage(msg, arg, needReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                    if (msg.CompareTo("LogToConsole") != 0)
                        Debug.LogFormat("SendMessageWithTag {0} {1} {2} {3}", objtag, msg, arg, needReceiver);
                } catch (Exception ex) {
                    Debug.LogErrorFormat("SendMessageWithTag({0} {1} {2} {3}) Exception {4}\n{5}", objtag, msg, arg, needReceiver, ex.Message, ex.StackTrace);
                }
            }
        }
    }
}

internal static class Literal
{
    internal static string GetIndentString(int indent)
    {
        PrepareIndent(indent);
        return s_IndentString.Substring(0, indent);
    }
    internal static string GetSpaceString(int indent)
    {
        PrepareSpace(indent * c_IndentSpaceString.Length);
        return s_SpaceString.Substring(0, indent * c_IndentSpaceString.Length);
    }

    private static void PrepareIndent(int indent)
    {
        if (s_IndentString.Length < indent) {
            int len = c_InitCount;
            while (len < indent) {
                len *= 2;
            }
            s_IndentString = new string('\t', indent);
        }
    }
    private static void PrepareSpace(int indent)
    {
        if (s_SpaceString.Length < indent) {
            int len = c_InitCount;
            while (len < indent) {
                len *= 2;
            }
            while (s_SpaceBuilder.Length < len) {
                s_SpaceBuilder.Append(c_IndentSpaceString);
            }
            s_SpaceString = s_SpaceBuilder.ToString();
        }
    }

    private static string s_IndentString = string.Empty;
    private static string s_SpaceString = string.Empty;
    private static StringBuilder s_SpaceBuilder = new StringBuilder();

    private const int c_InitCount = 256;
    private const string c_IndentSpaceString = "| ";
}
internal static class StringBuilderExtension
{
    public static void Append(this StringBuilder sb, string fmt, params object[] args)
    {
        if (args.Length == 0) {
            sb.Append(fmt);
        }
        else {
            sb.AppendFormat(fmt, args);
        }
    }
    public static void AppendLine(this StringBuilder sb, string fmt, params object[] args)
    {
        if (args.Length == 0) {
            sb.AppendLine(fmt);
        }
        else {
            sb.AppendFormat(fmt, args);
            sb.AppendLine();
        }
    }
}
