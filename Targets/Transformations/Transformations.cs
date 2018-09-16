using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks.Dataflow;
using Common;
using System.Diagnostics;
using Serilog;
using static Common.Utils;

namespace Targets.Transformations
{
    public static class Transformations
    {
        #region Fields
        private static ILogger log = Log.Logger.ForContext(typeof(Transformations));
        #endregion

        public static IPropagatorBlock<EventPackage, EventDataBatch> BatchEvents()
        {

            var timer = new Stopwatch();
            timer.Start();

            var batch = new EventDataBatch(Constants.MaxByteSize);

            var source = new BufferBlock<EventDataBatch>();
            var target = new ActionBlock<EventPackage>(package =>
            {
                EventData eventData = new EventData(
                    Encoding.UTF8.GetBytes(
                        package.eventVqt.ToJson()));

                eventData.Properties.Add("dataType", package.eventType);
                eventData.Properties.Add("plant", package.plant);

                foreach (var recipient in package.recipients)
                    eventData.Properties.Add(recipient, true);

                if (!batch.TryAdd(eventData))
                {
                    log.Debug($"Batch full at {batch.Count} events");

                    source.Post(batch);

                    batch = new EventDataBatch(Constants.MaxByteSize);
                    batch.TryAdd(eventData);

                    timer.Restart();
                }
                else if (timer.Elapsed > TimeSpan.FromMilliseconds(GetEventHubPublishInterval()))
                {
                    log.Debug($"Batch timedOut at {batch.Count} events");

                    source.Post(batch);
                    batch = new EventDataBatch(Constants.MaxByteSize);

                    timer.Restart();
                }

            });

            target.Completion.ContinueWith(delegate
            {
                if (batch.Count > 0)
                    source.Post(batch);

                batch.Dispose();
                source.Complete();
            });

            return DataflowBlock.Encapsulate(target, source);
        }
    }
}
