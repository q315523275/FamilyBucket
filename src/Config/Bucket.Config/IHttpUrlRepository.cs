using System.Threading.Tasks;

namespace Bucket.Config
{
    public interface IHttpUrlRepository
    {
        Task<string> GetApiUrl(long version);
    }
}
