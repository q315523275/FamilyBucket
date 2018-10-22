using Pinzhi.Identity.Dto.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pinzhi.Identity.Dto.Auth
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
        /// <summary>
        /// 随机
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 图形验证码
        /// </summary>
        public string ImgCode { get; set; }
    }
}
