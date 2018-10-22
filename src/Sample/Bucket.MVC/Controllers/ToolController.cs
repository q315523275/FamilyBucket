using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bucket.Utility;
namespace Bucket.MVC.Controllers
{
    public class ToolController : Controller
    {
        public IActionResult Index()
        {
            var code = Utility.Helpers.VerifyCode.CreateVerifyCode(Utility.Helpers.VerifyCode.VerifyCodeType.MixVerifyCode);
            var p = Utility.Helpers.VerifyCode.CreateByteByImgVerifyCode(code, 80, 38);
            return File(p, "image/Jpeg");
        }
    }
}