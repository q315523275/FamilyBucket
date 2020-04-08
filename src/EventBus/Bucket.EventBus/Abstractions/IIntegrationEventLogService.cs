using Bucket.EventBus.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.EventBus.Abstractions
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string module);
        Task SaveEventAsync(IntegrationEvent @event, object transaction = null);
        Task MarkEventAsPublishedAsync(long eventId);
        Task MarkEventAsInProgressAsync(long eventId);
        Task MarkEventAsFailedAsync(long eventId);
    }
}