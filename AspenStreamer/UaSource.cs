using Common;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;
using static Common.Utils;

namespace UaStreamer.Implementation
{
    public abstract class UaSource
    {
        #region Fields
        private static ILogger log = Log.Logger.ForContext<UaSource>();

        protected ApplicationInstance applicationInstance;
        protected Session session = null;
        protected Subscription subscription = null;
        protected List<MonitoredItem> monitoredItems = new List<MonitoredItem>();
        #endregion

        #region Constructor
        protected UaSource(ApplicationInstance applicationInstance) => this.applicationInstance = applicationInstance;
        #endregion

        #region Connection
        public void Connect(string serverUrl, string userName, string password)
        {
            if (session == null) session = new Session(applicationInstance);

            session.UserIdentity = new UserIdentity
            {
                IdentityType = UserIdentityType.UserName,
                UserName = userName,
                Password = password
            };
            session.UseDnsNameAndPortFromDiscoveryUrl = false;

            session.Connect(serverUrl, SecuritySelection.None);
        }

        public void Connect(string serverUrl)
        {
            if (session == null) session = new Session(applicationInstance);

            session.UserIdentity = new UserIdentity { IdentityType = UserIdentityType.Anonymous};
            session.UseDnsNameAndPortFromDiscoveryUrl = false;

            session.Connect(serverUrl, SecuritySelection.None);
        }

        public void Disconnect()
        {
            if (subscription != null) subscription.Delete();
            session.Disconnect();
        }
        #endregion

        #region Subscription
        public void CreateSubscription()
        {
            if (subscription == null || subscription.ConnectionStatus != SubscriptionConnectionStatus.Created)
            {
                subscription = new Subscription(session)
                {
                    PublishingEnabled = false,
                    PublishingInterval = GetUaPublishingInterval(),
                    MaxKeepAliveTime = Constants.UaMaxKeepAliveTime,
                    Lifetime = Constants.UaLifeTime
                };
            }

            subscription.DataChanged += new DataChangedEventHandler(DataChanged);
            subscription.Create();

            log.Information("Subscription Created \n" +
                $"connection status: {subscription.ConnectionStatus} \n" +
                $"subscription id: {subscription.SubscriptionId}");
        }

        protected List<StatusCode> AddItemsToSubscription(List<DataMonitoredItem> itemsToMonitor)
        {
            if (!itemsToMonitor.Any())
                return new List<StatusCode>();

            monitoredItems.AddRange(itemsToMonitor);

            log.Debug($"Adding {itemsToMonitor.Count} items to subscription");
            return subscription.CreateMonitoredItems(
                itemsToMonitor.Cast<MonitoredItem>().ToList(),
                TimestampsToReturn.Both,
                new RequestSettings{ OperationTimeout = Constants.KSpiceOperationTimeOut }
                );
        }

        protected List<StatusCode> RemoveItemsFromSubscription(List<DataMonitoredItem> itemsToRemove)
        {
            if (!itemsToRemove.Any())
                return new List<StatusCode>();

            monitoredItems.RemoveAll(item => itemsToRemove.Contains(item));

            log.Debug($"Removing {itemsToRemove.Count} items from subscription");
            return subscription.DeleteMonitoredItems(itemsToRemove.Cast<MonitoredItem>().ToList());
        }
        #endregion

        #region Publishing
        protected void EnablePublishing()
        {
            if (!subscription.CurrentPublishingEnabled)
            {
                subscription.PublishingEnabled = true;
                subscription.Modify();
            }
        }

        protected void DisablePublishing()
        {
            if (subscription.CurrentPublishingEnabled)
            {
                subscription.PublishingEnabled = false;
                subscription.Modify();
            }
        }
        #endregion

        #region Browse Structure
        protected List<ReferenceDescription> BrowseFolderForClass(NodeId folderNodeId, NodeClass nodeClass)
        {
            var nodeReferences = new List<ReferenceDescription>();

            try
            {
                if (session == null) return nodeReferences;

                BrowseContext context = new BrowseContext
                {
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    MaxReferencesToReturn = Constants.KSpiceMaxNodesReturned,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)nodeClass
                };

                nodeReferences = session.Browse(
                    folderNodeId,
                    context,
                    new RequestSettings { OperationTimeout = Constants.KSpiceOperationTimeOut},
                    out byte[] continuationPoint
                    );

                while (continuationPoint != null)
                {
                    log.Debug($"ContinuationPoint returned while browsing {folderNodeId.Identifier}. Continuing to browse for more data.");
                    nodeReferences.AddRange(
                        session.BrowseNext(new RequestSettings { OperationTimeout = Constants.KSpiceOperationTimeOut }, ref continuationPoint)
                        );
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error while browsing folder: {folderNodeId.Identifier}");
            }

            return nodeReferences;
        }
        #endregion

        #region Abstract
        protected abstract void DataChanged(Subscription subscription, DataChangedEventArgs args);
        #endregion
    }
}
