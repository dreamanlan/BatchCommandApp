using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using StoryScript;

internal sealed class GmSocketServer
{
    private readonly int m_Port;
    private TcpListener m_Listener;
    private Thread m_ListenThread;
    private volatile bool m_Running;

    internal GmSocketServer(int port)
    {
        m_Port = port;
    }

    internal void Start()
    {
        if (m_Running) return;
        m_Running = true;
        m_Listener = new TcpListener(IPAddress.Loopback, m_Port);
        m_Listener.Start();
        m_ListenThread = new Thread(ListenLoop);
        m_ListenThread.IsBackground = true;
        m_ListenThread.Start();
    }

    internal void Stop()
    {
        m_Running = false;
        try {
            if (null != m_Listener) {
                m_Listener.Stop();
            }
        }
        catch {
        }
        if (null != m_ListenThread) {
            m_ListenThread.Join(2000);
            m_ListenThread = null;
        }
        m_Listener = null;
    }

    private void ListenLoop()
    {
        while (m_Running) {
            try {
                var client = m_Listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(_ => HandleClient(client));
            }
            catch (SocketException) {
                // listener stopped
                break;
            }
            catch (ObjectDisposedException) {
                break;
            }
            catch (Exception ex) {
                LogSystem.Error("GmSocketServer accept error: {0}", ex.Message);
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try {
            client.ReceiveTimeout = 5000;
            using (var stream = client.GetStream()) {
                var buffer = new byte[4096];
                var sb = new StringBuilder();
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }
                string data = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(data)) {
                    // support multiple commands separated by newline
                    var lines = data.Split('\n');
                    foreach (var line in lines) {
                        var cmd = line.Trim();
                        if (!string.IsNullOrEmpty(cmd)) {
                            GmRootScript.SendCommand(cmd);
                            LogSystem.Warn("GmSocketServer received command: {0}", cmd);
                        }
                    }
                }
            }
        }
        catch (Exception ex) {
            LogSystem.Error("GmSocketServer handle client error: {0}", ex.Message);
        }
        finally {
            try { client.Close(); } catch { }
        }
    }
}
