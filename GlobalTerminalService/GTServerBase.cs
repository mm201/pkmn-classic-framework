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

namespace PkmnFoundations.GlobalTerminalService
{
    public abstract class GTServerBase
    {
        public GTServerBase(int port) : this(port, false, 4)
        {
        }

        public GTServerBase(int port, bool useSsl)
            : this(port, useSsl, 4)
        {
        }

        public GTServerBase(int port, bool useSsl, int threads)
            : this(port, useSsl, threads, 5000)
        {
        }

        public GTServerBase(int port, bool useSsl, int threads, int timeout)
        {
            Threads = threads;
            Timeout = timeout;
            UseSsl = useSsl;
            m_workers = new List<Thread>(threads);
            m_listener = new TcpListener(IPAddress.Any, port);
            if (UseSsl)
            {
                Certificate = new X509Certificate2("cert.pfx", "letmein");
            }
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

                Console.WriteLine("{0} server running on port {1} with {2} threads.", Title, ((IPEndPoint)m_listener.LocalEndpoint).Port, Threads);

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

                m_listener.Stop();
                m_closing = true;
                // wait for worker threads to exit
                while (m_workers.Count > 0) { }
            }
        }

        private void MainLoop(object o)
        {
            int threadIndex = m_workers.IndexOf(Thread.CurrentThread);
            Console.WriteLine("Thread {0} begins.", threadIndex);

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

                        c.ReceiveTimeout = 5000;
                        c.SendTimeout = 5000;

                        Stream s = GetStream(c);
                        BinaryReader br = new BinaryReader(s);

                        int length = br.ReadInt32();
                        byte[] data = new byte[length];
                        BitConverter.GetBytes(length).CopyTo(data, 0);
                        br.ReadBlock(data, 4, length - 4);

                        byte[] response = ProcessRequest(data);
                        s.Write(response, 0, response.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Thread {0} ends.", threadIndex);
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
                SslStream sslClient = new SslStream(c.GetStream());
                sslClient.AuthenticateAsServer(Certificate);
                return sslClient;
            }
            else return c.GetStream();
        }

        protected abstract byte[] ProcessRequest(byte[] data);

        public abstract String Title { get; }

        protected void WriteLength(byte[] message)
        {
            byte[] data = BitConverter.GetBytes(message.Length);
            Array.Copy(data, 0, message, 0, 4);
        }
    }
}
