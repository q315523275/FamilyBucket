using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracing.Storage;
using Tracing.Storage.Query;
using Microsoft.AspNetCore.Mvc;

namespace Tracing.Server.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        // GET
        [HttpGet]
        public async Task<IEnumerable<string>> Get([FromServices]IServiceQuery query)
        {
            var services = await query.GetServices(new TimeRangeQuery());
            return services.Select(x => x.Name);
        }
    }
}