using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Tracing.Server.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index([FromServices] IHostingEnvironment hostingEnvironment)
        {
            //using (var stream = System.IO.File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html")))
            //{
            //    using (var reader = new StreamReader(stream))
            //    {
            //        return Content(reader.ReadToEnd(), "text/html");
            //    }
            //}
            return Content("start");
        }
    }
}