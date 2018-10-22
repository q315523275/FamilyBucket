using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.MVC.Models.Dto
{
    public class InputLogin
    {
        /// <summary>
        /// 账户
        /// </summary>
        [NotEmpty("credit_0001","不能为空")]
        public string UserName { set; get; }
        public string Password { set; get; }
    }
}
