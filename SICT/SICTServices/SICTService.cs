using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SICTService
{
    public partial class SICTService : ServiceBase
    {
        private JanitorServices LogCleaner = null;

        public SICTService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartAll();
        }

        protected override void OnStop()
        {
            StopAll();
        }

#if(!DEBUG)

        private void StartAll()
#else

        public void StartAll()
#endif
        {
            //SALEventLogger.Instance.LogAudit("SICT Services started");
            LogCleaner = new JanitorServices();
            LogCleaner.Start();
        }

#if(!DEBUG)
        private void StopAll()
#else

        public void StopAll()
#endif
        {
            if (null != LogCleaner)
                LogCleaner.Stop();

            //SALEventLogger.Instance.LogAudit("SICT Services stopped");
        }
    }
}