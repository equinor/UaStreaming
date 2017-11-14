using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.UaSource
{
    class AnalogItemNode : UaNode
    {
        public DataValue DataValue { get; set; }
        public EUInformation EngineeringUnits { get; set; }
        public Range EuRange { get; set; }

    }
}
