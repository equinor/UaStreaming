using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UaStreaming.Contracts
{
    public interface IEventSource
    {
        void StartListening();

        void StopListening();

        void Subscribe(IEnumerable<string> tagNames);

        void UnSubscribe(IEnumerable<string> tagNames);
    }
}
