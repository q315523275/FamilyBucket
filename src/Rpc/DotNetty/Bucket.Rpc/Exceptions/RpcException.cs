using System;

namespace Bucket.Rpc.Exceptions
{
    /// <summary>
    /// 基础的Rpc异常类
    /// </summary>
    public class RpcException : Exception
    {
        /// <summary>
        /// 初始化一个新的Rpc异常实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public RpcException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
