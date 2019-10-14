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
        var txtWriter = new StringWriter(m_LogBuilder);
        System.Console.SetOut(txtWriter);
        System.Console.SetError(txtWriter);

        Application.logMessageReceived += HandleLog;

        m_Calculator.Init();
        m_Calculator.Register("copypdf", new ExpressionFactoryHelper<CopyPdfExp>());
        m_Calculator.Register("jc", new ExpressionFactoryHelper<JavaClassExp>());
        m_Calculator.Register("jo", new ExpressionFactoryHelper<JavaObjectExp>());
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

    void HandleLog(string logString, string stackTrace, LogType type)
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
    internal sealed class JavaClassExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            string _class = m_Class.Calc() as string;
            return new JavaClass(_class);
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_Class = exps[0];
            return true;
        }

        private IExpression m_Class;
    }
    internal sealed class JavaObjectExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            string _class = m_Object.Calc() as string;
            return new JavaObject(_class);
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_Object = exps[0];
            return true;
        }

        private IExpression m_Object;
    }
    internal sealed class ObjectcClassExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            string _class = m_Class.Calc() as string;
            return new ObjectcClass(_class);
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_Class = exps[0];
            return true;
        }

        private IExpression m_Class;
    }
    internal sealed class ObjectcObjectExp : AbstractExpression
    {
        protected override object DoCalc()
        {
            int objId = (int)System.Convert.ChangeType(m_Object.Calc(), typeof(int));
            return new ObjectcObject(objId);
        }
        protected override bool Load(IList<IExpression> exps)
        {
            m_Object = exps[0];
            return true;
        }

        private IExpression m_Object;
    }
}