﻿using System.ServiceProcess;

namespace Reflect.WebServer.Service
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}