using Bucket.SkrTrace.WebSocketServer.Handler;
using Microsoft.AspNetCore.Mvc;

namespace Bucket.SkrTrace.WebSocketServer.Controllers
{
    public class SocketGroupController : Controller
    {
        private readonly SkrTraceMessageHandler _skrTraceMessageHandler;

        public SocketGroupController(SkrTraceMessageHandler skrTraceMessageHandler)
        {
            _skrTraceMessageHandler = skrTraceMessageHandler;
        }

        [HttpGet("/SocketGroup/AddToGroup")]
        public dynamic AddToGroup(string groupId, string socketId)
        {
            _skrTraceMessageHandler.AddToGroup(socketId, groupId);
            return new { ErrorCode = "000000", Message = "操作成功" };
        }
        [HttpGet("/SocketGroup/RemoveFromGroup")]
        public dynamic RemoveFromGroup(string groupId, string socketId)
        {
            _skrTraceMessageHandler.RemoveFromGroup(socketId, groupId);
            return new { ErrorCode = "000000", Message = "操作成功" };
        }
    }
}