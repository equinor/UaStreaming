using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaSchema;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.UaSource
{
    class EuRangeProperty : PropertyNode
    {
        public override object getValue()
        {
            return _value;
        }

        public override void setValue(object value)
        {
            if (value is Range) _value = value;
            else _value = null;
        }
    }
}
