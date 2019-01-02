using Bucket.WebSocketManager;

namespace Bucket.SkrTrace.WebSocketServer.Handler
{
    public class SkrTraceMessageHandler : WebSocketHandler
    {
        public SkrTraceMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager) { }
        public void AddToGroup(string socketID, string groupID)
        {
            var isInGroup = false;
            var sockets = base.WebSocketConnectionManager.GetAllFromGroup(groupID);
            if (sockets != null)
            {
                foreach (var id in sockets)
                {
                    if (id == socketID)
                        isInGroup = true;
                }
            }
            if(!isInGroup)
                base.WebSocketConnectionManager.AddToGroup(socketID, groupID);
        }
        public void RemoveFromGroup(string socketID, string groupID)
        {
            var isInGroup = false;
            var sockets = base.WebSocketConnectionManager.GetAllFromGroup(groupID);
            if (sockets != null)
            {
                foreach (var id in sockets)
                {
                    if (id == socketID)
                        isInGroup = true;
                }
            }
            if (isInGroup)
                base.WebSocketConnectionManager.RemoveFromGroup(socketID, groupID);
        }
    }
}
