using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace sslProxy
{
    class Program
    {
        public static X509Certificate2 m_cert;
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(new IPAddress(new byte[]{10, 211, 55, 8}), 12401);
            Directory.CreateDirectory("log");
            m_cert = new X509Certificate2("iis private key.pfx", "letmein");

            // turn off cert validation entirely...
            // http://stackoverflow.com/questions/777607/the-remote-certificate-is-invalid-according-to-the-validation-procedure-using

            listener.Start();
            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(5);
                    continue;
                }

                BeginConversation(listener.AcceptTcpClient());
            }
        }

        static void BeginConversation(TcpClient client)
        {
            Console.WriteLine("Received connection from {0}", client.Client.AddressFamily);

            SslStream sslClient = new SslStream(client.GetStream());
            sslClient.AuthenticateAsServer(m_cert);

            TcpClient server = new TcpClient("pkgdsprod.nintendo.co.jp", 12401);
            SslStream sslServer = new SslStream(server.GetStream(), false, delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; });
            sslServer.AuthenticateAsClient("pkgdsprod.nintendo.co.jp");

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                sslServer.CopyTo(sslClient);
            });

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                sslClient.CopyTo(sslServer);
            });

        }
    }
}
