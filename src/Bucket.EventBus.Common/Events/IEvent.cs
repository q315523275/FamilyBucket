using System;

namespace Bucket.EventBus.Common.Events
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}
