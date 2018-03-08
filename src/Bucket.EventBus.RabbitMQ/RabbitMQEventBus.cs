using Bucket.EventBus.Common.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : BaseEventBus
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly IBasicProperties props;
        private readonly string exchangeName;
        private readonly string exchangeType;
        private readonly string queueName;
        private readonly bool autoAck;
        private readonly ILogger logger;
        private bool disposed;

        public RabbitMQEventBus(IConnectionFactory connectionFactory,
            ILogger<RabbitMQEventBus> logger,
            IEventHandlerExecutionContext context,
            string exchangeName,
            string exchangeType = ExchangeType.Fanout,
            string queueName = null,
            bool autoAck = false)
            : base(context)
        {
            this.connectionFactory = connectionFactory;
            this.logger = logger;
            this.connection = this.connectionFactory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.autoAck = autoAck;

            this.channel.ExchangeDeclare(this.exchangeName, this.exchangeType);

            props = channel.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2;

            this.queueName = this.InitializeEventConsumer(queueName);

            logger.LogInformation($"RabbitMQEventBus构造函数调用完成。Hash Code：{this.GetHashCode()}.");
        }

        public override Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.connection.IsOpen)
            {
                var json = JsonConvert.SerializeObject(@event, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                var eventBody = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(this.exchangeName,
                    @event.GetType().FullName,
                    props,
                    eventBody);
            }
            return Task.CompletedTask;
        }

        public override void Subscribe<TEvent, TEventHandler>()
        {
            if (!this.eventHandlerExecutionContext.HandlerRegistered<TEvent, TEventHandler>())
            {
                this.eventHandlerExecutionContext.RegisterHandler<TEvent, TEventHandler>();
                this.channel.QueueBind(this.queueName, this.exchangeName, typeof(TEvent).FullName);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.channel.Dispose();
                    this.connection.Dispose();

                    logger.LogInformation($"RabbitMQEventBus已经被Dispose。Hash Code:{this.GetHashCode()}.");
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }

        private string InitializeEventConsumer(string queue)
        {
            var localQueueName = queue;
            if (string.IsNullOrEmpty(localQueueName))
            {
                localQueueName = this.channel.QueueDeclare().QueueName;
            }
            else
            {
                this.channel.QueueDeclare(localQueueName, true, false, false, null);
            }

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += async (model, eventArgument) =>
            {
                var eventBody = eventArgument.Body;
                var json = Encoding.UTF8.GetString(eventBody);
                var @event = (IEvent)JsonConvert.DeserializeObject(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                await this.eventHandlerExecutionContext.HandleEventAsync(@event);
                if (!autoAck)
                {
                    channel.BasicAck(eventArgument.DeliveryTag, false);
                }
            };

            this.channel.BasicConsume(localQueueName, autoAck: this.autoAck, consumer: consumer);

            return localQueueName;
        }
    }
}
