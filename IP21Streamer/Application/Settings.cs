using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Application
{
    class Settings
    {
        static readonly ILog log = LogManager.GetLogger("Settings");

        #region Constants
        public const int SECONDS = 1000;
        public const int MINUTES = 60 * SECONDS;
        public const int K = 1000;
        #endregion

        #region Config Strings
        private readonly string SAP_CODE = "sapCode";
        private readonly string STID_CODE = "stidCode";
        private readonly string UPDATE_METADATA = "updateMetadataDB";
        private readonly string UA_SERVER_URL = "uaServerUrl";
        private readonly string UA_TAGS_DB_CONNSTRING = "uaTagsDB";
        private readonly string EVENT_HUB = "eventHub";
        private readonly string PUBLISH_TO_EVENTHUB = "publishToEventHub";
        public readonly string PUBLISH_INTERVAL_IN_SECONDS = "publishIntervalInSeconds";
        #endregion

        #region Setting Fields
        public int SAPCode { get; private set; }
        public string STIDCode { get; private set; }

        public string UaServerUrl { get; private set; }
        public string UaTagsDBConnString { get; private set; }
        public string EventHubConnString { get; set; }

        public bool UpdateDBModel { get; private set; }
        public bool PublishToEventHub { get; private set; }
        public int PublishInterval { get; private set; }
        #endregion

        public Settings()
        {
            this.Update();
        }

        internal void Update()
        {
            log.Debug("Updating Settings");

            ConfigurationManager.RefreshSection("appSettings");

            UaTagsDBConnString = ConfigurationManager.ConnectionStrings[UA_TAGS_DB_CONNSTRING].ConnectionString;
            EventHubConnString = ConfigurationManager.ConnectionStrings[EVENT_HUB].ConnectionString;

            SAPCode = Convert.ToInt32(ConfigurationManager.AppSettings.Get(SAP_CODE));
            STIDCode = ConfigurationManager.AppSettings.Get(STID_CODE);
            UaServerUrl = ConfigurationManager.AppSettings.Get(UA_SERVER_URL);
            PublishInterval = Convert.ToInt32(ConfigurationManager.AppSettings.Get(PUBLISH_INTERVAL_IN_SECONDS)) * SECONDS;

            UpdateDBModel = Convert.ToBoolean(ConfigurationManager.AppSettings.Get(UPDATE_METADATA));
            PublishToEventHub = Convert.ToBoolean(ConfigurationManager.AppSettings.Get(PUBLISH_TO_EVENTHUB));
        }
    }
}
