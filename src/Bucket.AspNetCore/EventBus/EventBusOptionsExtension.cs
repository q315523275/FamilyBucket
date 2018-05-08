using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.AspNetCore.EventBus
{
    public static class EventBusOptionsExtension
    {
        public static EventBusOptions UseRabbitMQ(this EventBusOptions options, Action<EventBusRabbitMQOptions> configure)
        {

            if (configure == null) throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new EventBusRabbitMQOptionsExtension(configure));

            return options;
        }
    }
}
