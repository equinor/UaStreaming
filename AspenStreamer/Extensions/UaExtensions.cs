using AspenStreamer.KDI;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace AspenStreamer.Extensions
{
    internal static class UaExtensions
    {
        internal static List<ReferenceDescription> FilterOnTagMatch(this List<ReferenceDescription> references, string pattern)
        {
            Regex regex = new Regex(pattern);

            return references
                .Where(reference => regex.IsMatch(reference.BrowseName.Name)).ToList();
        }

        internal static List<ReferenceDescription> LimitMatches(this List<ReferenceDescription> references, int limit)
        {
            return references
                .Take(limit)
                .ToList();
        }

        internal static List<KSpiceVariableData> ExtractKSpiceVariabeInfo(this List<ReferenceDescription> references, string plantCode)
        {
            Regex regex = new Regex(@"^(?<prefix>[^-]+)-(?<equipment>[^:]+):(?<suffix>[\w]+)$");

            return references
                .Select(reference =>
                {
                    var match = regex.Match(reference.BrowseName.Name);
                    var prefixMatch = match.Groups["prefix"].Value;
                    var equipmentMatch = match.Groups["equipment"].Value;
                    var suffixMatch = match.Groups["suffix"].Value;

                    return new KSpiceVariableData
                    {
                        namespaceIndex = reference.BrowseName.NamespaceIndex,
                        browseName = reference.BrowseName.Name,
                        nodeId = reference.NodeId.GetNodeIdFromENodeId(),
                        prefix = prefixMatch,
                        equipment = equipmentMatch,
                        suffix = suffixMatch,
                        tagName = plantCode + "." + equipmentMatch + ":" + suffixMatch
                    };
                })
                .ToList();
        }

        internal static NodeId GetNodeIdFromENodeId(this ExpandedNodeId expandedNodeId)
        {
            return new NodeId(expandedNodeId.IdType, expandedNodeId.Identifier, expandedNodeId.NamespaceIndex);
        }

        internal static DataMonitoredItem BuildMonitoredItem(this KSpiceVariableData info, DataChangeTrigger trigger, double samplingInterval)
        {
            return new DataMonitoredItem(info.nodeId)
            {
                DataChangeTrigger = trigger,
                SamplingInterval = samplingInterval,
                UserData = info
            };
        }
    }
}
