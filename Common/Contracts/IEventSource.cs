using System.Collections.Generic;

namespace Common.Contracts
{
    public interface IEventSource
    {
        void StartListening();

        void StopListening();

        void Subscribe(IEnumerable<string> tagNames);

        void UnSubscribe(IEnumerable<string> tagNames);
    }
}
