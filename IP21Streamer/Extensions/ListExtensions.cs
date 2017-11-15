using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Extensions
{
    static class ListExtensions
    {
        public static List<T> Copy<T>(this IList<T> listToClone) where T: ICloneable
        {
            return listToClone.ToList();
        }

        internal static List<T> Dequeue<T>(this List<T> list, int batchSize)
        {
            var dequeued = list.GetRange(0, Math.Min(batchSize, list.Count));
            list.RemoveRange(0, Math.Min(batchSize, list.Count));

            return dequeued;
        }
    }

}
