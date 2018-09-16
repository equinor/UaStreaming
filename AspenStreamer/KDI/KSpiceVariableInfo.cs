using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace AspenStreamer.KDI
{
    internal class KSpiceVariableData
    {
        public ushort namespaceIndex { get; set; }
        public string browseName { get; set; }
        public NodeId nodeId { get; set; }
        public string prefix { get; set; }
        public string equipment { get; set; }
        public string suffix { get; set; }
        public string tagName { get; set; }
    }
}
