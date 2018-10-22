using System;

namespace Bucket.EventBus.Events
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }

        public Guid Id { get; }
        public DateTime CreationDate { get; }
    }
}
