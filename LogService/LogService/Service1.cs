using Em.FundTrade.LogExtension.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;

namespace LogService
{
    public partial class Service1 : ServiceBase
    {
        internal static ServiceHost myServiceHost = null;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogManager.Maintenance();
            //log4net.Config.XmlConfigurator.Configure();
            myServiceHost = new ServiceHost(typeof(LogManager));
            myServiceHost.Open();
        }

        protected override void OnStop()
        {
            myServiceHost.Close();
            LogManager.WatiStop();
        }
    }
}
