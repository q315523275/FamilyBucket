using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.MVC.Models.Dto
{
    public class BaseOutput
    {
        public BaseOutput()
        {
            this.ErrorCode = "000000";
            this.Message = "操作成功";
        }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
