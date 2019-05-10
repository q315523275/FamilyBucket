using Bucket.EventBus.Util;
using System;

namespace Bucket.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = SnowflakeId.Default().NextId();
            CreationDate = DateTime.Now;
        }

        public long Id { get; }
        public DateTime CreationDate { get; }
    }
}
