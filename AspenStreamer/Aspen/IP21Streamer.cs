using Common.Contracts;
using Domain.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;
using Common;
using UaStreamer.Implementation;
using UaStreamer.Extensions;

namespace AspenStreamer.Implementation
{
    class IP21Source : UaSource, IEventSource
    {
        #region fields
        private static ILogger log = Log.Logger.ForContext<IP21Source>();

        private BufferBlock<EventPackage> outBuffer;

        private string plantCode;
        #endregion

        #region constructor
        public IP21Source(string plantCode, string serverUrl, string username, string password, BufferBlock<EventPackage> outBuffer) : base(ApplicationInstance.Default)
        {
            this.plantCode = plantCode;
            this.outBuffer = outBuffer;

            base.Connect(serverUrl, username, password);
        }
        #endregion

        #region interface
        public void StartListening()
        {
            throw new NotImplementedException();
        }

        public void StopListening()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IEnumerable<string> tagNames)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(IEnumerable<string> tagNames)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Override
        protected override void DataChanged(Subscription subscription, DataChangedEventArgs args)
        {
            foreach (var change in args.DataChanges)
            {
                NodeData nodeData = (NodeData)change.MonitoredItem.UserData;

                EventVqt @event = new EventVqt();
                @event.FillWith(nodeData, change);
                var package = new EventPackage(@event, plantCode, Constants.RealTime, null);

                outBuffer.Post(package);
            }
        }
        #endregion
    }
}
