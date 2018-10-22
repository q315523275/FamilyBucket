using Pinzhi.Identity.Dto.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Dto.Auth
{
    public class LoginBySmsInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [NotEmpty("GO_0005002", "手机号不能为空")]
        [Mobile("GO_0005003", "请输入有效手机号")]
        public string Mobile { set; get; }
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string SmsCode { set; get; }
    }
}
