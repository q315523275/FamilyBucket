using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bucket.Config;
using Bucket.EventBus.Abstractions;
using Bucket.Exceptions;
using Bucket.Redis;
using Bucket.Utility;
using Bucket.Utility.Helpers;
using Pinzhi.Identity.Dto.Auth;
using Pinzhi.Identity.Interface;
using Pinzhi.Identity.Model;

namespace Pinzhi.Identity.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PermissionRequirement _requirement;
        private readonly RedisClient _redisClient;
        private readonly IConfig _config;
        private readonly IEventBus _eventBus;

        public AuthRepository(PermissionRequirement requirement, RedisClient redisClient, IConfig config, IEventBus eventBus)
        {
            _requirement = requirement;
            _redisClient = redisClient;
            _config = config;
            _eventBus = eventBus;
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
                claims.Add(new Claim(ClaimTypes.Role, info));
                claims.Add(new Claim("scope", info));
            }
            // 用户身份标识
            var identity = new ClaimsIdentity();
            identity.AddClaims(claims);
            // accessToken
            return JwtToken.BuildJwtToken(claims, _requirement);
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
            Dictionary<string, object> dic = new Dictionary<string, object>
            {
                { "SmsCode", loginCode }
            };
            // 短发推送
            // _eventBus.Publish();

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
