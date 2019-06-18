using System;

namespace Bucket.Rpc.Exceptions
{
    /// <summary>
    /// RPC远程执行异常（由服务端转发至客户端的异常信息）
    /// </summary>
    public class RpcRemoteException : RpcException
    {
        /// <summary>
        /// 初始化一个新的Rpc异常实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public RpcRemoteException(int code, string message, Exception innerException = null) : base(message, innerException)
        {
            StatusCode = code;
        }
        public int StatusCode { set; get; }
    }
}
