using Pinzhi.Identity.Dto.Validators;

namespace Pinzhi.Identity.Dto.Auth
{
    public class LoginByMobileInput
    {
        [NotEmpty("GO_0005002", "手机号不能为空")]
        [Mobile("GO_0005003", "请输入有效手机号")]
        public string Mobile { set; get; }
    }
}
