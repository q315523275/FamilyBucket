using Bucket.EventBus.Abstractions;
using Bucket.SkrTrace.Transport.EventBus;
using Bucket.Utility;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bucket.SkrTrace.WebSocketServer.Handler
{
    public class SkrTraceWeSocketServerHandler : IIntegrationEventHandler<SkrTraceTransportEvent>
    {
        private readonly SkrTraceMessageHandler _skrTraceMessageHandler;

        public SkrTraceWeSocketServerHandler(SkrTraceMessageHandler skrTraceMessageHandler)
        {
            _skrTraceMessageHandler = skrTraceMessageHandler;
        }

        public async Task Handle(SkrTraceTransportEvent @event)
        {
            foreach (var request in @event.TraceSegmentRequest)
            {
                if (!string.IsNullOrWhiteSpace(request.Segment.Identity))
                {
                    foreach(var item in request.Segment.Spans)
                    {
                        if (item.SpanLayer == 3) // http
                        {
                            var requestMessage = item.Tags.FirstOrDefault(it => it.Key == "http.request");
                            var data = JsonConvert.SerializeObject(new
                            {
                                request.Segment.ApplicationCode,
                                item.Component,
                                StartTime = item.StartTime.ToUnixTimeMilliseconds(),
                                Duration = item.Duration.ToString("0.000"),
                                Time = item.StartTime.LocalDateTime.ToDateTimeString() + "/" + item.EndTime.LocalDateTime.ToDateTimeString(),
                                item.OperationName,
                                item.ParentSpanId,
                                item.SpanId,
                                SpanLayer = "http",
                                SpanType = (item.SpanType == 0 ? "entry" : "exit"),
                                HttpUrl = item.Tags?.FirstOrDefault(it => it.Key == "http.url").Value,
                                HttpMethod = item.Tags?.FirstOrDefault(it => it.Key == "http.method").Value,
                                HttpStatusCode = item.Tags?.FirstOrDefault(it => it.Key == "http.status_code").Value,
                                HttpRequest = item.Tags.FirstOrDefault(it => it.Key == "http.request").Value,
                                HttpResponse = item.Tags.FirstOrDefault(it => it.Key == "http.response").Value,
                            });
                            await _skrTraceMessageHandler.SendMessageToGroupAsync(request.Segment.Identity, new WebSocketManager.Message { MessageType = WebSocketManager.MessageType.Text, Data = data });
                        }
                    }
                }
            }
        }
    }
}
