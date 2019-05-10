using System;

namespace Bucket.Rpc.Messages
{
    public class TransportMessage
    {
        public TransportMessage()
        {
        }
        public TransportMessage(object content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentType = content.GetType().FullName;
        }
        public TransportMessage(object content, string fullName)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentType = fullName;
        }
        /// <summary>
        /// 消息Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 内容类型。
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 是否调用消息
        /// </summary>
        /// <returns>如果是则返回true，否则返回false</returns>
        public bool IsInvokeMessage() => ContentType == typeof(RemoteInvokeMessage).FullName;

        /// <summary>
        /// 是否是调用结果消息
        /// </summary>
        /// <returns>如果是则返回true，否则返回false</returns>
        public bool IsInvokeResultMessage() => ContentType == typeof(RemoteInvokeResultMessage).FullName;
        /// <summary>
        /// 获取内容。
        /// </summary>
        /// <typeparam name="T">内容类型。</typeparam>
        /// <returns>内容实例。</returns> 
        public T GetContent<T>()
        {
            return (T)Content;
        }
        /// <summary>
        /// 创建一个调用传输消息。
        /// </summary>
        /// <param name="invokeMessage">调用实例。</param>
        /// <returns>调用传输消息。</returns>  
        public static TransportMessage CreateInvokeMessage(RemoteInvokeMessage invokeMessage)
        {
            return new TransportMessage(invokeMessage, MessagePackTransportMessageType.remoteInvokeMessageTypeName)
            {
                Id = Guid.NewGuid().ToString("N")
            };
        }

        /// <summary>
        /// 创建一个调用结果传输消息。
        /// </summary>
        /// <param name="id">消息Id。</param>
        /// <param name="invokeResultMessage">调用结果实例。</param>
        /// <returns>调用结果传输消息。</returns>  
        public static TransportMessage CreateInvokeResultMessage(string id, RemoteInvokeResultMessage invokeResultMessage)
        {
            return new TransportMessage(invokeResultMessage, MessagePackTransportMessageType.remoteInvokeResultMessageTypeName)
            {
                Id = id
            };
        }
    }
}
