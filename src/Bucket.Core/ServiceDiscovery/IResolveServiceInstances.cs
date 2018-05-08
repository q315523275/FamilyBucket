using Bucket.Values;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.ServiceDiscovery
{
    public interface IResolveServiceInstances
    {
        Task<IList<ServiceInformation>> FindServiceInstancesAsync();
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(string name);
        Task<IList<ServiceInformation>> FindServiceInstancesWithVersionAsync(string name, string version);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate,
            Predicate<ServiceInformation> registryInformationPredicate);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<ServiceInformation> predicate);
        Task<IList<ServiceInformation>> FindAllServicesAsync();
    }
}
