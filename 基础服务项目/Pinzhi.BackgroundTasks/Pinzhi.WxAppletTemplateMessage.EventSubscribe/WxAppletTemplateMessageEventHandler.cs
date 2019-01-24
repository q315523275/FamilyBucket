using Bucket.Config;
using Bucket.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pinzhi.WxAppletTemplateMessage.Event;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pinzhi.WxAppletTemplateMessage.EventSubscribe
{
    public class WxAppletTemplateMessageEventHandler : IIntegrationEventHandler<WxAppletTemplateMessageEvent>
    {
        private readonly IConfig _config;
        private readonly ILogger<WxAppletTemplateMessageEventHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public WxAppletTemplateMessageEventHandler(IConfig config, ILogger<WxAppletTemplateMessageEventHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task Handle(WxAppletTemplateMessageEvent @event)
        {
            var apiUrl = _config.StringGet("WxAppletSendMsgApiUrl");
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                try
                {
                    var body = JsonConvert.SerializeObject(@event);
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PostAsync(apiUrl, new StringContent(body, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (result.IndexOf("000000") == -1)
                            Console.WriteLine($"微信小程序模板消息发送,body:{body}||response:{result}");
                    }
                    else
                    {
                        _logger.LogError("微信小程序模板消息发送非200");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "微信小程序模板消息发送非200");
                }
            }
        }
    }
}
