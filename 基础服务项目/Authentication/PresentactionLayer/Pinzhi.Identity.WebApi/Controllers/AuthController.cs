using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Pinzhi.Identity.Interface;
using Pinzhi.Identity.Dto.Auth;
using Bucket.WebApi;
namespace Pinzhi.Identity.WebApi.Controllers
{
    /// <summary>
    /// 认证授权控制器
    /// </summary>
    [Produces("application/json")]
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// 业务实现
        /// </summary>
        private readonly IAuthBusiness _authBusiness;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="authBusiness"></param>
        public AuthController(IAuthBusiness authBusiness)
        {
            _authBusiness = authBusiness;
        }
        /// <summary>
        /// 账户登陆 - 密码登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Login")]
        public async Task<LoginOutput> Login([FromBody] LoginInput input)
        {
            return await _authBusiness.LoginAsync(input);
        }
        /// <summary>
        /// 用户短信登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/LoginBySms")]
        public async Task<LoginOutput> LoginBySmsAsync([FromBody] LoginBySmsInput input)
        {
            return await _authBusiness.LoginBySmsAsync(input);
        }
        /// <summary>
        /// 账户登陆 - 发送登陆验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/SendSmsCode")]
        public async Task<SendSmsCodeOutput> SendSmsCode([FromBody] SendSmsCodeInput input)
        {
            return await _authBusiness.SendSmsCodeAsync(input);
        }
    }
}