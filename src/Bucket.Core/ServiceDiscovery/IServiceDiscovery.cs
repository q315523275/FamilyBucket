namespace Bucket.ServiceDiscovery
{
    public interface IServiceDiscovery : IManageServiceInstances, 
        IManageHealthChecks,
        IResolveServiceInstances,
        IHaveKeyValues
    {
    }
}
