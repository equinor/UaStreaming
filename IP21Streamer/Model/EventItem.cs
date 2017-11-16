using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Model
{
    class EventItem
    {
        public string Tag { get; set; }
        public float Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
