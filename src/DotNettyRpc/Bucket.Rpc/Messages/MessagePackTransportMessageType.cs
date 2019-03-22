using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Rpc.Messages
{
    public class MessagePackTransportMessageType
    {
        public static string remoteInvokeResultMessageTypeName = typeof(RemoteInvokeResultMessage).FullName;

        public static string remoteInvokeMessageTypeName = typeof(RemoteInvokeMessage).FullName;
    }
}
