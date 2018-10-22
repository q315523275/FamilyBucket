using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinzhi.Identity.Dto.Third;
using Pinzhi.Identity.Interface;

namespace Pinzhi.Identity.WebApi.Controllers
{
    /// <summary>
    /// 第三方登陆与授权
    /// </summary>
    public class ThirdController : Controller
    {
        private readonly IThirdBusiness _thirdBusiness;
        public ThirdController(IThirdBusiness thirdBusiness)
        {
            _thirdBusiness = thirdBusiness;
        }
        /// <summary>
        /// 微信小程序Code验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("/Third/WxMiniCodeVerify")]
        public async Task<WxMiniLoginOutput> WxMiniCodeVerify([FromBody] WxMiniLoginInput input)
        {
            return await _thirdBusiness.WxMiniCodeVerify(input);
        }
    }
}