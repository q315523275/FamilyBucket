using Pinzhi.Identity.DTO.Auth;
using System.Threading.Tasks;

namespace Pinzhi.Identity.Interface
{
    public interface IAuthBusiness
    {
        Task<LoginOutput> LoginAsync(LoginInput input);
    }
}
