using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using IP21Streamer.Application;
using System.Threading;

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

            while (true)
            {
                Thread.Sleep(5 * App.SECONDS);
                log.Debug($"SiteTag: {App.settings.SiteTag}");
            }
        }
    }
}
