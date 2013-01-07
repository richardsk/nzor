using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;


namespace NZOR.Server
{
    public partial class NZORService : ServiceBase
    {
        public NZORService()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            System.Threading.Thread.GetDomain().UnhandledException += new UnhandledExceptionEventHandler(NZORService_UnhandledException);
            NZORProcessor.Start();
        }

        void NZORService_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
         //TODO:   NZOR.Utility.Log.LogEvent("ERROR : Unhandled Exception : " + e.ExceptionObject == null ? "" : e.ExceptionObject.ToString());
        }


        protected override void OnStop()
        {
            NZORProcessor.Stop();
        }

        
    }
}
