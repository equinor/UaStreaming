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


        protected const int BATCH_SIZE = 1000;

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

        #region Update Model
        public override void GetUpdatedModel()
        {
            try
            {
                if (_session == null) return;

                var nodeReferences = BrowseFolder(new NodeId(ID_TYPE, FOLDER_NODEID, NAMESPACE_INDEX));

                var analogItemNodes = GetAnalogItems(
                    BrowseForAnalogItems(nodeReferences),
                    BATCH_SIZE);

                var tagNodes = GetAllNodeAttributesInBatches<IP21Tag>(nodeReferences, BATCH_SIZE);

                tagNodes.IncludeMeasurements(analogItemNodes);

                PrintDebugTagInfo(tagNodes, "04-FI-080");

            }
            catch (Exception e)
            {
                log.Error("Error while retrieving updated model", e);
            }

            Console.WriteLine("Press enter to exit program...");
            Console.ReadLine();
        }
        #endregion

        #region
        private void PrintDebugTagInfo(List<IP21Tag> tagNodes, string tagName)
        {
            IP21Tag tagNode = tagNodes.Find(node => node.DisplayName.Equals(tagName));

            dynamic tagObject = new JObject();
            tagObject.tag = tagNode.DisplayName;
            tagObject.nodeId = tagNode.NodeId.Identifier;
            tagObject.description = tagNode.Measurement.Description;
            tagObject.EuRangeLow = tagNode.Measurement.EuRange.Low;
            tagObject.EuRangeHigh = tagNode.Measurement.EuRange.High;
            tagObject.UnitId = tagNode.Measurement.EngineeringUnits.UnitId;
            tagObject.EngineeringUnit = tagNode.Measurement.EngineeringUnits.DisplayName;
        }
        #endregion

    }
}
