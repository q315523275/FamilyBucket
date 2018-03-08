using Bucket.Core;
using Bucket.ServiceDiscovery;
using Bucket.Values;
using Consul;
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

        public ConsulServiceDiscovery(ConsulServiceDiscoveryConfiguration configuration = null)
        {
            _configuration = configuration;

            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri(_configuration.HttpEndpoint);
                if (!string.IsNullOrEmpty(_configuration.Datacenter))
                {
                    config.Datacenter = _configuration.Datacenter;
                }
            });
        }

        private string GetVersionFromStrings(IEnumerable<string> strings)
        {
            return strings
                ?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal))
                .TrimStart(VERSION_PREFIX);
        }

        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync()
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, ServiceInformationPredicate: x => true);
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

        public async Task<IList<ServiceInformation>> FindServiceInstancesWithVersionAsync(string name, string version)
        {
            var instances = await FindServiceInstancesAsync(name);
            var range = new Range(version);

            return instances.Where(x => range.IsSatisfied(x.Version)).ToArray();
        }

        private async Task<IDictionary<string, string[]>> GetServicesCatalogAsync()
        {
            var queryResult = await _consul.Catalog.Services(); // local agent datacenter is implied
            var services = queryResult.Response;

            return services;
        }

        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<ServiceInformation> ServiceInformationPredicate)
        {
            return (await GetServicesCatalogAsync())
                .Where(kvp => nameTagsPredicate(kvp))
                .Select(kvp => kvp.Key)
                .Select(FindServiceInstancesAsync)
                .SelectMany(task => task.Result)
                .Where(x => ServiceInformationPredicate(x))
                .ToList();
        }

        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: predicate, ServiceInformationPredicate: x => true);
        }

        public async Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<ServiceInformation> predicate)
        {
            return await FindServiceInstancesAsync(nameTagsPredicate: x => true, ServiceInformationPredicate: predicate);
        }

        public async Task<IList<ServiceInformation>> FindAllServicesAsync()
        {
            var queryResult = await _consul.Agent.Services();
            var instances = queryResult.Response.Select(serviceEntry => new ServiceInformation
            {
                Name = serviceEntry.Value.Service,
                Id = serviceEntry.Value.ID,
                HostAndPort = new HostAndPort(serviceEntry.Value.Address, serviceEntry.Value.Port),
                Version = GetVersionFromStrings(serviceEntry.Value.Tags),
                Tags = serviceEntry.Value.Tags
            });

            return instances.ToList();
        }

        private string GetServiceId(string serviceName, Uri uri)
        {
            return $"{serviceName}_{uri.Host.Replace(".", "_")}_{uri.Port}";
        }

        public async Task<ServiceInformation> RegisterServiceAsync(string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            var serviceId = GetServiceId(serviceName, uri);
            string check = healthCheckUri?.ToString() ?? $"{uri}".TrimEnd('/') + "/status";

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
                Check = new AgentServiceCheck { HTTP = check, Interval = TimeSpan.FromSeconds(5) }
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
            string success = isSuccess ? "succeeded" : "failed";

            return isSuccess;
        }

        private string GetCheckId(string serviceId, Uri uri)
        {
            return $"{serviceId}_{uri.GetPath().Replace("/", "")}";
        }

        public async Task<string> RegisterHealthCheckAsync(string serviceName, string serviceId, Uri checkUri, TimeSpan? interval = null, string notes = null)
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
                HTTP = checkUri.ToString(),
                Interval = interval
            };
            var writeResult = await _consul.Agent.CheckRegister(checkRegistration);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            string success = isSuccess ? "succeeded" : "failed";

            return checkId;
        }

        public async Task<bool> DeregisterHealthCheckAsync(string checkId)
        {
            var writeResult = await _consul.Agent.CheckDeregister(checkId);
            bool isSuccess = writeResult.StatusCode == HttpStatusCode.OK;
            string success = isSuccess ? "succeeded" : "failed";

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
    }
}
