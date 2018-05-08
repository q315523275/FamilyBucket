using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.WebSocketServer.DTO
{
    public class SendPrivateMessageInput
    {
        public string PushId { set; get; }

        public string Message { set; get; }
    }
}
