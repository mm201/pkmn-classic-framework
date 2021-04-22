using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using PkmnFoundations.Support;
using System.Diagnostics;

namespace PkmnFoundations.GlobalTerminalService
{
    public abstract class GTServerBase
    {
        public GTServerBase(int port) 
            : this(port, false, 16, 5000)
        {
        }

        public GTServerBase(int port, bool useSsl)
            : this(port, useSsl, 16, 5000)
        {
        }

        public GTServerBase(int port, bool useSsl, int threads)
            : this(port, useSsl, threads, 5000)
        {
        }

        // todo: It would probably be better to load a custom certificate from
        // app.config, but since the DS doesn't even validate it, there's no
        // real point in securing it.
        public GTServerBase(int port, bool useSsl, int threads, int timeout)
            : this(port, useSsl, threads, timeout,
            useSsl ? GetDefaultCertificate() : null)
        {
        }

        public GTServerBase(int port, bool useSsl, int threads, int timeout, X509Certificate2 certificate)
        {
            Threads = threads;
            Timeout = timeout;
            UseSsl = useSsl;
            Certificate = certificate;
            m_workers = new List<Thread>(threads);
            m_listener = new TcpListener(IPAddress.Any, port);
        }

        private static X509Certificate2 GetDefaultCertificate()
        {
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection cers = store.Certificates.Find(X509FindType.FindBySubjectName, "pkgdsprod.nintendo.co.jp", false);

            if (cers.Count > 0)
                return cers[0];

            LogHelper.Write("X.509 certificate not found. Please add a certificate with subject \"pkgdsprod.nintendo.co.jp\" to the store. Using dummy certificate.");
            return new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "cert.pfx", "letmein");
        }

        public int Threads
        {
            get;
            protected set;
        }

        public int Timeout
        {
            get;
            protected set;
        }

        protected bool UseSsl
        {
            get;
            set;
        }

        protected X509Certificate Certificate { get; set; }

        private List<Thread> m_workers;
        private object m_lock = new object();
        private bool m_closing;
        private TcpListener m_listener;

        public void BeginPolling()
        {
            lock (m_lock)
            {
                if (m_workers.Count > 0) return;

                LogHelper.Write(String.Format("{0} server running on port {1} with {2} threads.", Title, ((IPEndPoint)m_listener.LocalEndpoint).Port, Threads));

                m_closing = false;
                m_listener.Start();
                for (int x = 0; x < Threads; x++)
                {
                    Thread t = new Thread(MainLoop);
                    m_workers.Add(t);
                    t.Start();
                }
            }
        }

        public void EndPolling()
        {
            lock (m_lock)
            {
                if (m_workers.Count == 0) return;

                m_closing = true;
                // wait for worker threads to exit
                while (m_workers.Count > 0) 
                {
                    Thread.Sleep(10);
                }
                m_listener.Stop();
            }
        }

        private void MainLoop(object o)
        {
            int threadIndex = m_workers.IndexOf(Thread.CurrentThread);
            // This is too chatty for an event log.
            //LogHelper.Write(String.Format("Thread {0} begins.", threadIndex));

            while (!m_closing)
            {
                try
                {
                    if (!m_listener.Pending())
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    using (TcpClient c = AcceptClient())
                    {
                        if (c == null)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        try
                        {
                            c.ReceiveTimeout = Timeout;
                            c.SendTimeout = Timeout;

                            Stream s = GetStream(c);
                            BinaryReader br = new BinaryReader(s);

                            int length = br.ReadInt32();
                            if (length > 7820)
                            {
                                LogHelper.Write(String.Format("Indicated request length is over limit.\nHost: {0}", c.Client.RemoteEndPoint), EventLogEntryType.FailureAudit);
                                continue;
                            }
                            if (length < 320)
                            {
                                LogHelper.Write(String.Format("Indicated request length is under limit.\nHost: {0}", c.Client.RemoteEndPoint), EventLogEntryType.FailureAudit);
                                continue;
                            }

                            byte[] data = new byte[length];
                            BitConverter.GetBytes(length).CopyTo(data, 0);

                            int actualLength = br.ReadBlock(data, 4, length - 4);
                            if (actualLength + 4 != length)
                            {
                                LogHelper.Write(String.Format("The client disconnected prematurely.\nHost: {0}", c.Client.RemoteEndPoint), EventLogEntryType.FailureAudit);
                                continue;
                            }

                            byte[] response = ProcessRequest(data, c);
                            s.Write(response, 0, response.Length);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Write(String.Format("Unhandled exception while handling request:\nHost: {0}\nException: {1}", c.Client.RemoteEndPoint, ex.Message), EventLogEntryType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Write(String.Format("Unhandled exception while handling request:\nException: {0}", ex.Message), EventLogEntryType.Error);
                }
            }

            //LogHelper.Write(String.Format("Thread {0} ends.", threadIndex));
            m_workers.Remove(Thread.CurrentThread);
        }

        private TcpClient AcceptClient()
        {
            lock (m_listener)
            {
                if (!m_listener.Pending()) return null;
                return m_listener.AcceptTcpClient();
            }
        }

        private Stream GetStream(TcpClient c)
        {
            if (UseSsl)
            {
                // todo: If we target .NET Core 3+, we can manually enable RC4 cipher suites.
                // https://github.com/dotnet/runtime/issues/23818
                // The DS wants to use SSLv3 and one of the following ciphers:
                // 0x0004 TLS_RSA_WITH_RC4_128_MD5
                // 0x0005 TLS_RSA_WITH_RC4_128_SHA
                // For now, the only functional approach is to enable these ciphers in the registry or via e.g. IISCrypto.
                SslStream sslClient = new SslStream(c.GetStream());
                sslClient.AuthenticateAsServer(Certificate, false, System.Security.Authentication.SslProtocols.Ssl3, false);
                return sslClient;
            }
            else return c.GetStream();
        }

        protected abstract byte[] ProcessRequest(byte[] data, TcpClient c);

        public abstract String Title { get; }

        protected void WriteLength(byte[] message)
        {
            byte[] data = BitConverter.GetBytes(message.Length);
            Array.Copy(data, 0, message, 0, 4);
        }
    }
}
