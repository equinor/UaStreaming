using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EventPackage
    {
        public EventVqt eventVqt { get; private set; }
        public string plant { get; private set; }
        public string eventType { get; private set; }
        public List<string> recipients { get; set; }

        public EventPackage(EventVqt eventVqt, string plantCode, string eventType, List<string> recipients)
        {
            this.eventVqt = eventVqt;
            this.plant = plantCode;
            this.eventType = eventType;
            this.recipients = recipients;
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.None);
        public string ToPrettyJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
