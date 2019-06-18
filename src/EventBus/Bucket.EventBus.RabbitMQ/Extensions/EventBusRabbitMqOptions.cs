namespace Bucket.EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
        public string ExchangeName { get; set; } = "bucket_event_bus";
        public string HostName { get; set; } = "localhost";
        public int Port { set; get; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string QueueName { set; get; }
        /// <summary>
        /// Qos限速,默认1
        /// </summary>
        public ushort PrefetchCount { set; get; } = 1;
        /// <summary>
        /// 重试策略,默认5
        /// </summary>
        public int RetryCount { set; get; } = 5;
    }
}
