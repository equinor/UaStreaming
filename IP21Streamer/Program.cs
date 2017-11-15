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
        static App _application; // todo: remove this when ready
        static ILog log;

        static void Main(string[] args)
        {
            // Create Logger Object
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger("Main");

            _application = new App();
            _application.Run();


            Console.WriteLine("Press enter to exit application...");
            Console.ReadLine();

        }
    }
}
