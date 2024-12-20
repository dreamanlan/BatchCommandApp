﻿using System;
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
            try {
                if (!m_Enabled || null == m_LogStream)
                    return;
                string msg = DateTime.Now.ToString("HH-mm-ss-fff:") + string.Format(format, args);
                m_LogStream.WriteLine(msg);
                m_LogStream.Flush();
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogErrorFormat("exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public void Init(string logPath, string suffix)
        {
            try {
#if UNITY_EDITOR
                string logFile = string.Format("{0}/EditorStoryScript{1}.log", logPath, suffix);
#else
            	string logFile = string.Format("{0}/StoryScript{1}.log", logPath, suffix);
#endif
                if (null != m_LogStream) {
                    Release();
                }
                m_LogStream = new StreamWriter(logFile, false);
                Log("======StoryScript Logger Start ({0}, {1})======", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex) {
                UnityEngine.Debug.LogErrorFormat("exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        public void Dispose()
        {
            Release();
        }

        private void Release()
        {
            if (null != m_LogStream) {
                m_LogStream.Close();
                m_LogStream.Dispose();
                m_LogStream = null;
            }
        }

        private StreamWriter m_LogStream = null;
        private bool m_Enabled = true;
    }
}
