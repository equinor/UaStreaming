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
    }
}
