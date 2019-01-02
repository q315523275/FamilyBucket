using Bucket.SkrTrace.Core.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core.Transport
{
    public class AsyncQueueTraceDispatcher : ITraceDispatcher
    {
        private readonly ConcurrentQueue<TraceSegmentRequest> _segmentQueue;
        private readonly CancellationTokenSource _cancellation;
        private readonly ISkrTraceCollect _skrTraceCollect;
        private readonly SkrTraceOptions _skrTraceOptions;
        public AsyncQueueTraceDispatcher(ISkrTraceCollect skrTraceCollect, IOptions<SkrTraceOptions> skrTraceOptions)
        {
            _skrTraceOptions = skrTraceOptions.Value;
            _skrTraceCollect = skrTraceCollect;
            _segmentQueue = new ConcurrentQueue<TraceSegmentRequest>();
            _cancellation = new CancellationTokenSource();
        }
        public bool Dispatch(TraceSegmentRequest segment)
        {
            if (_skrTraceOptions.PendingSegmentLimit < _segmentQueue.Count || _cancellation.IsCancellationRequested)
            {
                return false;
            }

            _segmentQueue.Enqueue(segment);

            return true;
        }
        public Task Flush(CancellationToken token = default(CancellationToken))
        {
            var limit = _skrTraceOptions.PendingSegmentLimit;
            var index = 0;
            var segments = new List<TraceSegmentRequest>();
            while(index++ < limit && _segmentQueue.TryDequeue(out var request))
            {
                segments.Add(request);
            }

            if (segments.Count > 0)
               _skrTraceCollect.CollectAsync(segments, token);
            return Task.CompletedTask;
        }
        public void Close()
        {
            _cancellation.Cancel();
        }
    }
}
