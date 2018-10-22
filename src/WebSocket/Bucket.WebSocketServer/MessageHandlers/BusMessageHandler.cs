using Bucket.WebSocketManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Bucket.WebSocketServer.MessageHandlers
{
    public class BusMessageHandler : WebSocketHandler
    {
        public BusMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }
    }
}
