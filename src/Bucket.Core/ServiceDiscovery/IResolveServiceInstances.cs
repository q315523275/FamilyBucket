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
        Task<IList<ServiceInformation>> FindServiceInstancesWithStatusAsync(string name, bool passingOnly = true);
        Task<IList<ServiceInformation>> FindServiceInstancesWithLambdaAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate, Predicate<ServiceInformation> ServiceInformationPredicate);
    }
}
