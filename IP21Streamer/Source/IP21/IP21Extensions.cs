using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source.IP21
{
    internal static class ListExtensions
    {
        internal static void UpdateDisplayNameData(this List<IP21Tag> tags, List<DataValue> displayNameData)
        {
            var tagEnum = tags.GetEnumerator();
            var dispEnum = displayNameData.GetEnumerator();

            while (tagEnum.MoveNext() && dispEnum.MoveNext())
            {
                tagEnum.Current.DisplayName = dispEnum.Current.WrappedValue.ToString();
                tagEnum.Current.TagName = dispEnum.Current.WrappedValue.ToString();
            }
        }

        internal static void UpdateMeasurementData(this List<IP21Tag> tags, List<IP21Measurement> measurementData)
        {
            var tagEnum = tags.GetEnumerator();
            var measureEnum = measurementData.GetEnumerator();

            while (tagEnum.MoveNext() && measureEnum.MoveNext())
            {
                tagEnum.Current.Measurement = measureEnum.Current;
            }
        }

        internal static List<T> Dequeue<T>(this List<T> list, int batchSize)
        {
            List<T> result = list.GetRange(0, Math.Min(batchSize, list.Count - 1));
            list.RemoveRange(0, Math.Min(batchSize, list.Count - 1));

            return result;
        }
    }
}
