using IP21Streamer.Source.IP21;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace IP21Streamer.Source
{
    abstract class UaSource : ISource
    {
        #region Fields
        static readonly ILog log = LogManager.GetLogger("UaSource");

        protected const int SECONDS = 1000;
        private const int PUBLISHING_INTERVAL = 2 * SECONDS;

        private ApplicationInstance _applicationInstance;
        protected Session _session = null;
        private Subscription _subscription = null;
        #endregion

        #region Construction
        public UaSource(ApplicationInstance applicationInstance)
        {
            this._applicationInstance = applicationInstance;
        }
        #endregion

        #region Connection
        public void Connect(string serverUrl, string userName, string password)
        {
            if (_session == null) _session = new Session(_applicationInstance);

            _session.UserIdentity = new UserIdentity
            {
                IdentityType = UserIdentityType.UserName,
                UserName = userName,
                Password = password
            };

            _session.UseDnsNameAndPortFromDiscoveryUrl = false;
            _session.Connect(serverUrl, SecuritySelection.None);
        }

        public void Disconnect()
        {
            if (_subscription != null) _subscription.Delete();
            _session.Disconnect();
        }
        #endregion

        #region Browsing and Updating Model
        public abstract void GetUpdatedModel();
        #endregion

        #region Subscription
        public void Subscribe()
        {
            if (_subscription == null || _subscription.ConnectionStatus != SubscriptionConnectionStatus.Created)
                CreateSubscription();

            CreateMonitoringList();
        }

        private void CreateSubscription()
        {
            _subscription = new Subscription(_session)
            {
                PublishingEnabled = true,
                PublishingInterval = PUBLISHING_INTERVAL,
                MaxKeepAliveTime = 10 * SECONDS,
                Lifetime = 15 * SECONDS,
                //Datachanged += new DataChangedEventHandler();
            };

            _subscription.Create();

            log.Info("Subscription Created: /n" +
                $"PublishingInterval: {_subscription.CurrentPublishingInterval} \n" +
                $"MaxKeepAliveTime: {_subscription.CurrentMaxKeepAliveTime}\n" +
                $"Lifetime: {_subscription.CurrentLifetime}");
        }

        private void CreateMonitoringList()
        {
            throw new NotImplementedException();
        }

        public void UpdateSubscription()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void GetEventCounts()
        {
            throw new NotImplementedException();
        }
    }
}
