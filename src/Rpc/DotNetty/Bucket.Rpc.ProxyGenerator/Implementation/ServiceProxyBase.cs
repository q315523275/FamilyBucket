using Bucket.Rpc.Client;
using Bucket.Rpc.Convertibles;
using Bucket.Rpc.Exceptions;
using Bucket.Rpc.Messages;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.Rpc.ProxyGenerator.Implementation
{
    /// <summary>
    /// 一个抽象的服务代理基类。
    /// </summary>
    public abstract class ServiceProxyBase
    {
        #region Field

        private readonly IRemoteInvokeService _remoteInvokeService;
        private readonly ITypeConvertibleService _typeConvertibleService;
        private readonly EndPoint _endPoint;
        #endregion Field

        #region Constructor

        protected ServiceProxyBase(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService, EndPoint endPoint)
        {
            _remoteInvokeService = remoteInvokeService;
            _typeConvertibleService = typeConvertibleService;
            _endPoint = endPoint;
        }
        #endregion Constructor

        #region Protected Method

        /// <summary>
        /// 远程调用。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="parameters">参数字典。</param>
        /// <param name="serviceId">服务Id。</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <returns>调用结果。</returns>
        protected async Task<T> InvokeAsync<T>(IDictionary<string, object> parameters, string serviceId, int timeout = 60)
        {
            if (_endPoint == null)
                throw new RpcException($"无法解析服务Id：{serviceId}的地址信息。");

            var message = await _remoteInvokeService.InvokeAsync(new RemoteInvokeContext
            {
                InvokeMessage = new RemoteInvokeMessage
                {
                    Parameters = parameters,
                    ServiceId = serviceId
                }
            }, _endPoint, timeout);

            if (message == null)
                return default;

            var result = _typeConvertibleService.Convert(message.Result, typeof(T));

            return (T)result;
        }
        /// <summary>
        ///  远程调用。
        /// </summary>
        /// <typeparam name="T">返回类型。</typeparam>
        /// <param name="parameters">参数字典。</param>
        /// <param name="serviceId">服务Id。</param>
        /// <param name="endPoint">调用端点</param>
        /// <param name="timeout">超时时间，单位秒</param>
        /// <returns></returns>
        protected async Task<T> InvokeAsync<T>(IDictionary<string, object> parameters, string serviceId, EndPoint endPoint, int timeout = 60)
        {
            var message = await _remoteInvokeService.InvokeAsync(new RemoteInvokeContext
            {
                InvokeMessage = new RemoteInvokeMessage
                {
                    Parameters = parameters,
                    ServiceId = serviceId
                }
            }, endPoint, timeout);

            if (message == null)
                return default;

            var result = _typeConvertibleService.Convert(message.Result, typeof(T));

            return (T)result;
        }
        #endregion Protected Method
    }
}