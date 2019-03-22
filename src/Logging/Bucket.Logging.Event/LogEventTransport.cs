using Bucket.EventBus.Abstractions;

namespace Bucket.Logging.Events
{
    public class LogEventTransport : ILoggerTransport
    {
        private readonly IEventBus _eventBus;
        public LogEventTransport(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public void Publish(LogMessageEntry logs)
        {
            _eventBus.Publish(new LoggerEvent(logs));
        }
    }
}
