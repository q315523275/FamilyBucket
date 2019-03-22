using Bucket.EventBus.Events;
namespace Bucket.Logging.Events
{
    public class LoggerEvent: IntegrationEvent
    {
        public LoggerEvent(LogMessageEntry logInfo)
        {
            this.LogInfo = logInfo;
        }
        public LogMessageEntry LogInfo { get; set; }
    }
}
