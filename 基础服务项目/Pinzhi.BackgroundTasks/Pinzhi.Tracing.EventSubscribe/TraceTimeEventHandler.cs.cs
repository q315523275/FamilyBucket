using Bucket.EventBus.Abstractions;
using Bucket.Tracing.Events;
using Dapper;
using MySql.Data.MySqlClient;
using Pinzhi.Tracing.EventSubscribe.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pinzhi.Tracing.EventSubscribe
{
    public class TraceTimeEventHandler : IIntegrationEventHandler<TracingEvent>
    {
        private readonly TraceDbOptions _traceDbOptions;

        public TraceTimeEventHandler(TraceDbOptions traceDbOptions)
        {
            _traceDbOptions = traceDbOptions;
        }

        public async Task Handle(TracingEvent @event)
        {
            try
            {
                if (@event.TraceSpan.OperationName.StartsWith("server") && @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "service.name")?.Value == "Pinzhi.ApiGateway")
                {
                    var model = new
                    {
                        Id = System.Guid.NewGuid().ToString("N"),
                        @event.TraceSpan.OperationName,
                        @event.TraceSpan.Duration,
                        StartTimestamp = @event.TraceSpan.StartTimestamp.DateTime,
                        FinishTimestamp = @event.TraceSpan.FinishTimestamp.DateTime,
                        ServiceName = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "service.name")?.Value,
                        ServiceEnvironment = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "service.environment")?.Value,
                        HttpPath = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "http.path")?.Value?.ToLower(),
                    };
                    using (var connection = new MySqlConnection(_traceDbOptions.ConnectionString))
                    {
                        await connection.ExecuteAsync(@"INSERT INTO tb_api_times (Id, OperationName, Duration, StartTimestamp, FinishTimestamp, ServiceName, ServiceEnvironment, HttpPath) 
                                      VALUES (@Id, @OperationName, @Duration, @StartTimestamp, @FinishTimestamp, @ServiceName, @ServiceEnvironment, @HttpPath)", model);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("api耗时统计消费:" + ex.Message);
            }
        }
    }
}
