namespace Bucket.Rpc.Messages
{
    public class MessagePackTransportMessageType
    {
        public static string remoteInvokeResultMessageTypeName = typeof(RemoteInvokeResultMessage).FullName;

        public static string remoteInvokeMessageTypeName = typeof(RemoteInvokeMessage).FullName;
    }
}
