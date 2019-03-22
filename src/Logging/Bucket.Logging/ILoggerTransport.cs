namespace Bucket.Logging
{
    public interface ILoggerTransport
    {
        void Publish(LogMessageEntry logMessageEntry);
    }
}
