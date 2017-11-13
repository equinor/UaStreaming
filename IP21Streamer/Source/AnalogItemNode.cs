using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Source
{
    class AnalogItemNode : UaNode
    {
        public string EngineeringUnits { get; set; }
        public string EuRange { get; set; }


        public class EURange
        {
            public int High { get; set; }
            public int Low { get; set; }
        }
    }
}
