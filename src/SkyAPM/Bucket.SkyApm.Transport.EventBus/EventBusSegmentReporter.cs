using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bucket.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
namespace Bucket.SkyApm.Transport.EventBus
{
    public class EventBusSegmentReporter : ISegmentReporter
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<EventBusSegmentReporter> _logger;

        public EventBusSegmentReporter(IEventBus eventBus, ILogger<EventBusSegmentReporter> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public Task ReportAsync(IReadOnlyCollection<SegmentRequest> segmentRequests, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _eventBus.Publish(new SkyApmTransportEvent { SegmentRequests = segmentRequests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "链路追踪数据EventBus传输失败");
            }
            return Task.CompletedTask;
        }
    }
}
