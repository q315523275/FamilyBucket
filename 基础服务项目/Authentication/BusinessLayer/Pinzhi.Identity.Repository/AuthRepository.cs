using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bucket.Config;
using Bucket.EventBus.Abstractions;
using Bucket.Exceptions;
using Bucket.Redis;
using Bucket.Utility;
using Bucket.Utility.Helpers;
using Pinzhi.Identity.Dto.Auth;
using Pinzhi.Identity.Interface;
using Newtonsoft.Json;
using Pinzhi.Identity.Dto.Udc;
using System.Linq;
using Bucket.Authorize;

namespace Pinzhi.Identity.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly RedisClient _redisClient;
        private readonly IConfig _config;
        private readonly IEventBus _eventBus;
        private readonly ITokenBuilder _tokenBuilder;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthRepository(RedisClient redisClient, IConfig config, IEventBus eventBus, ITokenBuilder tokenBuilder, IHttpClientFactory httpClientFactory)
        {
            _redisClient = redisClient;
            _config = config;
            _eventBus = eventBus;
            _tokenBuilder = tokenBuilder;
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 创建Jwt Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public string CreateAccessToken(UserTokenDto userInfo, List<string> roles)
        {
            // 用户基本信息
            var claims = new List<Claim> {
                new Claim("Uid", userInfo.Id.ToString()),
                new Claim("Name", userInfo.RealName.SafeString()),
                new Claim("MobilePhone", userInfo.Mobile.SafeString()),
                new Claim("Email", userInfo.Email.SafeString())
            };
            // 角色数据
            foreach (var info in roles)
            {
                claims.Add(new Claim("scope", info));
            }
            var expires = _config.StringGet("TokenExpires", "4");
            var token = _tokenBuilder.BuildJwtToken(claims, DateTime.UtcNow, DateTime.Now.AddHours(Convert.ToInt32(expires)));
            // accessToken
            return token.TokenValue;
        }
        /// <summary>
        /// 短息发送
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsTemplateName"></param>
        public async Task<string> SendSmsCodeAsync(string mobile, string smsTemplateName)
        {
            var errCountKey = string.Format(CacheKeys.SmsCodeVerifyErr, mobile);
            var sendCountKey = string.Format(CacheKeys.SmsCodeSendIdentity, mobile);
            var loginCodeKey = string.Format(CacheKeys.SmsCodeLoginCode, mobile);

            var rediscontect = _config.StringGet("RedisDefaultServer");
            var redis = _redisClient.GetDatabase(rediscontect, 5);

            // 错误次数过多
            var errCount = await redis.StringGetAsync(errCountKey);
            if (!errCount.IsNullOrEmpty && errCount.SafeString().ToInt() > 5)
                throw new BucketException("GO_0005055", "登陆错误次数过多，请30分钟后再试");

            // 验证一分钟发一条
            if (await redis.KeyExistsAsync(sendCountKey))
                throw new BucketException("GO_2001", "一分钟只能发送一条短信");

            // 生成验证码
            string loginCode = Randoms.CreateRandomValue(6, true);
            // 发送短信
            //Dictionary<string, object> dic = new Dictionary<string, object>
            //{
            //    { "SmsCode", loginCode }
            //};
            // 短发推送
            //_eventBus.Publish(new SmsEvent(dic)
            //{
            //    ChannelType = 0,
            //    MobIp = Web.Ip,
            //    Sender = "6b4b881169e144da9ae93113c0ca41d4",
            //    SmsTemplateId = 2,
            //    SmsTemplateName = smsTemplateName,
            //    Source = "品值GO用户登陆项目",
            //    Mob = mobile,
            //});

            // 基础键值
            // @event.MobIp.Split(',')[0] 当多层代理时x-forwarded-for多ip
            Dictionary<string, object> dic = new Dictionary<string, object>{
                { "channelType", "0" },
                { "smsTemplateId", "2" },
                { "smsTemplateName", smsTemplateName },
                { "source", "品值GO用户登陆项目" },
                { "sender", "6b4b881169e144da9ae93113c0ca41d4" },
                { "mobIp", Web.Ip.Split(',')[0] },
                { "mob", mobile },
                { "SmsCode", loginCode }
            };
            var body = JsonConvert.SerializeObject(dic);
            var apiUrl = _config.StringGet("SmsApiUrl");
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(apiUrl, new StringContent(body, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            // 验证码缓存
            await redis.StringSetAsync(loginCodeKey, loginCode, new TimeSpan(0, 0, 0, 300));
            // 发送人缓存(60秒发一次)
            await redis.StringSetAsync(sendCountKey, loginCode, new TimeSpan(0, 0, 0, 60));
            return loginCode;
        }
        /// <summary>
        /// 验证短信验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public async Task VerifySmsCodeAsync(string mobile, string smsCode)
        {
            // 配置中心验证是否验证码
            var isVerify = _config.StringGet("IsVerifySmsCode");
            if (!isVerify.IsEmpty() && isVerify == "2")
                return;
            // keys
            var errCountKey = string.Format(CacheKeys.SmsCodeVerifyErr, mobile);
            var loginCodeKey = string.Format(CacheKeys.SmsCodeLoginCode, mobile);

            var rediscontect = _config.StringGet("RedisDefaultServer");
            var redis = _redisClient.GetDatabase(rediscontect, 5);

            // 错误次数判断
            var errCount = await redis.StringGetAsync(errCountKey);
            if (!errCount.IsNullOrEmpty && errCount.SafeString().ToInt() > 5)
                throw new BucketException("GO_0005055", "登陆错误次数过多，请30分钟后再试");

            // 短信验证码验证
            var verifyValue = await redis.StringGetAsync(loginCodeKey);
            if (string.IsNullOrWhiteSpace(verifyValue))
                throw new BucketException("GO_0005014", "验证码不存在");

            if (verifyValue.SafeString().ToLower() != smsCode.SafeString().ToLower())
            {
                // 记录错误次数,30分钟
                await redis.StringSetAsync(errCountKey, (errCount.IsNullOrEmpty ? 1 : errCount.SafeString().ToInt() + 1), new TimeSpan(0, 30, 0));
                // 抛出异常
                throw new BucketException("GO_0005015", "验证码错误");
            }
            // 清除次数
            await redis.KeyDeleteAsync(errCountKey);
            await redis.KeyDeleteAsync(loginCodeKey);
        }
    }
}
