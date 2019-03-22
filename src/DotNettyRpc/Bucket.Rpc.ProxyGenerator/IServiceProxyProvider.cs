using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.Rpc.ProxyGenerator
{
    public interface IServiceProxyProvider
    {
        Task<T> InvokeAsync<T>(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint);
        Task InvokeAsync(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint);
    }
}
