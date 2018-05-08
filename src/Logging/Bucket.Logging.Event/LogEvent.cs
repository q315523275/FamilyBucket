using Bucket.EventBus.Common.Events;
using System;
namespace Bucket.Logging.Events
{
    public class LogEvent: IEvent
    {
        public LogEvent(LogInfo logInfo)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.Now.ToUniversalTime();
            this.LogInfo = logInfo;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public LogInfo LogInfo { get; set; }
    }
}
