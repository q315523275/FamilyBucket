using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Implementation;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.SkrTrace.Core.Service
{
    public class TraceSegmentTransportService : ExecutionService, ITracingContextListener
    {
        private readonly ITraceDispatcher _dispatcher;
        private readonly SkrTraceOptions _skrTraceOptions;
        public TraceSegmentTransportService(ITraceDispatcher dispatcher, IRuntimeEnvironment runtimeEnvironment, IOptions<SkrTraceOptions> skrTraceOptions): base(runtimeEnvironment)
        {
            _dispatcher = dispatcher;
            _skrTraceOptions = skrTraceOptions.Value;
            Period = TimeSpan.FromMilliseconds(_skrTraceOptions.Interval);
            TracingContext.ListenerManager.Add(this);
        }

        protected override TimeSpan DueTime { get; } = TimeSpan.FromSeconds(3);

        protected override TimeSpan Period { get; }
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _dispatcher.Flush(cancellationToken);
        }

        protected override Task Stopping(CancellationToken cancellationToke)
        {
            _dispatcher.Close();
            TracingContext.ListenerManager.Remove(this);
            return Task.CompletedTask;
        }
        public void AfterFinished(ITraceSegment traceSegment)
        {
            _dispatcher.Dispatch(traceSegment.Transform());
        }
    }
}
