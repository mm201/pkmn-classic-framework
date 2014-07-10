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
            m_server_4 = new GTServer4();
            m_server_5 = new GTServer5();
        }

        private GTServer4 m_server_4;
        private GTServer5 m_server_5;

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public void Start()
        {
            m_server_4.BeginPolling();
            m_server_5.BeginPolling();
        }

        protected override void OnStop()
        {
            if (m_server_4 != null) m_server_4.EndPolling();
            // fixme: it waits for the GenIV server to stop completely before
            // shutting down the GenV server. Should shut them both down async.
            if (m_server_5 != null) m_server_5.EndPolling();
        }
    }
}
