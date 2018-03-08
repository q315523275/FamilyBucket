using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bucket.Buried
{
    public interface IBuriedContext
    {
        Task PublishAsync<T>(T buriedInformation);
        Task PublishAsync<T>(T buriedInformation,HttpContext httpContext);
        Dictionary<string, string> DownStreamHeaders();
    }
}
