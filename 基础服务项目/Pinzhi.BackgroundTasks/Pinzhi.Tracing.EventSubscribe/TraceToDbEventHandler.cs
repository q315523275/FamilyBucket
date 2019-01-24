using Bucket.EventBus.Abstractions;
using Bucket.Tracing.Events;
using Dapper;
using MySql.Data.MySqlClient;
using Pinzhi.Tracing.EventSubscribe.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Bucket.Utility;
using Newtonsoft.Json;
namespace Pinzhi.Tracing.EventSubscribe
{
    public class TraceToDbEventHandler : IIntegrationEventHandler<TracingEvent>
    {
        private readonly TraceDbOptions _traceDbOptions;

        public TraceToDbEventHandler(TraceDbOptions traceDbOptions)
        {
            _traceDbOptions = traceDbOptions;
        }

        public async Task Handle(TracingEvent @event)
        {
            try
            {
                if (@event.TraceSpan.OperationName.StartsWith("httpclient"))
                {
                    var requestTag = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "http.request");
                    var responseTag = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "http.response");
                    var httpRequest = (requestTag != null ? requestTag.Value : string.Empty);
                    var httpResponse = (responseTag != null ? responseTag.Value : string.Empty);
                    if (httpRequest.Length > 5120)
                        httpRequest = httpRequest.Substring(0, 5100);
                    if (httpResponse.Length > 5120)
                        httpResponse = httpResponse.Substring(0, 5100);
                    var model = new {
                        @event.TraceSpan.TraceId,
                        @event.TraceSpan.SpanId,
                        @event.TraceSpan.OperationName,
                        Duration = (@event.TraceSpan.Duration / 1000),
                        StartTimestamp = @event.TraceSpan.StartTimestamp.LocalDateTime,
                        FinishTimestamp = @event.TraceSpan.FinishTimestamp.LocalDateTime,
                        ServiceName = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "service.name").Value.SafeString(),
                        HttpUrl = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "http.url").Value.SafeString(),
                        HttpMethod = @event.TraceSpan?.Tags?.FirstOrDefault(x => x.Key == "http.method").Value.SafeString(),
                        HttpRequest = httpRequest,
                        HttpResponse = httpResponse,
                    };
                    
                    using (var connection = new MySqlConnection(_traceDbOptions.ConnectionString))
                    {
                        await connection.ExecuteAsync(@"INSERT INTO tb_bs_logs (TraceId, SpanId, OperationName, Duration, StartTimestamp, FinishTimestamp, ServiceName, HttpUrl, HttpMethod, HttpRequest, HttpResponse) 
                                      VALUES (@TraceId, @SpanId, @OperationName, @Duration, @StartTimestamp, @FinishTimestamp, @ServiceName, @HttpUrl, @HttpMethod, @HttpRequest, @HttpResponse)", model);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("api请求消费:" + ex.Message);
            }
        }
    }
}
