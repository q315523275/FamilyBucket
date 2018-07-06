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
            var code = Utility.Helpers.Randoms.CreateRandomValue(4, false);
            var p = Utility.Helpers.VerifyCode.GetSingleObj().CreateByteByImgVerifyCode(code, 100, 38);
            return File(p, "image/Jpeg");
        }
    }
}