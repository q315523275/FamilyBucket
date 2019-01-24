using System;
using System.Threading.Tasks;
using Bucket.DbContext;
using Bucket.EventBus.Abstractions;
using Bucket.Logging.Events;

namespace Pinzhi.Logging.EventSubscribe
{
    /// <summary>
    /// 日志数据库存储实现
    /// </summary>
    public class ErrorLogEventHandler : IIntegrationEventHandler<LogEvent>
    {
        private readonly IDbRepository<ErrorLogInfo> _dbRepository;

        public ErrorLogEventHandler(IDbRepository<ErrorLogInfo> dbRepository)
        {
            _dbRepository = dbRepository;
        }
        /// <summary>
        /// 日志事件处理器
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(LogEvent @event)
        {
            try
            {
                _dbRepository.UseDb("log").Insert(new ErrorLogInfo
                {
                    Id = @event.Id,
                    AddTime = @event.CreationDate,
                    ClassName = @event.LogInfo.ClassName,
                    IP = @event.LogInfo.IP,
                    LogMessage = @event.LogInfo.LogMessage,
                    LogTag = @event.LogInfo.LogTag,
                    LogType = @event.LogInfo.LogType,
                    ProjectName = @event.LogInfo.ProjectName
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志消费:" + ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
