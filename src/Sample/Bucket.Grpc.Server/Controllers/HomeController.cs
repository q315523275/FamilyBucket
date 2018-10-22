using Microsoft.AspNetCore.Mvc;
using Bucket.WebApi;
namespace Bucket.Grpc.Server.Controllers
{
    public class HomeController: ApiControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}