using Bucket.Core;
using Bucket.Values;
using Consul;
using Microsoft.Extensions.Options;
using SemVer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.ServiceDiscovery.Consul
{
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private const string VERSION_PREFIX = "version-";

        private readonly ConsulServiceDiscoveryConfiguration _configuration;

        private readonly ConsulClient _consul;

        public ConsulServiceDiscovery(IOptions<ConsulServiceDiscoveryConfiguration> configuration)
        {
            _configuration = configuration.Value;

            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri(_configuration.HttpEndpoint);
                if (!string.IsNullOrEmpty(_configuration.Datacenter))
                {
                    config.Datacenter = _configuration.Datacenter;
                }
            });
        }

        #region IResolveServiceInstances
        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }
        private async Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            var queryResult = await _consul.Catalog.Services(); // local agent datacenter is implied
            var services = queryResult.Response;

            return services;
        }
        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync()
        {
            return await FindServiceInstancesWithLambdaAsync(nameTagsPredicate: x => true, ServiceInformationPredicate: x => true);
        }
        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync(string name)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: true);
            var instances = queryResult.Response.Select(serviceEntry => new ServiceInformation
            {
                Name = serviceEntry.Service.Service,
                HostAndPort = new HostAndPort(serviceEntry.Service.Address, serviceEntry.Service.Port),
                Version = GetVersionFromStrings(serviceEntry.Service.Tags),
                Tags = serviceEntry.Service.Tags ?? Enumerable.Empty<string>(),
                Id = serviceEntry.Service.ID
            });

            return instances.ToList();
        }
        public async Task<IList<ServiceInformation>> FindServiceInstancesWithStatusAsync(string name, bool passingOnly = true)
        {
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: passingOnly);
            var instances = queryResult.Response.Select(serviceEntry => new ServiceInformation
            {
                Name = serviceEntry.Service.Service,
                HostAndPort = new HostAndPort(serviceEntry.Service.Address, serviceEntry.Service.Port),
                Version = GetVersionFromStrings(serviceEntry.Service.Tags),
                Tags = serviceEntry.Service.Tags ?? Enumerable.Empty<string>(),
                Id = serviceEntry.Service.ID
            });

            return instances.ToList();
        }
        public async Task<IList<ServiceInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name);
            var range = new Range(version);

            return instances.Where(x => range.IsSatisfied(x.Version)).ToArray();
        }
        public async Task<IList<ServiceInformation>> FindServiceInstancesWithLambdaAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<ServiceInformation> ServiceInformationPredicate)
        {
            return (await GetServicesCatalogAsync())
                .Where(kvp => nameTagsPredicate(kvp))
                .Select(kvp => kvp.Key)
                .Select(FindServiceInstancesAsync)
                .SelectMany(task => task.Result)
                .Where(x => ServiceInformationPredicate(x))
                .ToList();
        }
        #endregion

        #region IManageServiceInstances
        private string GetServiceId(string serviceName, Uri uri)
        {
            return $"{serviceName}_{uri.Host.Replace(".", "_")}_{uri.Port}";
        }

        public async Task<ServiceInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, ServiceType serviceType = ServiceType.HTTP, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var serviceId = GetServiceId(serviceName, uri);

            // 健康检查地址
            string checkUrl = healthCheckUri?.ToString() ?? $"{uri}".TrimEnd('/') + "/status";
            if (serviceType == ServiceType.TCP)
                checkUrl = (healthCheckUri == null ? $"{uri.Host}:{uri.Port}" : $"{healthCheckUri.Host}:{healthCheckUri.Port}");

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),// 服务启动多久后注册
                Interval = TimeSpan.FromSeconds(10),// 健康检查时间间隔，或者称为心跳间隔
                HTTP = (serviceType == ServiceType.HTTP ? checkUrl : null),// 健康检查地址
                TCP = (serviceType == ServiceType.TCP ? checkUrl : null),// 健康检查地址
                Timeout = TimeSpan.FromSeconds(10)
            };

            string versionLabel = $"{VERSION_PREFIX}{version}";

            var tagList = (tags ?? Enumerable.Empty<string>()).ToList();
            tagList.Add(versionLabel);

            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = serviceName,
                Tags = tagList.ToArray(),
                Address = uri.Host,
                Port = uri.Port,
                Check = httpCheck
            };

            await _consul.Agent.ServiceRegister(registration);

            return new ServiceInformation
            {
                Name = registration.Name,
                Id = registration.ID,
                HostAndPort = new HostAndPort(registration.Address, registration.Port),
                Version = version,
                Tags = tagList
            };
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            var writeResult = await _consul.Agent.ServiceDeregister(serviceId);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            return isSuccess;
        }
        #endregion

        #region IManageHealthChecks

        private string GetCheckId(string serviceId, Uri uri)
        {
            return $"{serviceId}_{uri.GetPath().Replace("/", "")}";
        }

        public async Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, ServiceType serviceType = ServiceType.HTTP, TimeSpan? interval = null, string notes = null)
        {
            if (checkUri == null)
            {
                throw new ArgumentNullException(nameof(checkUri));
            }

            var checkId = GetCheckId(serviceId, checkUri);
            var checkRegistration = new AgentCheckRegistration
            {
                ID = checkId,
                Name = serviceName,
                Notes = notes,
                ServiceID = serviceId,
                HTTP = (serviceType == ServiceType.HTTP ? checkUri.ToString() : null),
                TCP = (serviceType == ServiceType.TCP ? $"{checkUri.Host}:{checkUri.Port}" : null),
                Interval = interval
            };
            var writeResult = await _consul.Agent.CheckRegister(checkRegistration);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            return checkId;
        }

        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            var writeResult = await _consul.Agent.CheckDeregister(checkId);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            return isSuccess;
        }

        private QueryOptions QueryOptions(ulong index)
        {
            return new QueryOptions
            {
                Datacenter = _configuration.Datacenter,
                Token = _configuration.AclToken ?? "anonymous",
                WaitIndex = index,
                WaitTime = _configuration.LongPollMaxWait
            };
        }

        #endregion

        #region IHaveKeyValues

        public async Task KeyValuePutAsync(string key, string value)
        {
            var keyValuePair = new KVPair(key) { Value = Encoding.UTF8.GetBytes(value) };
            await _consul.KV.Put(keyValuePair);
        }
        public async Task<string> KeyValueGetAsync(string key)
        {
            var queryResult = await _consul.KV.Get(key);
            if (queryResult.Response == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(queryResult.Response.Value);
        }
        public async Task KeyValueDeleteAsync(string key)
        {
            await _consul.KV.Delete(key);
        }
        public async Task KeyValueDeleteTreeAsync(string prefix)
        {
            await _consul.KV.DeleteTree(prefix);
        }
        public async Task<string[]> KeyValuesGetKeysAsync(string prefix)
        {
            var queryResult = await _consul.KV.Keys(prefix ?? "");
            return queryResult.Response;
        }
        #endregion
    }
}
