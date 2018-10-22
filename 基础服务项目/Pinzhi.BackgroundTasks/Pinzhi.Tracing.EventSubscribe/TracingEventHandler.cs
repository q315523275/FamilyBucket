using System;
using System.Threading.Tasks;
using Bucket.Tracing.Events;
using Pinzhi.Tracing.EventSubscribe.Elasticsearch;
using System.Collections.Generic;
using Bucket.Tracing.DataContract;
using Bucket.EventBus.Abstractions;
using System.Linq;

namespace Pinzhi.Tracing.EventSubscribe
{
    public class TracingEventHandler : IIntegrationEventHandler<TracingEvent>
    {
        private readonly ISpanStorage _spanStorage;
        private readonly IServiceStorage _serviceStorage;
        public TracingEventHandler(ISpanStorage spanStorage, IServiceStorage serviceStorage)
        {
            _spanStorage = spanStorage;
            _serviceStorage = serviceStorage;
        }

        public async Task Handle(TracingEvent @event)
        {
            try
            {
                /// 需要增加缓存区
                var span = @event.TraceSpan;
                if (span != null)
                {
                    await _spanStorage.StoreAsync(span);
                    var service = new Service { Name = span?.Tags?.FirstOrDefault(x => x.Key == "service.name")?.Value };
                    await _serviceStorage.StoreServiceAsync(new List<Service> { service });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Tracing消费异常:" + ex.Message);
            }
        }
    }
}
