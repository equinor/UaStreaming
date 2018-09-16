using IP21Streamer.Model;
using IP21Streamer.Publisher;
using IP21Streamer.Repository;
using IP21Streamer.Source;
using IP21Streamer.Source.IP21;
using IP21Streamer.Source.UaSource.IP21;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Application
{
    class App
    {
        #region Constants
        public const int SECONDS = 1000;
        public const int MINUTES = 60 * SECONDS;
        public const int UPDATE_SETTINGS_INTERVAL = 1 * MINUTES;

        // todo: These should be removed at some point
        private const string USER_NAME = "statoil-net\\bomu";
        private const string PASSWORD = "";
        #endregion

        #region Field
        private readonly ILog log = LogManager.GetLogger(typeof(App));

        private ISource<IP21Tag> _uaSource; // todo: fix the type reference
        private IRepository _metaDataStore;
        private IPublisher _eventHub;

        public static Settings Settings = new Settings();
        #endregion

        public App()
        {
            _uaSource = new IP21Source(ApplicationInstance.Default, NewEventCallback);
            _metaDataStore = new MetaDataStore(Settings.UaTagsDBConnString);
            _eventHub = new EventHub(Settings);

        }

        public void Run()
        {
            Initialize();

            if (Settings.UpdateDBModel)
                UpdateMetaDataDB();

            CreateSubscriptions();
        }

        private void CreateSubscriptions()
        {
            _uaSource.SubscribeTo(
                _metaDataStore.GetSubscriptionList());
        }

        private void Initialize()
        {
            _metaDataStore.Initialize();
            _uaSource.Connect(Settings.UaServerUrl, USER_NAME, PASSWORD);
            _eventHub.Open();

            new Thread(KeepSettingsUpToDate).Start();

            if (Settings.PublishToEventHub)
                new Thread(Publisher).Start();
        }

        private void NewEventCallback(EventItem data)
        {
            _eventHub.AddToPublishQueue(data.ToJson(), "event");
        }

        private void UpdateMetaDataDB()
        {
            List<IP21Tag> tagNodes = _uaSource.GetUpdatedModel();

            List<TagItem> foundTagItems = new List<TagItem>();
            foundTagItems.FillWith(tagNodes);

            _metaDataStore.UpdateMetaDataWith(foundTagItems);
        }

        private void KeepSettingsUpToDate()
        {
            Thread.CurrentThread.IsBackground = true;

            while (true)
            {
                Thread.Sleep(UPDATE_SETTINGS_INTERVAL);
                Settings.Update();
            }
        }

        private void Publisher()
        {
            Thread.CurrentThread.IsBackground = true;

            while (Settings.PublishToEventHub)
            {
                _eventHub.Publish();
                Thread.Sleep(Settings.PublishInterval);
            }
        }

    }
}
