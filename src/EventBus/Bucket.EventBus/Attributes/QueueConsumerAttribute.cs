using System;

namespace Bucket.EventBus.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class QueueConsumerAttribute : Attribute
    {
        public string QueueName
        {
            get { return _queueName; }
        }
        public int PrefetchCount
        {
            get { return _prefetchCount; }
        }

        private string _queueName { get; set; }
        private int _prefetchCount { get; set; }
        public QueueConsumerAttribute(string queueName)
        {
            _queueName = queueName;
        }
        public QueueConsumerAttribute(string queueName, int prefetchCount = 1)
        {
            _queueName = queueName;
            _prefetchCount = prefetchCount;
        }
    }
}
