using Bucket.Config.Utils;
using Bucket.LoadBalancer;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Bucket.Config.Abstractions;

namespace Bucket.Config.Implementation
{
    public class HttpUrlRepository : IHttpUrlRepository
    {
        private readonly BucketConfigOptions _setting;
        private readonly IServiceProvider _serviceProvider;

        public HttpUrlRepository(IOptions<BucketConfigOptions> setting, IServiceProvider serviceProvider)
        {
            _setting = setting.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetApiUrl(long version)
        {
            string appId = _setting.AppId;
            string secret = _setting.AppSercet;

            var path = $"/configs/{_setting.AppId}/{_setting.NamespaceName}";

            var query = $"version={version}";

            var sign = $"appId={appId}&appSecret={secret}&namespaceName={_setting.NamespaceName}";

            var pathAndQuery = $"{path}?{query}&env={_setting.Env}&sign=" + SecureHelper.SHA256(sign);

            if (_setting.UseServiceDiscovery)
            {
                var _loadBalancerHouse = _serviceProvider.GetRequiredService<ILoadBalancerHouse>();
                var _balancer = await _loadBalancerHouse.Get(_setting.ServiceName, "RoundRobin");
                var HostAndPort = await _balancer.Lease();
                _setting.ServerUrl = $"{HostAndPort.ToUri()}";
            }

            return $"{_setting.ServerUrl.TrimEnd('/')}{pathAndQuery}";
        }
    }
}
