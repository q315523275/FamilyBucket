using Pinzhi.Identity.Dto.Auth;
using System.Threading.Tasks;

namespace Pinzhi.Identity.Interface
{
    public interface IAuthBusiness
    {
        Task<LoginOutput> LoginAsync(LoginInput input);
        Task<LoginOutput> LoginBySmsAsync(LoginBySmsInput input);
        Task<SendSmsCodeOutput> SendSmsCodeAsync(SendSmsCodeInput input);
    }
}
