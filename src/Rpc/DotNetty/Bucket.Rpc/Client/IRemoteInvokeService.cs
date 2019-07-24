using Bucket.Rpc.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Rpc.Client
{
    /// <summary>
    /// 一个抽象的远程调用服务。
    /// </summary>
    public interface IRemoteInvokeService
    {
        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">调用上下文。</param>
        /// <param name="endPoint">调用端点。</param>
        /// <returns>远程调用结果消息模型。</returns>
        Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context, EndPoint endPoint, int timeout);

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">调用上下文。</param>
        /// <param name="endPoint">调用端点。</param>
        /// <param name="cancellationToken">取消操作通知实例。</param>
        /// <returns>远程调用结果消息模型。</returns>
        Task<RemoteInvokeResultMessage> InvokeAsync(RemoteInvokeContext context, EndPoint endPoint, int timeout, CancellationToken cancellationToken);
    }
}
