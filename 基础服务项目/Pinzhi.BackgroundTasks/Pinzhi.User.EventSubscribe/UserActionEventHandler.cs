using System.Threading.Tasks;
using Bucket.EventBus.Abstractions;
using Pinzhi.User.Event;
using Bucket.DbContext;
using System;
using Microsoft.Extensions.Logging;
namespace Pinzhi.User.EventSubscribe
{
    public class UserActionEventHandler : IIntegrationEventHandler<UserActionEvent>
    {
        private readonly IDbRepository<UserEventInfo> _dbRepository;
        private readonly ILogger<UserActionEventHandler> _logger;

        public UserActionEventHandler(IDbRepository<UserEventInfo> dbRepository, ILogger<UserActionEventHandler> logger)
        {
            _dbRepository = dbRepository;
            _logger = logger;
        }

        public Task Handle(UserActionEvent @event)
        {
            try
            {
                _dbRepository.UseDb("super").Insert(new UserEventInfo
                {
                    Channel = @event.Channel,
                    CreateTime = @event.CreationDate,
                    EventCode = @event.EventCode,
                    EventKey = @event.EventKey,
                    EventName = @event.EventName,
                    EventValue = @event.EventValue,
                    Id = @event.Id,
                    Mode = @event.Mode,
                    Source = @event.Source,
                    UserKey = @event.UserKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户行为事件消费");
            }
            return Task.CompletedTask;
        }
    }
}
