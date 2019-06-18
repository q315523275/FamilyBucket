namespace Bucket.LoadBalancer
{
    public enum LoadBalancerMode
    {
        Random,
        RoundRobin,
        LeastConnection,
        Hash
    }
}