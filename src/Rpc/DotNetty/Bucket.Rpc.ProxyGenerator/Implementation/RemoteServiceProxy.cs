using Bucket.Rpc.Client;
using Bucket.Rpc.Convertibles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.Rpc.ProxyGenerator.Implementation
{
    public class RemoteServiceProxy : ServiceProxyBase
    {
        public RemoteServiceProxy(IServiceProvider serviceProvider) :
            base(serviceProvider.GetRequiredService<IRemoteInvokeService>(), serviceProvider.GetRequiredService<ITypeConvertibleService>(), null)
        {
        }

        public new async Task<T> InvokeAsync<T>(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint)
        {
            return await base.InvokeAsync<T>(parameters, serviceId, endPoint);
        }
        public new Task InvokeAsync(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint)
        {
            return base.InvokeAsync(parameters, serviceId, endPoint);
        }
    }
}
