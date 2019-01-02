using Bucket.Listener.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Listener
{
    public class BucketListenerAgentStartup : IListenerAgentStartup
    {
        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}
