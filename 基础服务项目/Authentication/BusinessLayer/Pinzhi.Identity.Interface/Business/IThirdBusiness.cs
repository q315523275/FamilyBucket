using Pinzhi.Identity.Dto.Third;
using System.Threading.Tasks;

namespace Pinzhi.Identity.Interface
{
    public interface IThirdBusiness
    {
        /// <summary>
        /// 微信小程序Code验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<WxMiniLoginOutput> WxMiniCodeVerify(WxMiniLoginInput input);
    }
}
