using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bucket.WebSocketServer.MessageHandlers;
using Bucket.WebSocketManager;
using Bucket.WebSocketServer.DTO;

namespace Bucket.WebSocketServer.Controllers
{
    [Produces("application/json")]
    public class PushController : Controller
    {
        private BusMessageHandler _busMessageHandler { get; set; }

        public PushController(BusMessageHandler busMessageHandler)
        {
            _busMessageHandler = busMessageHandler;
        }

        [HttpPost("/Push/SendPrivateMessage")]
        public async Task<SendMessageOutput> SendPrivateMessage([FromBody]SendPrivateMessageInput input)
        {
            if (string.IsNullOrWhiteSpace(input.PushId) || string.IsNullOrWhiteSpace(input.Message))
                return new SendMessageOutput { ErrorCode = "101", Message = "参数异常" };

            await _busMessageHandler.SendMessageAsync(input.PushId, new Message { MessageType = MessageType.Text, Data = input.Message });
            return new SendMessageOutput();
        }
    }
}