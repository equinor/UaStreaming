using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Source.UaSource
{
    abstract class PropertyNode : UaNode
    {
        protected object _value;

        public abstract object getValue();
        public abstract void setValue(object value);
    }
}
