using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.WebSocketServer.DTO
{
    public class SendMessageOutput
    {
        public SendMessageOutput()
        {
            ErrorCode = "000000";
            Message = "推送成功";
        }
        public string ErrorCode { set; get; }
        public string Message { set; get; }
    }
}
