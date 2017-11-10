using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;

namespace IP21Streamer.Source
{
    interface ISource
    {
        void Connect(string serverUrl, string userName, string password);
        void Disconnect();

        void GetUpdatedModel();
        void Subscribe();
        void UpdateSubscription();

        void GetEventCounts();
    }
}
