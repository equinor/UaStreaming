using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EventVqt
    {
        public string tag;
        public DateTime time;
        public double value;
        public int status;

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.None);
    }
}
