using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
        /// <summary>
        /// Default password (value: "guest").
        /// </summary>
        /// <remarks>PLEASE KEEP THIS MATCHING THE DOC ABOVE.</remarks>
        public const string DefaultPass = "guest";
        /// <summary>
        /// Default user name (value: "guest").
        /// </summary>
        /// <remarks>PLEASE KEEP THIS MATCHING THE DOC ABOVE.</remarks>
        public const string DefaultUser = "guest";
        /// <summary>
        /// Default virtual host (value: "/").
        /// </summary>
        /// <remarks> PLEASE KEEP THIS MATCHING THE DOC ABOVE.</remarks>
        public const string DefaultVHost = "/";

        /// <summary>The host to connect to.</summary>
        public string HostName { get; set; } = "localhost";
        public int Port { set; get; } = -1;
        public string UserName { get; set; } = DefaultPass;
        public string Password { get; set; } = DefaultUser;
        public string VirtualHost { get; set; } = DefaultVHost;
        public string QueueName { set; get; }
    }
}
