using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace Bucket.MVC.Controllers
{
    public class ToolController : Controller
    {
        public IActionResult Index()
        {
            var code = Bucket.ImgVerifyCode.VerifyCode.CreateVerifyCode(Bucket.ImgVerifyCode.VerifyCode.VerifyCodeType.MixVerifyCode);
            var p = Bucket.ImgVerifyCode.VerifyCode.CreateByteByImgVerifyCode(code, 80, 38);
            return File(p, "image/Jpeg");
        }
    }
}