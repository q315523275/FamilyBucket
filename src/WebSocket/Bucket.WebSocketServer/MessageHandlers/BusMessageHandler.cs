using Bucket.WebSocketManager;

namespace Bucket.WebSocketServer.MessageHandlers
{
    public class BusMessageHandler : WebSocketHandler
    {
        public BusMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }
    }
}
