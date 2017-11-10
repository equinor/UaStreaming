using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source
{
    static class UaSourceExtensions
    {
        internal static void FillWith(this List<UaNode> nodes,
                List<DataValue> browseNameData,
                List<DataValue> displayNameData,
                List<DataValue> descriptionData)
        {
            var nodeEnum = nodes.GetEnumerator();
            var browseEnum = browseNameData.GetEnumerator();
            var displayEnum = displayNameData.GetEnumerator();
            var descEnum = descriptionData.GetEnumerator();

            while (nodeEnum.MoveNext() && browseEnum.MoveNext() && displayEnum.MoveNext() && descEnum.MoveNext())
            {
                nodeEnum.Current.BrowseName = browseEnum.Current.WrappedValue.ToString();
                nodeEnum.Current.DisplayName = displayEnum.Current.WrappedValue.ToString();
                nodeEnum.Current.Description = descEnum.Current.WrappedValue.ToString();
            }

        }

        internal static List<T> Dequeue<T>(this List<T> list, int batchSize)
        {
            var dequeued = list.GetRange(0, Math.Min(batchSize, list.Count));
            list.RemoveRange(0, Math.Min(batchSize, list.Count));

            return dequeued;
        }
    }
}
