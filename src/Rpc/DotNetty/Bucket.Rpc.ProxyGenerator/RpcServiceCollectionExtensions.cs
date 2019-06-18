using Bucket.Rpc.Client;
using Bucket.Rpc.Convertibles;
using Bucket.Rpc.ProxyGenerator.Implementation;
using Bucket.Rpc.Server.ServiceDiscovery.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bucket.Rpc.ProxyGenerator
{
    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 添加代理。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddServiceProxy(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
            services.AddSingleton<IServiceProxyFactory>(sp =>
            {
                var (serviceTypes, clientTypes) = RegisterProxType(sp);
                return new ServiceProxyFactory(sp.GetRequiredService<IRemoteInvokeService>(), sp.GetRequiredService<ITypeConvertibleService>(), serviceTypes, clientTypes);
            });
            services.AddSingleton<IServiceProxyProvider, ServiceProxyProvider>();
            services.AddSingleton<RemoteServiceProxy>();

            return builder;
        }
        /// <summary>
        /// 注册需要代理的type
        /// </summary>
        /// <returns></returns>
        public static (IEnumerable<Type> serviceTypes, IEnumerable<Type> clientTypes) RegisterProxType(IServiceProvider serviceProvider)
        {
            // 需要代理type
            var _types = AppDomain.CurrentDomain.GetAssemblies().Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes);
            var services = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsInterface && typeInfo.GetCustomAttribute<RpcServiceBundleAttribute>() != null;
            }).ToArray();
            var serviceImplementations = _types.Where(i =>
            {
                var typeInfo = i.GetTypeInfo();
                return typeInfo.IsClass && !typeInfo.IsAbstract && i.Namespace != null && !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Microsoft");
            });

            var clients = new List<Type>();
            foreach (var service in services)
            {
                if (!serviceImplementations.Any(i => service.GetTypeInfo().IsAssignableFrom(i)))
                {
                    clients.Add(service);
                }
            }
            // 生成代理
            var proxyGenerater = serviceProvider.GetService<IServiceProxyGenerater>();
            var serviceTypes = proxyGenerater.GenerateProxys(clients);
            return (serviceTypes, clients);
        }
    }
}
