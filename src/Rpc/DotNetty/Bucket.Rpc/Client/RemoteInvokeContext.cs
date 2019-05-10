using Bucket.Rpc.Messages;

namespace Bucket.Rpc.Client
{
    /// <summary>
    /// 远程调用上下文。
    /// </summary>
    public class RemoteInvokeContext
    {
        /// <summary>
        /// 远程调用消息。
        /// </summary>
        public RemoteInvokeMessage InvokeMessage { get; set; }
    }
}
