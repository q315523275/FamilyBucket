using MessagePack;
using Bucket.Rpc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bucket.Rpc.Codec.MessagePack.Messages
{
    [MessagePackObject]
    public class ParameterItem
    {
        #region Constructor

        public ParameterItem(KeyValuePair<string, object> item)
        {
            Key = item.Key;
            Value = item.Value == null ? null : new MessagePackDynamicItem(item.Value);
        }

        public ParameterItem()
        {
        }

        #endregion Constructor

        [Key(0)]
        public string Key { get; set; }

        [Key(1)]
        public MessagePackDynamicItem Value { get; set; }
    }

    [MessagePackObject]
    public class MessagePackRemoteInvokeMessage
    {
        public MessagePackRemoteInvokeMessage(RemoteInvokeMessage message)
        {
            ServiceId = message.ServiceId;
            Parameters = message.Parameters?.Select(i => new ParameterItem(i)).ToArray();
        }

        public MessagePackRemoteInvokeMessage()
        {
        }

        [Key(0)]
        public string ServiceId { get; set; }

        [Key(1)]
        public string Token { get; set; }

        [Key(2)]
        public ParameterItem[] Parameters { get; set; }

        [Key(3)]
        public ParameterItem[] Attachments { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
