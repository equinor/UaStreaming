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

        #region Config Strings
        private readonly string SAP_CODE = "sapCode";
        private readonly string STID_CODE = "stidCode";
        private readonly string UPDATE_METADATA = "updateMetadataDB";
        private readonly string UA_SERVER_URL = "uaServerUrl";
        private readonly string UA_TAGS_DB_CONNSTRING = "uaTagsDB";
        #endregion

        #region Setting Fields
        public int SAPCode { get; private set; }
        public string STIDCode { get; private set; }

        public string UaServerUrl { get; private set; }
        public string UaTagsDBConnString { get; private set; }

        public bool UpdateDBModel { get; private set; }
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

            SAPCode = Convert.ToInt32(ConfigurationManager.AppSettings.Get(SAP_CODE));
            STIDCode = ConfigurationManager.AppSettings.Get(STID_CODE);
            UaServerUrl = ConfigurationManager.AppSettings.Get(UA_SERVER_URL);

            UpdateDBModel = Convert.ToBoolean(ConfigurationManager.AppSettings.Get(UPDATE_METADATA));
        }
    }
}
