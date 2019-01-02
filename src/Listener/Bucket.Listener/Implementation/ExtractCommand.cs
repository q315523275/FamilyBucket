using Bucket.Listener.Abstractions;
using Bucket.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Listener.Implementation
{
    public class ExtractCommand : IExtractCommand
    {
        private readonly IEnumerable<IBucketListener> _bucketListeners;

        public ExtractCommand(IEnumerable<IBucketListener> bucketListeners)
        {
            _bucketListeners = bucketListeners;
        }

        public async Task ExtractCommandMessage(NetworkCommand command)
        {
            foreach (var listener in _bucketListeners)
            {
                if (listener.ListenerName == command.NotifyComponent)
                    await listener.ExecuteAsync(command.CommandText);
            }
        }
    }
}
