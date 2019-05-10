using System;
using System.Net;

namespace Bucket.Rpc.ProxyGenerator
{
    /// <summary>
    /// 一个抽象的服务代理工厂。
    /// </summary>
    public interface IServiceProxyFactory
    {
        /// <summary>
        /// 创建服务代理。
        /// </summary>
        /// <param name="proxyType">代理类型。</param>
        /// <returns>服务代理实例。</returns>
        object CreateProxy(Type proxyType, EndPoint endPoint);

        T CreateProxy<T>(EndPoint endPoint) where T : class;
    }

    ///// <summary>
    ///// 服务代理工厂扩展。
    ///// </summary>
    //public static class ServiceProxyFactoryExtensions
    //{
    //    /// <summary>
    //    /// 创建服务代理。
    //    /// </summary>
    //    /// <typeparam name="T">服务接口类型。</typeparam>
    //    /// <param name="serviceProxyFactory">服务代理工厂。</param>
    //    /// <param name="proxyType">代理类型。</param>
    //    /// <returns>服务代理实例。</returns>
    //    public static T CreateProxy<T>(this IServiceProxyFactory serviceProxyFactory, Type proxyType, EndPoint endPoint)
    //    {
    //        return (T)serviceProxyFactory.CreateProxy(proxyType, endPoint);
    //    }
    //}
}
