using Bucket.EventBus.Common.Events;
using Bucket.Logging.Events;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bucket.Logging.EventHandlers
{
    /// <summary>
    /// 日志数据库存储实现
    /// </summary>
    public class DbLogEventHandler : IEventHandler<LogEvent>
    {
        private readonly DbLogOptions _dbLogOptions;
        public DbLogEventHandler(DbLogOptions dbLogOptions)
        {
            _dbLogOptions = dbLogOptions;
        }

        public bool CanHandle(IEvent @event)
            => @event.GetType().Equals(typeof(LogEvent));

        public async Task<bool> HandleAsync(LogEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_dbLogOptions.IsWriteConsole)
            {
                Console.WriteLine(JsonConvert.SerializeObject(@event));
                return true;
            }
            // 表名
            var tableName = "tb_logs";
            if (_dbLogOptions.IsDbSharding) {
                switch (_dbLogOptions.DbShardingRule)
                {
                    case 1:
                        tableName = tableName + "_" + DbShardingHelper.DayRule(DateTime.Now);
                        break;
                    case 2:
                        tableName = tableName + "_" + DbShardingHelper.MonthRule(DateTime.Now);
                        break;
                }
            }
            var sql = string.Format(@"INSERT INTO {0} (Id,Timestamp,ProjectName, LogTag, LogType, LogMessage, IP) 
                                      VALUES (@Id,@Timestamp,@ProjectName, @LogTag, @LogType, @LogMessage, @IP)"
                                    , tableName);
            using (var connection = new MySqlConnection(_dbLogOptions.ConnectionString))
            {
                await connection.ExecuteAsync(sql, @event);
            }
            return true;
        }

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
            => CanHandle(@event) ? HandleAsync((LogEvent)@event, cancellationToken) : Task.FromResult(false);
    }
}
