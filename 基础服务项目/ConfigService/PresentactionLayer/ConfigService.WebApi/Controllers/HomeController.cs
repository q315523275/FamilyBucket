using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.WebApi.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return Redirect("/swagger/");
        }
    }
}