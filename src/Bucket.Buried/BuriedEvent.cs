using System;
using Bucket.EventBus.Common.Events;

namespace Bucket.Buried
{
    public class BuriedEvent : IEvent
    {
        public BuriedEvent(BuriedInformation buriedInformation)
        {
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.buriedInformation = buriedInformation;
        }

        public Guid Id { get; }
        public DateTime Timestamp { get; }
        public BuriedInformation buriedInformation { get; set; }
    }
}
