﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDemo1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
#if DEBUG
            var service = new Service1();
            service.OnDebug();
#else

            ServiceBase[] ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };

            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
