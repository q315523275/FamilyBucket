using Bucket.EventBus.Common.Events;
using System;

namespace Bucket.Logging.Events
{
    public class PublishLogEvent : IEvent
    {
        public PublishLogEvent(string logType,string logMessage)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.LogType = logType;
            this.LogMessage = logMessage;
            this.IP = DnsHelper.GetIpAddressAsync().GetAwaiter().GetResult();
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public string LogType { get; }
        public string LogMessage { get; }
        public string IP { get; }
    }
}
