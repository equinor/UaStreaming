using IP21Streamer.Repository;
using IP21Streamer.Source;
using IP21Streamer.Source.IP21;
using IP21Streamer.Source.UaSource.IP21;
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
        private const string PASSWORD = "9oyU6gof";
        #endregion

        #region Field
        private ISource<IP21Tag> _uaSource; // todo: fix the type reference
        private IRepository _metaDataStore;

        public static Settings Settings = new Settings();
        #endregion

        public App()
        {
            _uaSource = new IP21Source(ApplicationInstance.Default);
            _metaDataStore = new MetaDataStore(Settings.UaTagsDBConnString);

        }

        public void Run()
        {
            Initialize();

            if (Settings.UpdateDBModel)
                UpdateMetaDataDB();

            CreateSubscriptions()
        }

        private void CreateSubscriptions()
        {
            _metaDataStore.GetSubscriptionList();

            _uaSource.SubscribeTo(subscriptionList);
        }

        private void Initialize()
        {
            _metaDataStore.Initialize();
            _uaSource.Connect(Settings.UaServerUrl, USER_NAME, PASSWORD);

            new Thread(KeepSettingsUpToDate).Start();

            if (Settings.Publish)
                new Thread(Publisher).Start();
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

    }
}
