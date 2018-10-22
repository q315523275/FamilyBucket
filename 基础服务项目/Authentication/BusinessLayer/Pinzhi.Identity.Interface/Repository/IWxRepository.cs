using Pinzhi.Identity.Dto.Wx;
using System.Threading.Tasks;

namespace Pinzhi.Identity.Interface
{
    public interface IWxRepository
    {
        Task<QueryOpenIdOutput> QueryOpenIdAsync(string code, string appId);
    }
}
