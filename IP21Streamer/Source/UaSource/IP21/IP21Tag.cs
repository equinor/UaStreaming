using IP21Streamer.Source.UaSource;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.IP21
{
    internal class IP21Tag : UaNode
    {
        public string TagName { get; set; }
        public StatusCode SubscriptionStatus { get; set; }
        public AnalogItemNode Measurement { get; set; }

        public IP21Tag() => Measurement = new IP21Measurement();

        public string ToJsonMessage()
        {
            dynamic json = new JObject();
            json.tag = TagName;
            json.time = Measurement.DataValue.SourceTimestamp;
            json.value = Measurement.DataValue.WrappedValue.ToString();
            json.status = Measurement.DataValue.StatusCode.ToString();

            return json.ToString();
        }
    }

}
