using Bucket.EventBus.Common.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Buried.EventHandler
{
    public class BuriedEventHandler: IEventHandler<BuriedEvent>
    {
        private readonly ILogger logger;
        /// <summary>
        /// 不使用日志防止死循环
        /// </summary>
        /// <param name="logger"></param>
        public BuriedEventHandler(
            ILogger<BuriedEventHandler> logger)
        {
            // this.eventStore = eventStore;
            this.logger = logger;
            // this.logger.LogInformation($"PublishLogEventHandler构造函数调用完成。Hash Code: {this.GetHashCode()}.");
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(BuriedEvent));

        public async Task<bool> HandleAsync(BuriedEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            // this.logger.LogInformation($"开始处理PublishLogEvent事件，处理器Hash Code：{this.GetHashCode()}.");

            // await this.eventStore.SaveEventAsync(@event);
            // 日志逻辑处理

            Console.WriteLine($"埋点消息：{Newtonsoft.Json.JsonConvert.SerializeObject(@event)}");

            // this.logger.LogInformation($"结束处理PublishLogEvent事件，处理器Hash Code：{this.GetHashCode()}.");
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((BuriedEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
