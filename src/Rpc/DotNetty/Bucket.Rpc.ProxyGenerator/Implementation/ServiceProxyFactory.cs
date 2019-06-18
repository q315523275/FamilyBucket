using Bucket.Rpc.Client;
using Bucket.Rpc.Convertibles;
using Bucket.Rpc.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Bucket.Rpc.ProxyGenerator.Implementation
{
    /// <summary>
    /// 默认的服务代理工厂实现
    /// </summary>
    public class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly IRemoteInvokeService _remoteInvokeService;
        private readonly ITypeConvertibleService _typeConvertibleService;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<EndPoint, object>> types = new ConcurrentDictionary<Type, ConcurrentDictionary<EndPoint, object>>();
        private IEnumerable<Type> _serviceTypes;
        public ServiceProxyFactory(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService, IEnumerable<Type> serviceTypes, IEnumerable<Type> clientTypes)
        {
            _remoteInvokeService = remoteInvokeService;
            _typeConvertibleService = typeConvertibleService;
            _serviceTypes = serviceTypes;
            foreach (var type in clientTypes)
            {
                types.TryAdd(type, new ConcurrentDictionary<EndPoint, object>());
            }
        }

        /// <summary>
        /// 创建服务代理
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <returns>服务代理实例</returns>
        public object CreateProxy(Type proxyType, EndPoint endPoint)
        {
            return proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[] { _remoteInvokeService, _typeConvertibleService, endPoint });
        }

        public T CreateProxy<T>(EndPoint endPoint) where T : class
        {
            var instanceType = typeof(T);
            if (types.TryGetValue(instanceType, out var instanceList))
            {
                if (instanceList.TryGetValue(endPoint, out object instance))
                {
                    return instance as T;
                }
                else
                {
                    var proxyType = _serviceTypes.Single(instanceType.GetTypeInfo().IsAssignableFrom);
                    instance = proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[] { _remoteInvokeService, _typeConvertibleService, endPoint });
                    instanceList.TryAdd(endPoint, instance);
                    return instance as T;
                }
            }
            throw new RpcException($"无法生成代理方法。");
        }
    }
}