using IP21Streamer.Publisher;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Extensions
{
    static class ListExtensions
    {
        public static List<T> Copy<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.ToList();
        }

        internal static List<T> Dequeue<T>(this List<T> list, int batchSize)
        {
            var dequeued = list.GetRange(0, Math.Min(batchSize, list.Count));
            list.RemoveRange(0, Math.Min(batchSize, list.Count));

            return dequeued;
        }

        internal static void StuffWith(
            this EventDataBatch eventBatch,
            Queue<DataItem> dataItems)
        {
            while (dataItems.Any())
            {
                var dataItem = dataItems.Peek();
                var eventData = new EventData(Encoding.UTF8.GetBytes(dataItem.Message));
                eventData.Properties.Add("dataType", dataItem.DataType);

                if (eventBatch.TryAdd(eventData)) dataItems.Dequeue();
                else return;
            }
        }
    }

}
