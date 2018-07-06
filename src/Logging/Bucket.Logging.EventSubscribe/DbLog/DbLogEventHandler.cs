using System;
using System.Threading;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Dapper;

using Bucket.Logging.Events;
using Bucket.EventBus.Attributes;
using Bucket.EventBus.Abstractions;

namespace Bucket.Logging.EventSubscribe
{
    /// <summary>
    /// 日志数据库存储实现
    /// </summary>
    public class DbLogEventHandler : IIntegrationEventHandler<LogEvent>
    {
        private readonly DbLogOptions _dbLogOptions; // 日志数据库参数
        private readonly string tableName = "tb_logs"; // 日志数据库表名
        private readonly string sql = ""; // 日志sql语句
        public DbLogEventHandler(DbLogOptions dbLogOptions)
        {
            _dbLogOptions = dbLogOptions;
            if (_dbLogOptions.IsDbSharding)
            {
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
            sql = string.Format(@"INSERT INTO {0} (Id,AddTime,ClassName,ProjectName, LogTag, LogType, LogMessage, IP) 
                                      VALUES (@Id,@AddTime,@ClassName,@ProjectName, @LogTag, @LogType, @LogMessage, @IP)"
                        , tableName);
        }
        /// <summary>
        /// 日志事件处理器
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(LogEvent @event)
        {
            try
            {
                if (_dbLogOptions.IsWriteConsole)
                {
                    Console.WriteLine($"日志::LogMessage:{@event.LogInfo.LogMessage},ProejectName:{@event.LogInfo.ProjectName},ClassName:{@event.LogInfo.ClassName},Ip:{@event.LogInfo.IP},CreateTime:{@event.LogInfo.AddTime}");
                }
                var model = @event.LogInfo;
                using (var connection = new MySqlConnection(_dbLogOptions.ConnectionString))
                {
                    await connection.ExecuteAsync(sql, model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("日志消费:" + ex.Message + ex.InnerException.Message + ex.StackTrace);
            }
        }
    }
}
