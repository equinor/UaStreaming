using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IP21Streamer.Application;
using Microsoft.Azure.EventHubs;
using log4net;
using System.Threading;
using IP21Streamer.Model;
using IP21Streamer.Extensions;

namespace IP21Streamer.Publisher
{
    class EventHub : IPublisher
    {
        #region Fields
        private string _connectionString;
        private int _publishInterval;

        private Settings Settings;
        private EventHubClient Client = null;
        private Queue<DataItem> PublishQueue = new Queue<DataItem>();
        private Object publishQueueGate = new object();

        private readonly ILog _log = LogManager.GetLogger(typeof(EventHub));
        #endregion

        #region Constructor
        public EventHub(Settings settings)
        {
            Settings = settings;

            _connectionString = settings.EventHubConnString;
            _publishInterval = settings.PublishInterval;
        }
        #endregion

        #region Connection
        public void Close()
        {
            Client.CloseAsync().Wait(5 * Settings.SECONDS);
            _log.Info("Closed connection to event hub");
        }

        public void Open()
        {
            bool connectionClosed = true;

            while (connectionClosed)
            {
                try
                {
                    Client = EventHubClient.CreateFromConnectionString(_connectionString);
                    _log.Info($"Connected to Event Hub: {Client.EventHubName}");

                    connectionClosed = false;
                }
                catch (Exception exception)
                {
                    _log.Warn($"Failed to connect to iotHub: {exception.Message}");

                    Thread.Sleep(10 * Settings.SECONDS);
                    _log.Info($"Attempting to Connect Again");
                }
            }
        }
        #endregion

        #region Publish
        public void Publish()
        {
            lock (publishQueueGate)
            {
                while (PublishQueue.Any())
                {
                    EventDataBatch eventBatch = new EventDataBatch(256 * Settings.K);

                    eventBatch.StuffWith(PublishQueue);

                    if (Client.SendAsync(eventBatch.ToEnumerable()).Wait(10 * Settings.SECONDS))
                        _log.Info($"Batch sent successfully to {Client.EventHubName}");
                    else
                    {
                        _log.Info($"Failed sending batch to {Client.EventHubName}");
                    }
                }
            }
        }

        public void AddToPublishQueue(string message, string dataType)
        {
            lock (publishQueueGate)
            {
                PublishQueue.Enqueue(
                new DataItem { Message = message, DataType = dataType });
            }
        }
        #endregion
    }
}
