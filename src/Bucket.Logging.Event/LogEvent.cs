using Bucket.EventBus.Common.Events;
using System;

namespace Bucket.Logging.Events
{
    public class LogEvent: IEvent
    {
        public LogEvent()
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.Now.ToUniversalTime();
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public string ProjectName { get; set; }
        public string LogTag { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
        public string IP { get; set; }
    }
}
