using Bucket.LoadBalancer;

namespace Bucket.Config
{
    public class ConfigServiceLocator
    {
        private readonly ConfigOptions _setting;
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        public ConfigServiceLocator(ConfigOptions setting, ILoadBalancerHouse loadBalancerHouse)
        {
            _setting = setting;
            _loadBalancerHouse = loadBalancerHouse;
        }
        /// <summary>
        /// get config http url
        /// </summary>
        /// <returns></returns>
        public string GetConfigService()
        {
            string configHttpUrl = string.Empty;
            if (_setting.UseServiceDiscovery && _loadBalancerHouse != null)
            {
                var _load = _loadBalancerHouse.Get(_setting.ServiceName, "RoundRobin").GetAwaiter().GetResult();
                var HostAndPort = _load.Lease().GetAwaiter().GetResult();
                configHttpUrl = $"{HostAndPort.ToUri()}";
            }
            else
            {
                configHttpUrl = _setting.ServerUrl;
            }
            return configHttpUrl;
        }
    }
}
