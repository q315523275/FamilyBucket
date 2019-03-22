using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Logging
{
    public class AsyncQueueLoggerDispatcher : ILoggerDispatcher
    {
        private readonly ConcurrentQueue<LogMessageEntry> _messageQueue;
        private readonly CancellationTokenSource _cancellation;
        public AsyncQueueLoggerDispatcher()
        {
            _messageQueue = new ConcurrentQueue<LogMessageEntry>();
            _cancellation = new CancellationTokenSource();
        }
        public bool Dispatch(LogMessageEntry logMessage)
        {
            if (_cancellation.IsCancellationRequested)
                return false;
            _messageQueue.Enqueue(logMessage);
            return true;
        }
        public Task Flush(ILoggerTransport loggerTransport, CancellationToken token = default)
        {
            while (_messageQueue.TryDequeue(out var message))
            {
                loggerTransport.Publish(message);
            }
            return Task.CompletedTask;
        }
        public void Close()
        {
            _cancellation.Cancel();
        }
    }
}
