using Bucket.Rpc.Messages;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.Rpc.Codec.ProtoBuffer.Messages
{
    [ProtoContract]
    public class ParameterItem
    {
        #region Constructor

        public ParameterItem(KeyValuePair<string, object> item)
        {
            Key = item.Key;
            Value = item.Value == null ? null : new ProtoBufferDynamicItem(item.Value);
        }

        public ParameterItem()
        {
        }

        #endregion Constructor

        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public ProtoBufferDynamicItem Value { get; set; }
    }

    [ProtoContract]
    public class ProtoBufferRemoteInvokeMessage
    {
        public ProtoBufferRemoteInvokeMessage(RemoteInvokeMessage message)
        {
            ServiceId = message.ServiceId;
            Parameters = message.Parameters?.Select(i => new ParameterItem(i)).ToArray();
        }

        public ProtoBufferRemoteInvokeMessage()
        {
        }

        /// <summary>
        /// 服务Id。
        /// </summary>
        [ProtoMember(1)]
        public string ServiceId { get; set; }

        /// <summary>
        /// 服务参数。
        /// </summary>
        [ProtoMember(2)]
        public ParameterItem[] Parameters { get; set; }

        public RemoteInvokeMessage GetRemoteInvokeMessage()
        {
            return new RemoteInvokeMessage
            {
                Parameters = Parameters?.ToDictionary(i => i.Key, i => i.Value?.Get()),
                ServiceId = ServiceId
            };
        }
    }
}
