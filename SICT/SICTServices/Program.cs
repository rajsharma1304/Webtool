///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> Program.cs </FileName>
///<CreatedOn> 6 Jan 2015</CreatedOn>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SICTService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
#if(!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SICTService()
            };
            ServiceBase.Run(ServicesToRun);
#else
            new SICTService().StartAll();
#endif
        }
    }
}