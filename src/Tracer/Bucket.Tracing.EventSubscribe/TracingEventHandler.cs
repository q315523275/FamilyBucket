using Bucket.EventBus.Events;
using Bucket.EventBus.Attributes;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Bucket.Tracing.Events;
using Bucket.Tracing.EventSubscribe.Elasticsearch;
using System.Collections.Generic;
using Bucket.Tracing.DataContract;
using Bucket.EventBus.Abstractions;

namespace Bucket.Tracing.EventSubscribe
{
    [QueueConsumer("Bucket.Tracing.Event")]
    public class TracingEventHandler : IIntegrationEventHandler<TracingEvent>
    {
        private readonly ISpanStorage _spanStorage;
        public TracingEventHandler(ISpanStorage spanStorage)
        {
            _spanStorage = spanStorage;
        }

        public async Task Handle(TracingEvent @event)
        {
            try
            {
                ///// 需要增加缓存区
                //var span = @event.TraceSpan;
                //if (span != null)
                //{
                //    //var StartTime = DateTime.Now;
                //    await _spanStorage.StoreAsync(new List<Span> { span });
                //    //var TimeLength = Math.Round((DateTime.Now - StartTime).TotalMilliseconds, 4);
                //    //Console.WriteLine("Elasticsearch数据创建耗时" + TimeLength + "毫秒");
                //}
                Thread.Sleep(2000);
                Console.WriteLine("Trace数据消费，线程ID:"+ Thread.CurrentThread.ManagedThreadId + ",时间："+ DateTime.Now.ToString());
            }
            catch(Exception ex)
            {
                Console.WriteLine("Tracing消费:" + ex.Message);
            }
        }
    }
}
