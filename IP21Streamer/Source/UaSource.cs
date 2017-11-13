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
        private static readonly ILog log = LogManager.GetLogger(typeof(UaSource));

        protected const int SECONDS = 1000;
        protected const int PUBLISHING_INTERVAL = 2 * SECONDS;
        protected const int MAX_NODES_TO_RETURN = 100000;

        protected ApplicationInstance _applicationInstance;
        protected Session _session = null;
        protected Subscription _subscription = null;
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

        protected List<ReferenceDescription> BrowseFolder(NodeId FolderNodeId)
        {
            var nodeReferences = new List<ReferenceDescription>();

            try
            {
                if (_session == null) return nodeReferences;

                BrowseContext context = new BrowseContext
                {
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    MaxReferencesToReturn = MAX_NODES_TO_RETURN,
                    IncludeSubtypes = true
                };

                //byte[] continuationPoint = null;

                nodeReferences = _session.Browse(
                    FolderNodeId,
                    context,
                    new RequestSettings { OperationTimeout = 10 * SECONDS },
                    out byte[] continuationPoint);

            }
            catch (Exception exception)
            {
                log.Error("Error in BrowseFolder", exception);
            }

            return nodeReferences;
        }

        private List<List<ReferenceDescription>> BrowseAnalogItems(List<ReferenceDescription> nodeReferences)
        {
            var propertyReferences = new List<List<ReferenceDescription>>();

            try
            {
                if (_session == null) return propertyReferences;

                List<BrowseDescription> nodesToBrowse = BrowseDescriptionsFromReferenceDescriptions(nodeReferences);

                propertyReferences = _session.BrowseList(nodesToBrowse);

            }
            catch (Exception exception)
            {
                log.Error("Error Browsing for AnalogItems", exception);
            }

            return propertyReferences;
        }

        private List<BrowseDescription> BrowseDescriptionsFromReferenceDescriptions(List<ReferenceDescription> nodeReferences)
        {
            return nodeReferences
                .Select(nodeRef =>
                {
                    return new BrowseDescription
                    {
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasProperty,
                        IncludeSubtypes = true,
                        NodeId = NodeIdFromENodeId(nodeRef.NodeId)
                    };
                }).ToList();
        }
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

        #region Reading Nodes
        protected List<UaNode> GetAllNodeAttributesInBatches(List<ReferenceDescription> nodeReferences, int batchSize)
        {
            List<UaNode> nodes = new List<UaNode>();

            while (nodeReferences.Any())
            {
                var nodeRefsBatch = nodeReferences.Dequeue(batchSize);

                nodes.AddRange(
                    GetAllNodeAttributes(nodeRefsBatch));
            }

            return nodes;
        }

        protected List<UaNode> GetAllNodeAttributes(List<ReferenceDescription> nodeReferences)
        {
            List<DataValue> browseNameData = GetNodeAttributes(nodeReferences, Attributes.BrowseName);
            List<DataValue> displayNameData = GetNodeAttributes(nodeReferences, Attributes.DisplayName);
            List<DataValue> descriptionData = GetNodeAttributes(nodeReferences, Attributes.Description);

            List<UaNode> nodes = nodeReferences
                .Select(nodeRef => UaNodeFromReferenceDescription(nodeRef))
                .ToList();

            nodes.FillWith(browseNameData, displayNameData, descriptionData);

            return nodes;
        }

        protected List<DataValue> GetNodeAttributes(List<ReferenceDescription> nodeReferences, uint nodeAttribute)
        {
            List<ReadValueId> readValues = nodeReferences
                    .Select(nodeRef => ReadValueIdFromReferenceDescriptions(nodeRef, nodeAttribute))
                    .ToList();

            List<DataValue> result = ReadNodeAttributes(readValues);

            return result;
        }

        protected List<DataValue> ReadNodeAttributes(List<ReadValueId> readValues)
        {
            List<DataValue> results = _session.Read(
                        readValues,
                        0,
                        TimestampsToReturn.Both,
                        new RequestSettings { OperationTimeout = 10 * SECONDS });

            return results;
        }

        protected List<AnalogItemNode> GetAnalogItems(List<ReferenceDescription> nodeReferences)
        {
            var baseAnalogItems = GetAllNodeAttributes(nodeReferences);

            var propertyReferences = BrowseAnalogItems(nodeReferences);

            List<DataValue> euRangeData = GetNodeAttributes(
                propertyReferences.Select(propList => propList.First()).ToList(),
                Attributes.BrowseName);

            List<DataValue> engineeringUnitData = GetNodeAttributes(
                propertyReferences.Select(propList => propList.Last()).ToList(),
                Attributes.BrowseName);

            // Here we are
            // Write a new Fill in method
        }

        #endregion

        #region Helpers
        protected ReadValueId ReadValueIdFromReferenceDescriptions(ReferenceDescription nodeRef, uint attribute)
        {
            ExpandedNodeId ENid = nodeRef.NodeId;
            NodeId nodeId = new NodeId(ENid.IdType, ENid.Identifier, ENid.NamespaceIndex);

            return new ReadValueId()
            {
                NodeId = nodeId,
                AttributeId = attribute
            };
        }

        protected UaNode UaNodeFromReferenceDescription(ReferenceDescription nodeReference)
        {
            ExpandedNodeId nodeId = nodeReference.NodeId;
            return new IP21Tag
            {
                NodeId = new NodeId(nodeId.IdType, nodeId.Identifier, nodeId.NamespaceIndex)
            };
        }

        protected NodeId NodeIdFromENodeId(ExpandedNodeId nodeId)
        {
            return new ExpandedNodeId(nodeId.IdType, nodeId.Identifier, nodeId.NamespaceIndex);
        }
        #endregion

        #region Extensions



        #endregion

        public void GetEventCounts()
        {
            throw new NotImplementedException();
        }
    }
}
