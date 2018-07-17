using Microsoft.AspNetCore.Mvc;
using Pinzhi.Identity.DTO.Tool;
using System;
using System.Linq;

namespace Pinzhi.Identity.WebApi.Controllers
{
    /// <summary>
    /// 工具控制器
    /// </summary>
    public class ToolController : Controller
    {
        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Tool/ValidateCode")]
        public IActionResult ValidateCode(int width = 100, int height = 32)
        {
            var code = Bucket.Utility.Helpers.Randoms.CreateRandomValue(4, false);
            var st = Bucket.Utility.Helpers.VerifyCode.CreateByteByImgVerifyCode(code, width, height);
            return File(st, "image/jpeg");
        }
    }
}