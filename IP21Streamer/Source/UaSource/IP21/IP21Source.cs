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
using IP21Streamer.Repository;
using IP21Streamer.Model;

namespace IP21Streamer.Source.UaSource.IP21
{
    class IP21Source : UaSource<IP21Tag>
    {

        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(IP21Source));
        private Action<EventItem> _newEventCallback;

        private const int BATCH_SIZE = 1000;
        private const int SAMPLING_INTERVAL = 5 * 1000;

        private const IdType ID_TYPE = IdType.String;
        private const string FOLDER_NODEID = "DA: IP_AnalogDef";
        private const ushort NAMESPACE_INDEX = 2;

        #endregion

        #region Construction
        public IP21Source(ApplicationInstance applicationInstance, Action<EventItem> newEventCallback) : base(applicationInstance)
        {
            _newEventCallback = newEventCallback;
        }
        #endregion

        #region Update Model
        public override List<IP21Tag> GetUpdatedModel()
        {
            try
            {
                if (_session == null) return null;

                var nodeReferences = BrowseFolder(new NodeId(ID_TYPE, FOLDER_NODEID, NAMESPACE_INDEX));

                var analogItemNodes = GetAnalogItems(
                    BrowseForAnalogItems(nodeReferences),
                    BATCH_SIZE);

                var tagNodes = GetAllNodeAttributesInBatches<IP21Tag>(nodeReferences, BATCH_SIZE);

                tagNodes.IncludeMeasurements(analogItemNodes);

                //PrintDebugTagInfo(tagNodes, "04-FI-080");
                PrintDebugTagInfo(tagNodes.Last());
                return tagNodes;
            }
            catch (Exception e)
            {
                log.Error("Error while retrieving updated model", e);
                throw;
            }

        }
        #endregion

        #region Helpers
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
            tagObject.EngineeringUnit = tagNode.Measurement.EngineeringUnits.DisplayName.Text;

            log.Debug(tagObject.ToString());

        }

        private void PrintDebugTagInfo(IP21Tag tagNode)
        {

            dynamic tagObject = new JObject();
            tagObject.tag = tagNode.DisplayName;
            tagObject.nodeId = tagNode.NodeId.Identifier;
            tagObject.description = tagNode.Measurement.Description;
            tagObject.EuRangeLow = tagNode.Measurement.EuRange.Low;
            tagObject.EuRangeHigh = tagNode.Measurement.EuRange.High;
            tagObject.UnitId = tagNode.Measurement.EngineeringUnits.UnitId;
            tagObject.EngineeringUnit = tagNode.Measurement.EngineeringUnits.DisplayName.Text;

            log.Debug(tagObject.ToString());

        }
        #endregion

        #region Subscriptions
        public override void SubscribeTo(List<TagItem> subscriptionList)
        {
            List<DataMonitoredItem> itemsToMonitor = new List<DataMonitoredItem>();

            itemsToMonitor.AddRange(
                subscriptionList.Select(subItem =>
               {
                   return new DataMonitoredItem(
                       new NodeId(IdType.Opaque, subItem.MeasurementNodeID, NAMESPACE_INDEX))
                   {
                       DataChangeTrigger = DataChangeTrigger.StatusValueTimestamp,
                       SamplingInterval = SAMPLING_INTERVAL,
                       UserData = subItem.Tag
                   };
               }));

            CreateSubscription();
            AddMonitoredItemsToSubscription(itemsToMonitor);
        }

        protected override void Subscription_DataUpdated(Subscription subscription, DataChangedEventArgs args)
        {
            foreach (var change in args.DataChanges)
            {
                EventItem data = new EventItem
                {
                    Tag = change.MonitoredItem.UserData.ToString(),
                    Value = float.Parse(change.Value.Value.ToString()),
                    Timestamp = change.Value.SourceTimestamp,
                    Status = change.Value.StatusCode.Message
                };

                log.Debug($"Event Received: \n" +
                    $"{data.ToJson()}");

                _newEventCallback(data);
            }
        }
        #endregion


    }
}
