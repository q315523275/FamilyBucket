using Pinzhi.Identity.Dto.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Dto.Third
{
    public class WxMiniUnBindingInput
    {
        [NotEmpty("WxMini_003", "授权Code不能为空")]
        public string Code { set; get; }
        [NotEmpty("identity_001", "AppId不能为空")]
        public string AppId { set; get; } = "wxf722c01492f27e62";
    }
}
