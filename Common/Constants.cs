using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {
        #region Constants
        public const int Seconds = 1000;
        public const long MaxByteSize = 256000;

        public const int Good = 192;
        public const int Bad = 0;
        public const int Questionable = 64;

        public const string RealTime = "realTime";

        public const int UaMaxKeepAliveTime = 10 * Seconds;
        public const int UaLifeTime = 20 * Seconds;

        public const string KSpiceRoot = "M|KSpiceApisBee";
        public const ushort KSpiceNameSpace = 2;
        public const int KSpiceOperationTimeOut = 20 * Seconds;
        public const int KSpiceMaxNodesReturned = 100000;
        public const int KSpiceBufferCapacity = 200000;
        #endregion
    }
}
