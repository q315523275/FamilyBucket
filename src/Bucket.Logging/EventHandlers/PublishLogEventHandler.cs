using Bucket.EventBus.Common.Events;
using Bucket.Logging.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Logging.EventHandlers
{
    public class PublishLogEventHandler: IEventHandler<PublishLogEvent>
    {
        private readonly IEventStore eventStore;
        private readonly ILogger logger;
        /// <summary>
        /// 不使用日志防止死循环
        /// </summary>
        /// <param name="logger"></param>
        public PublishLogEventHandler(
            ILogger<PublishLogEventHandler> logger)
        {
            // this.eventStore = eventStore;
            this.logger = logger;
            // this.logger.LogInformation($"PublishLogEventHandler构造函数调用完成。Hash Code: {this.GetHashCode()}.");
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(PublishLogEvent));

        public async Task<bool> HandleAsync(PublishLogEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            // this.logger.LogInformation($"开始处理PublishLogEvent事件，处理器Hash Code：{this.GetHashCode()}.");

            // await this.eventStore.SaveEventAsync(@event);
            // 日志逻辑处理

            Console.WriteLine($"消息Id：{@event.Id},消息类型{@event.LogType},消息内容:{@event.LogMessage}");

            // this.logger.LogInformation($"结束处理PublishLogEvent事件，处理器Hash Code：{this.GetHashCode()}.");
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((PublishLogEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
