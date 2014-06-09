using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace PkmnFoundations.GlobalTerminalService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private GTServer4 m_server;

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public void Start()
        {
            m_server = new GTServer4();
            m_server.BeginPolling();
        }

        protected override void OnStop()
        {
            if (m_server != null) m_server.EndPolling();
        }
    }
}
