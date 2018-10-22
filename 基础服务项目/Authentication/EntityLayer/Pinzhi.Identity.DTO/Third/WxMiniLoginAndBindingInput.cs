using Pinzhi.Identity.Dto.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Dto.Third
{
    public class WxMiniLoginAndBindingInput
    {
        [NotEmpty("WxMini_003", "授权Code不能为空")]
        public string Code { set; get; }
        [NotEmpty("GO_0005002", "手机号不能为空")]
        [Mobile("GO_0005003", "请输入有效手机号")]
        public string Mobile { set; get; }
        [NotEmpty("GO_0005004", "短信验证码不能为空")]
        public string SmsCode { set; get; }
        [NotEmpty("identity_001", "AppId不能为空")]
        public string AppId { set; get; } = "wxf722c01492f27e62";
    }
}
