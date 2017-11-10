using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;
using IP21Streamer.Source.IP21;
using log4net;

namespace IP21Streamer.Source.IP21
{
    class IP21Source : UaSource
    {

        #region Fields

        static readonly ILog log = LogManager.GetLogger(typeof(IP21Source));

        private const int MAX_NODES_TO_RETURN = 100000;
        private const int BATCH_SIZE = 1000;

        private const IdType ID_TYPE = IdType.String;
        private const string FOLDER_NODEID = "DA: IP_AnalogDef";
        private const ushort NAMESPACE_INDEX = 2;

        List<IP21Tag> foundTags = new List<IP21Tag>();

        #endregion

        #region Construction
        public IP21Source(ApplicationInstance applicationInstance) : base(applicationInstance)
        {
        }
        #endregion

        #region Browse and Update Model
        public override void GetUpdatedModel()
        {

            try
            {
                if (_session == null) return;

                BrowseContext context = new BrowseContext
                {
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    MaxReferencesToReturn = MAX_NODES_TO_RETURN,
                    IncludeSubtypes = true
                };

                NodeId startFolderId = new NodeId(ID_TYPE, FOLDER_NODEID, NAMESPACE_INDEX);
                byte[] continuationPoint = null;

                List<ReferenceDescription> nodeRefs = _session
                    .Browse(
                        startFolderId,
                        context,
                        new RequestSettings { OperationTimeout = 10 * SECONDS},
                        out continuationPoint);

                foundTags = ReadInBatches(nodeRefs, BATCH_SIZE);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private List<IP21Tag> ReadInBatches(List<ReferenceDescription> nodeRefs, int batchSize)
        {
            
            List<IP21Tag> tagsToReturn = new List<IP21Tag>();

            while (nodeRefs.Any())
            {
                var nodeRefsBatch = nodeRefs.Dequeue(batchSize);

                // Read Tag Name and Display Name data
                List<ReadValueId> nodesToRead = nodeRefsBatch
                    .Select(nodeRef => ReadValueIdFromNodeRefs(nodeRef, Attributes.DisplayName))
                    .ToList();

                List<DataValue> displayNameData = ReadNodeAttributes(nodesToRead);

                List<IP21Tag> readTags = new List<IP21Tag>();
                readTags.AddRange(nodesToRead
                    .Select(nodeToRead => TagFromReadValueId(nodeToRead))
                    .ToList());

                readTags.UpdateDisplayNameData(displayNameData);

                // Read Description from Measurement Node
                List<BrowseDescription> measurementBrowseList = MeasurementBrowseListFromTagNodes(nodesToRead);
                List<ReferenceDescription> measurementReferences = _session.BrowseList(measurementBrowseList)
                        .Select(listOfList =>
                        {
                            return listOfList.First();
                        }).ToList();

                nodesToRead.Clear();
                nodesToRead = measurementReferences
                    .Select(nodeRef => ReadValueIdFromNodeRefs(nodeRef, Attributes.Description))
                    .ToList();

                List<DataValue> measurementDescriptions = ReadNodeAttributes(nodesToRead);

                List<IP21Measurement> measurementNodes = nodesToRead
                    .Zip(measurementDescriptions, (readNode, mData) =>
                    {
                        return new IP21Measurement
                        {
                            NodeId = readNode.NodeId,
                            Description = mData.WrappedValue.ToString()
                        };
                    })
                    .ToList();

                readTags.UpdateMeasurementData(measurementNodes);

                tagsToReturn.AddRange(readTags);
            }

            log.Debug($"Last read Tag: " +
                $"Tag: {tagsToReturn.Last().TagName}" +
                $"Description: {tagsToReturn.Last().Measurement.Description}");

            return tagsToReturn;
        }

        private List<BrowseDescription> MeasurementBrowseListFromTagNodes(List<ReadValueId> nodesToRead)
        {
            return nodesToRead.Select(nodeToRead =>
            {
                return new BrowseDescription
                {
                    BrowseDirection = BrowseDirection.Forward,
                    NodeId = nodeToRead.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.HasComponent,
                };
            }).ToList();
        }

        private IP21Tag TagFromReadValueId(ReadValueId nodeToRead)
        {
            NodeId nodeId = nodeToRead.NodeId;
            return new IP21Tag
            {
                NodeId = new NodeId(nodeId.IdType, nodeId.Identifier, nodeId.NamespaceIndex)
            };
        }

        private List<DataValue> ReadNodeAttributes(List<ReadValueId> nodesToRead)
        {
            List<DataValue> results = _session.Read(
                        nodesToRead,
                        0,
                        TimestampsToReturn.Both,
                        new RequestSettings { OperationTimeout = 10 * SECONDS });

            return results;
        }

        private ReadValueId ReadValueIdFromNodeRefs(ReferenceDescription nodeRef, uint attribute)
        {
            ExpandedNodeId ENid = nodeRef.NodeId;
            NodeId nodeId = new NodeId(ENid.IdType, ENid.Identifier, ENid.NamespaceIndex);

            return new ReadValueId()
            {
                NodeId = nodeId,
                AttributeId = attribute
            };
        }

        #endregion
    }
}
