using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.MVC.Models.Dto
{
    public class OutputLogin: BaseOutput
    {
        public object Data { set; get; }
    }
    public class LoginDto
    {
        public string Access_Token { set; get; }
    }
}
