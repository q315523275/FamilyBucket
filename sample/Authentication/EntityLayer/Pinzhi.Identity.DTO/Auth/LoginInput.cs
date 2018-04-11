using Pinzhi.Identity.DTO.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pinzhi.Identity.DTO.Auth
{
    public class LoginInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [NotEmpty("001","账号不能为空")]
        public string UserName { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
    }
}
