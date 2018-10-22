using Microsoft.AspNetCore.Mvc;

namespace Bucket.WebApi
{
    public class StatusController : ApiControllerBase
    {
        [Route("/status")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetStatus()
        {
            return "OK";
        }
    }
}
