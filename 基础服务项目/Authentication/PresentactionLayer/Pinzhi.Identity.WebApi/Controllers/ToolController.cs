using Bucket.Config;
using Bucket.Redis;
using Bucket.Utility;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Identity.Dto.Tool;
using System;
using System.Linq;
using Bucket.Exceptions;
namespace Pinzhi.Identity.WebApi.Controllers
{
    /// <summary>
    /// 工具控制器
    /// </summary>
    public class ToolController : Controller
    {
        private readonly RedisClient _redisClient;
        private readonly IConfig _config;
        public ToolController(IConfig config, RedisClient redisClient)
        {
            _config = config;
            _redisClient = redisClient;
        }

        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("/ValidateCode")]
        public IActionResult ValidateCode(string guid, int width = 100, int height = 32)
        {
            if (guid.IsEmpty())
                throw new BucketException("pz_001", "请输入用户标识");
            var rediscontect = _config.StringGet("RedisDefaultServer");
            var redis = _redisClient.GetDatabase(rediscontect, 5);
            var code = Bucket.Utility.Helpers.Randoms.CreateRandomValue(4, false);
            redis.StringSet($"ImgCode:{guid}", code, new TimeSpan(0, 0, 0, 300));
            var st = Bucket.ImgVerifyCode.VerifyCode.CreateByteByImgVerifyCode(code, width, height);
            return File(st, "image/jpeg");
        }
    }
}