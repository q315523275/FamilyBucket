using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.Rpc.ProxyGenerator.Implementation
{
    public class ServiceProxyProvider : IServiceProxyProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProxyProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<T> InvokeAsync<T>(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint)
        {
            var proxy = _serviceProvider.GetRequiredService<RemoteServiceProxy>();
            if (proxy == null)
            {
                proxy = new RemoteServiceProxy(_serviceProvider);

            }
            return await proxy.InvokeAsync<T>(parameters, serviceId, endPoint);
        }

        public Task InvokeAsync(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint)
        {
            var proxy = _serviceProvider.GetRequiredService<RemoteServiceProxy>();
            if (proxy == null)
            {
                proxy = new RemoteServiceProxy(_serviceProvider);
            }
            return proxy.InvokeAsync(parameters, serviceId, endPoint);
        }
    }
}
