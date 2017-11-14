using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;
using IP21Streamer.Source.IP21;
using IP21Streamer.Source.UaSource;
using log4net;
using Newtonsoft.Json.Linq;

namespace IP21Streamer.Source.UaSource.IP21
{
    class IP21Source : UaSource
    {

        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(IP21Source));


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

                var nodeReferences = BrowseFolder(new NodeId(ID_TYPE, FOLDER_NODEID, NAMESPACE_INDEX));

                var tagNodes = GetAllNodeAttributesInBatches<IP21Tag>(nodeReferences, BATCH_SIZE);

                var analogReferences = BrowseForAnalogItems(nodeReferences);

                var analogItemNodes = GetAnalogItems(analogReferences, 1000);

                tagNodes.IncludeMeasurements(analogItemNodes);


                //Variant

                PrintDebugTagInfo(tagNodes, "04-FI-080");
                
            }
            catch (Exception e)
            {
                log.Error("Error while retrieving updated model", e);
            }

            Console.WriteLine("Press enter to exit program...");
            Console.ReadLine();
        }



        private List<IP21Tag> ReadInBatches(List<ReferenceDescription> nodeRefs, int batchSize)
        {
            throw new NotImplementedException();
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

        #region Helpers
        private static void PrintDebugTagInfo(List<IP21Tag> nodeList, string identifier)
        {
            var targetNode = nodeList
                    .Where(node => node.DisplayName.Equals(identifier))
                    .First();

            dynamic job = new JObject();
            job.nodeId = targetNode.NodeId.Identifier;
            job.tag = targetNode.DisplayName;
            job.UnitId = targetNode.Measurement.EngineeringUnits.UnitId;
            job.EngineeringUnits = targetNode.Measurement.EngineeringUnits.DisplayName;
            job.EURangeHigh = targetNode.Measurement.EuRange.High;
            job.EURangeLow = targetNode.Measurement.EuRange.Low;


            log.Debug($"last tagNode: \n" +
                $"{job}");
        }
        #endregion
    }
}
