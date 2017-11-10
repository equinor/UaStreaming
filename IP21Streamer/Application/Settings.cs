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
        private readonly string SITE_TAG = "siteTag";
        private readonly string UA_SERVER_URL = "uaServerUrl";
        private readonly string UPDATE_METADATA = "updateMetadataDB";
        #endregion

        #region Setting Fields
        public string SiteTag { get; private set; }
        public string UaServerUrl { get; private set; }

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

            SiteTag = ConfigurationManager.AppSettings.Get(SITE_TAG);
            UaServerUrl = ConfigurationManager.AppSettings.Get(UA_SERVER_URL);

            UpdateDBModel = Convert.ToBoolean(ConfigurationManager.AppSettings.Get(UPDATE_METADATA));
        }
    }
}
