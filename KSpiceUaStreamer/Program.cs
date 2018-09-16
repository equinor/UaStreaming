using Common;
using KSpiceUaStreamer.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace KSpiceUaStreamer
{
    class Program
    {
        /*static async Task Main(string[] args)
        {
            var streamer = new Streamer();
            await streamer.StartAsync(new CancellationToken());

            Console.WriteLine("Press Enter to Exit!");
            Console.ReadLine();
        }*/

        static void Main(string[] args)
        {
            var returnCode = HostFactory.Run(hc =>
            {
                var applicationName = Utils.GetApplicationName();
                var environment = Utils.GetEnvironment();
                hc.SetServiceName($"{applicationName}-{environment}");
                hc.SetDescription($"Write real time events from UA connection to Event Hub - {environment}");
                hc.SetDisplayName($"{applicationName} {environment}");

                hc.StartAutomatically();
                hc.RunAsPrompt();
                hc.Service<Streamer>(sc =>
                {
                    sc.ConstructUsing(() => new Streamer());
                    var cancellationTokenSource = new CancellationTokenSource();

                    Task streamingTask = null;
                    sc.WhenStarted(service =>
                    {
                        streamingTask = service.StartAsync(cancellationTokenSource.Token);
                    });
                    sc.WhenStopped(service =>
                    {
                        cancellationTokenSource.Cancel();
                        streamingTask.Wait();
                    });
                });
            });

            var exitCode = (int)Convert.ChangeType(returnCode, returnCode.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
