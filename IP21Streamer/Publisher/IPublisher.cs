using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Publisher
{
    interface IPublisher
    {
        void Open();
        void Close();
        void AddToPublishQueue(string message, string dataType);
        void Publish();
    }
}
