using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Expression;
using ExpressionAPI;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        s_Instance = this;

        var txtWriter = new StringWriter(m_LogBuilder);
        System.Console.SetOut(txtWriter);
        System.Console.SetError(txtWriter);

        Application.logMessageReceived += HandleLog;

        m_Calculator.Init();
        m_Calculator.Register("copypdf", new ExpressionFactoryHelper<CopyPdfExp>());
        m_Calculator.Register("jc", new ExpressionFactoryHelper<JavaClassExp>());
        m_Calculator.Register("jo", new ExpressionFactoryHelper<JavaObjectExp>());
        m_Calculator.Register("jp", new ExpressionFactoryHelper<JavaProxyExp>());
        m_Calculator.Register("oc", new ExpressionFactoryHelper<ObjectcClassExp>());
        m_Calculator.Register("oo", new ExpressionFactoryHelper<ObjectcObjectExp>());

        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true) {
            yield return new WaitForSeconds(1.0f);
            if (m_LogBuilder.Length > 0) {
                var txt = m_LogBuilder.ToString();
                m_LogBuilder.Length = 0;
                DebugConsole.Log(txt);
            }
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        DebugConsole.Log("[" + type.ToString() + "]" + logString);
    }

    private void OnExecCommand(string cmd)
    {
        Dsl.DslFile file = new Dsl.DslFile();
        if(file.LoadFromString(string.Format("script(){{{0};}};", cmd), "cmd", msg => Debug.LogWarning(msg))) {
            m_Calculator.Cleanup();
            m_Calculator.LoadDsl("main", file.DslInfos[0].First);
            m_Calculator.Calc("main");
        }
    }

    private StringBuilder m_LogBuilder = new StringBuilder();
    private Expression.DslCalculator m_Calculator = new Expression.DslCalculator();

    public static object Call(string proc, params object[] args)
    {
        if (null == s_Instance)
            return null;
        object r = s_Instance.m_Calculator.Calc(proc, args);
        return r;
    }
    private static Main s_Instance = null;
}

namespace ExpressionAPI
{
    internal sealed class CopyPdfExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            string file = m_File.Calc() as string;
            int start = ToInt(m_Start.Calc());
            int count = ToInt(m_Count.Calc());
            CopyPdf(file, start, count);
            return count;
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_File = exps[0];
            m_Start = exps[1];
            m_Count = exps[2];
            return true;
        }
        private void CopyPdf(string file, int start, int count)
        {
            var sb = new StringBuilder();
            PdfReader reader = new PdfReader(file);
            for (int page = start; page < start + count && page <= reader.NumberOfPages; ++page) {
                var txt = PdfTextExtractor.GetTextFromPage(reader, page);
                sb.AppendLine(txt);
            }
            reader.Close();
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = sb.ToString();
#elif UNITY_IOS
            SetClipboard(sb.ToString());
#elif UNITY_ANDROID
            AndroidJavaClass cb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");
            cb.CallStatic ("setText", sb.ToString());
#endif
        }

        private IExpression m_File;
        private IExpression m_Start;
        private IExpression m_Count;
#if UNITY_IOS
        [DllImport("__Internal")]
        static extern void SetClipboard(string str);
        [DllImport("__Internal")]
        static extern string GetClipboard();
#endif
    }
    internal sealed class JavaClassExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                r = new JavaClass(str);
            }
            return r;
        }
    }
    internal sealed class JavaObjectExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                var al = new ArrayList();
                for(int i = 1; i < operands.Count; ++i) {
                    al.Add(operands[i]);
                }
                r = new JavaObject(str, al.ToArray());
            }
            return r;
        }
    }
    internal sealed class JavaProxyExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 2) {
                var _class = operands[0] as string;
                var scpMethod = operands[1] as string;
                r = new JavaProxy(_class, scpMethod);
            }
            return r;
        }
    }
    internal sealed class ObjectcClassExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                var str = operands[0] as string;
                r = new ObjectcClass(str);
            }
            return r;
        }
    }
    internal sealed class ObjectcObjectExp : SimpleExpressionBase
    {
        protected override object OnCalc(IList<object> operands)
        {
            object r = null;
            if (operands.Count >= 1) {
                int objId = (int)System.Convert.ChangeType(operands[0], typeof(int));
                return new ObjectcObject(objId);
            }
            return r;
        }
    }
}