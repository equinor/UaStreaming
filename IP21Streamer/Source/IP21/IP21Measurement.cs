using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.IP21
{
    class IP21Measurement : UaNode
    {
        public DataValue DataValue { get; set; }
    }
}
