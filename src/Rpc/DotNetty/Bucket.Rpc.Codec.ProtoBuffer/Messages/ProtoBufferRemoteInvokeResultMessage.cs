using Bucket.Rpc.Messages;
using ProtoBuf;

namespace Bucket.Rpc.Codec.ProtoBuffer.Messages
{
    [ProtoContract]
    public class ProtoBufferRemoteInvokeResultMessage
    {
        #region Constructor

        public ProtoBufferRemoteInvokeResultMessage(RemoteInvokeResultMessage message)
        {
            ExceptionMessage = message.ExceptionMessage;
            Result = message.Result == null ? null : new ProtoBufferDynamicItem(message.Result);
        }

        public ProtoBufferRemoteInvokeResultMessage()
        {
        }

        #endregion Constructor

        /// <summary>
        /// 异常消息。
        /// </summary>
        [ProtoMember(1)]
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// 结果内容。
        /// </summary>
        [ProtoMember(2)]
        public ProtoBufferDynamicItem Result { get; set; }

        public RemoteInvokeResultMessage GetRemoteInvokeResultMessage()
        {
            return new RemoteInvokeResultMessage
            {
                ExceptionMessage = ExceptionMessage,
                Result = Result?.Get()
            };
        }
    }
}
