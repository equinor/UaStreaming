using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using IP21Streamer.Application;
using System.Threading;
using IP21Streamer.Source.UaSource;
using IP21Streamer.Source.UaSource.IP21;
using UnifiedAutomation.UaBase;

namespace IP21Streamer
{
    class Program
    {
        static App application;
        static ILog log;

        static void Main(string[] args)
        {
            // Create Logger Object
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger("Main");

            application = new App();

            IP21Source source = new IP21Source(ApplicationInstance.Default);

            source.Connect("opc.tcp://mo-tw08:63500/InfoPlus21/OpcUa/Server", "statoil-net\\bomu", "9oyU6gof");

            source.GetUpdatedModel();

        }
    }
}
