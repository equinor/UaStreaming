using Microsoft.ServiceBus.Messaging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Targets.EventHub
{
    public class EventHubTarget : ITargetBlock<EventDataBatch>
    {
        #region Fields
        private ActionBlock<EventDataBatch> inBuffer;
        private EventHubClient client;

        private static ILogger log = Log.Logger.ForContext<EventHubTarget>();
        #endregion

        #region Constructor
        public EventHubTarget(string connectionString, int maxDegreeOfParallelism)
        {
            client = EventHubClient.CreateFromConnectionString(connectionString);

            inBuffer = new ActionBlock<EventDataBatch>(async batch =>
            {
                log.Debug($"Sending batch of count {batch.Count} to {client.Path}");
                await client.SendBatchAsync(batch.ToEnumerable());
                log.Debug($"Sent batch of count {batch.Count} to {client.Path}");
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });
        }
        #endregion

        #region 
        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, EventDataBatch messageValue, ISourceBlock<EventDataBatch> source, bool consumeToAccept) 
            => ((ITargetBlock<EventDataBatch>)inBuffer).OfferMessage(messageHeader, messageValue, source, consumeToAccept);

        public void Complete() 
            => inBuffer.Complete();

        public void Fault(Exception exception) 
            => ((ITargetBlock<EventDataBatch>)inBuffer).Fault(exception);

        public Task Completion
            => inBuffer.Completion;
        #endregion


    }
}
