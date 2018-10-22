using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public interface ILoadBalancerFactory
    {
        Task<ILoadBalancer> Get(string serviceName, string _LoadBalancer);
    }
}
