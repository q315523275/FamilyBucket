using Bucket.Rpc.Exceptions;
using Bucket.Rpc.Messages;
using Bucket.Rpc.Transport;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Rpc.Client.Implementation
{
    public class RemoteInvokeService : IRemoteInvokeService
    {
        private readonly ITransportClientFactory _transportClientFactory;
        private readonly ILogger<RemoteInvokeService> _logger;
        public RemoteInvokeService(ITransportClientFactory transportClientFactory, ILogger<RemoteInvokeService> logger)
        {
            _transportClientFactory = transportClientFactory;
            _logger = logger;
        }

        #region Implementation of IRemoteInvokeService

        public Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context, EndPoint endPoint, int timeout)
        {
            return InvokeAsync(context, endPoint, timeout, Task.Factory.CancellationToken);
        }

        public async Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context, EndPoint endPoint, int timeout, CancellationToken cancellationToken)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.InvokeMessage == null)
                throw new ArgumentNullException(nameof(context.InvokeMessage));

            if (string.IsNullOrEmpty(context.InvokeMessage.ServiceId))
                throw new ArgumentException("服务Id不能为空。", nameof(context.InvokeMessage.ServiceId));

            var invokeMessage = context.InvokeMessage;

            if (endPoint == null)
                throw new RpcException($"无法解析服务Id：{invokeMessage.ServiceId}的地址信息。");

            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug($"使用地址：'{endPoint}'进行调用。");

                var client = await _transportClientFactory.CreateClientAsync(endPoint);
                return await client.SendAsync(context.InvokeMessage, timeout);
            }
            catch (RpcCommunicationException exception)
            {
                // await _healthCheckService.MarkFailure(address);
                throw exception;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"发起请求中发生了错误，服务Id：{invokeMessage.ServiceId}。");
                throw exception;
            }
        }

        #endregion Implementation of IRemoteInvokeService
    }
}
