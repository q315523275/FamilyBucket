using Bucket.EventBus.Abstractions;
using Pinzhi.Sms.Event;
using System.Threading.Tasks;
using Bucket.Config;
using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Pinzhi.Sms.EventSubscribe
{
    public class SmsEventHandler : IIntegrationEventHandler<SmsEvent>
    {
        private readonly IConfig _config;
        private readonly ILogger<SmsEventHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public SmsEventHandler(IConfig config, ILogger<SmsEventHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 短信事件处理器
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(SmsEvent @event)
        {
            var apiUrl = _config.StringGet("SmsApiUrl");
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                try
                {
                    // 基础键值
                    // @event.MobIp.Split(',')[0] 当多层代理时x-forwarded-for多ip
                    Dictionary<string, object> dic = new Dictionary<string, object>{
                        { "channelType", @event.ChannelType },
                        { "smsTemplateId", @event.SmsTemplateId },
                        { "smsTemplateName", @event.SmsTemplateName },
                        { "source", @event.Source },
                        { "sender", @event.Sender },
                        { "mobIp", @event.MobIp.Split(',')[0] },
                        { "mob", @event.Mob }
                    };
                    // 合并键值
                    foreach (var info in @event.Parameter)
                    {
                        dic[info.Key] = info.Value;
                    }
                    var body = JsonConvert.SerializeObject(dic);
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.PostAsync(apiUrl, new StringContent(body, Encoding.UTF8, "application/json"));
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"短信消费发送,手机号:{@event.Mob},时间{DateTime.Now.ToString()}");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "短信平台发送非200");
                }
            }
        }
    }
}
