using Bucket.EventBus.Events;
using System;
namespace Bucket.Logging.Events
{
    public class LogEvent: IntegrationEvent
    {
        public LogEvent(LogInfo logInfo)
        {
            this.LogInfo = logInfo;
        }
        public LogInfo LogInfo { get; set; }
    }
}
