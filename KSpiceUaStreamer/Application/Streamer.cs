using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Common;
using Targets.Console;
using Targets.EventHub;
using Targets.Transformations;
using UaStreamer.KDI;
using UnifiedAutomation.UaBase;
using static Common.Utils;

namespace KSpiceUaStreamer.Application
{
    class Streamer
    {
        public async Task StartAsync(CancellationToken token)
        {
            var log = LogConfig.CreateLogger();
            log.Information($"Starting {GetApplicationName()}");

            try
            {
                ApplicationLicenseManager.AddProcessLicenses(System.Reflection.Assembly.GetExecutingAssembly(), GetUALicensePath());
                ApplicationInstance.Default.Start();

                log.Information($"License {ApplicationLicenseManager.GetAvailableLicense()} loaded successfully");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to load license.");
            }

            try
            {
                await StreamToEventHub(token);
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Fatal error");
            }
            finally
            {
                LogConfig.EndLogging();
            }
        }

        private async Task StreamToConsole(CancellationToken token)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var eventSource = new KSpiceUaSource(GetPlantCode(), GetSourceConnectionString());
            var eventTarget = new ConsoleTarget();

            eventSource.LinkTo(eventTarget, linkOptions);
            eventSource.StartListening();

            while (!token.IsCancellationRequested)
                await Task.Delay(3000, token);

            eventSource.StopListening();
            eventSource.Complete();

            await eventTarget.Completion;
        }

        private async Task StreamToEventHub(CancellationToken token)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var eventSource = new KSpiceUaSource(GetPlantCode(), GetSourceConnectionString());
            var batchEvents = Transformations.BatchEvents();
            var eventTarget = new EventHubTarget(GetEventHubConnectionString(), GetEventHubPublishParallelism());

            eventSource.LinkTo(batchEvents, linkOptions);
            batchEvents.LinkTo(eventTarget, linkOptions);
            eventSource.StartListening();

            while (!token.IsCancellationRequested)
                await Task.Delay(3000, token);

            eventSource.StopListening();
            eventSource.Complete();

            await batchEvents.Completion;
            await eventTarget.Completion;
        }
    }
}
