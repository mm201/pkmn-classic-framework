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
        {
            Threads = threads;
            UseSsl = useSsl;
            m_workers = new List<Thread>(threads);
            // todo: enumerate nic bindings
            m_listener = new TcpListener(port);
            if (UseSsl)
            {
                Certificate = new X509Certificate2("cert.pfx", "letmein");
            }
        }

        public int Threads
        {
            get;
            private set;
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
                if (!m_listener.Pending())
                {
                    Thread.Sleep(5);
                    continue;
                }
                Stream s = AcceptRequest();
                if (s == null)
                {
                    Thread.Sleep(5);
                    continue;
                }

                byte[] data = new byte[4];
                s.Read(data, 0, 4);
                int length = BitConverter.ToInt32(data, 0);
                data = new byte[length];
                BitConverter.GetBytes(length).CopyTo(data, 0);
                s.Read(data, 4, length - 4); // todo: stop DoS by timing out blocking requests

                byte[] response = ProcessRequest(data);
                s.Write(response, 0, response.Length);
            }

            Console.WriteLine("Thread {0} ends.", threadIndex);
            m_workers.Remove(Thread.CurrentThread);
        }

        private Stream AcceptRequest()
        {
            // todo: handle case that another thread took the request and return null
            TcpClient c = m_listener.AcceptTcpClient();
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
    }
}
