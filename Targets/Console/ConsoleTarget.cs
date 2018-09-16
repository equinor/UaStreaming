using Domain.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Targets.Console
{
    public class ConsoleTarget : ITargetBlock<EventPackage>
    {
        #region Fields
        private ActionBlock<EventPackage> inBuffer;
        private static ILogger log = Log.Logger.ForContext<ConsoleTarget>();
        #endregion

        #region Constructor
        public ConsoleTarget()
        {
            log.Debug("Starting Console Target");

            inBuffer = new ActionBlock<EventPackage>( package =>
            {
                log.Debug($"Event: {package.ToPrettyJson()}");
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
        }
        #endregion

        #region ITargetBlock
        public Task Completion 
            => inBuffer.Completion;

        public void Complete() 
            => inBuffer.Complete();

        public void Fault(Exception exception) 
            => ((ITargetBlock<EventPackage>)inBuffer).Fault(exception);

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, EventPackage messageValue, ISourceBlock<EventPackage> source, bool consumeToAccept) 
            => ((ITargetBlock<EventPackage>)inBuffer).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        #endregion


    }
}
