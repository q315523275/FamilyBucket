using Bucket.EventBus.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.EventBus.RabbitMQ
{
    public static class EventBusOptionsExtension
    {
        /// <summary>
        /// 使用RabbitMq做事件总线
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static EventBusOptions UseRabbitMQ(this EventBusOptions options, Action<EventBusRabbitMqOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            var setting = new EventBusRabbitMqOptions();
            configure.Invoke(setting);
            options.RegisterExtension(new EventBusRabbitMqOptionsExtension(setting));
            return options;
        }
        /// <summary>
        /// 使用RabbitMq做事件总线
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static EventBusOptions UseRabbitMQ(this EventBusOptions options, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var setting = new EventBusRabbitMqOptions();
            configuration.GetSection("EventBus").GetSection("RabbitMQ").Bind(setting);

            options.RegisterExtension(new EventBusRabbitMqOptionsExtension(setting));

            return options;
        }
    }
}
