using IP21Streamer.Source.UaSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source
{
    interface ISource<T>
    {
        void Connect(string serverUrl, string userName, string password);
        void Disconnect();

        List<T> GetUpdatedModel();
        void Subscribe();
        void UpdateSubscription();

        void GetEventCounts();
    }
}
