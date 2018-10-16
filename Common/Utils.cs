using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Utils
    {
        #region Constants
        public const int SECONDS = 1000;
        #endregion

        #region Settings
        public static string GetApplicationName()
            => GetConfigValue("applicationName");

        public static string GetEnvironment()
            => GetConfigValue("environment");

        public static string GetPlantCode()
            => GetConfigValue("plantCode");

        public static string GetSourceType()
            => GetConfigValue("sourceType");

        public static string GetSourceConnectionString()
            => GetConfigValue("sourceConnectionString");

        public static string GetUALicensePath()
            => GetConfigValue("uaLicensePath");

        public static double GetUaPublishingInterval()
            => Double.Parse(GetConfigValue("uaPublishingInterval"));

        public static double GetUaSamplingInterval()
            => Double.Parse(GetConfigValue("uaSamplingInterval"));

        public static string GetTagMatchPattern()
            => GetConfigValue("tagMatchPattern");

        public static int GetMatchLimit()
            => int.Parse(GetConfigValue("maxNumberOfTags"));

        internal static string GetLogPath() 
            => GetConfigValue("logPath");

        public static string GetAiInstrumentationKey()
            => GetConfigValue("aiInstrumentationKey");

        public static string GetEventHubConnectionString()
            => GetConfigValue("eventHubConnectionString");

        public static double GetEventHubPublishInterval()
            => Double.Parse(GetConfigValue("eventHubPublishInterval"));

        public static int GetEventHubPublishParallelism()
            => int.Parse(GetConfigValue("eventHubPublishParallelism"));
        #endregion

        private static string GetConfigValue(string name)
        {
            var value = ConfigurationManager.AppSettings.Get(name);

            if (value == null)
                throw new Exception($"Cannot find application setting: {name}");

            return value;
        }

    }
}
