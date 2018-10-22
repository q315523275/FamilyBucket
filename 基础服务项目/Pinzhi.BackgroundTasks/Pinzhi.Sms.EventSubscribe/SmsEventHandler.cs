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
                        { "mobIp", @event.MobIp.Split(',')[0] },
                        { "mob", @event.Mob }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError("短信平台发送非200", ex);
                }
            }
        }
    }
}
