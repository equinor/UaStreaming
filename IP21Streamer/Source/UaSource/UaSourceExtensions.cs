using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.UaSource
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

        internal static void FillWith<T>(this List<T> nodes,
                List<DataValue> browseNameData,
                List<DataValue> displayNameData,
                List<DataValue> descriptionData) where T: UaNode
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

        internal static void FillWith(this List<AnalogItemNode> analogItems, List<List<DataValue>> properties)
        {
            var analogEnum = analogItems.GetEnumerator();
            var propEnum = properties.GetEnumerator();

            while (analogEnum.MoveNext() && propEnum.MoveNext())
            {
                foreach (var property in propEnum.Current)
                {
                    var extObject = (property.Value as ExtensionObject).Body;

                    if (extObject is Range)
                    {
                        analogEnum.Current.EuRange = extObject as Range;
                    }
                    else if (extObject is EUInformation)
                    {
                        analogEnum.Current.EngineeringUnits = extObject as EUInformation;
                    }
                }
            }
        }

        internal static void IncludeMeasurements(this List<IP21Tag> tagNodes, List<AnalogItemNode> analogItemNodes)
        {
            var tagEnum = tagNodes.GetEnumerator();
            var analogEnum = analogItemNodes.GetEnumerator();

            while (tagEnum.MoveNext() && analogEnum.MoveNext())
            {
                tagEnum.Current.Measurement = analogEnum.Current;
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
