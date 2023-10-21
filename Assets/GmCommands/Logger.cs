using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using StoryScript;

namespace GmCommands
{
    public sealed class Logger : IDisposable
    {
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }
        public void Log(string format, params object[] args)
        {
            if (!m_Enabled)
                return;
            string msg = DateTime.Now.ToString("HH-mm-ss-fff:") + string.Format(format, args);
            m_LogStream.WriteLine(msg);
            m_LogStream.Flush();
        }
        public void Init(string logPath, string suffix)
        {
            string logFile = string.Format("{0}/Game{1}.log", logPath, suffix);
            m_LogStream = new StreamWriter(logFile, false);
            Log("======GameLog Start ({0}, {1})======", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
        }
        public void Dispose()
        {
            Release();
        }

        private void Release()
        {
            m_LogStream.Close();
            m_LogStream.Dispose();
        }

        private StreamWriter m_LogStream;
        private bool m_Enabled = true;
    }
}
