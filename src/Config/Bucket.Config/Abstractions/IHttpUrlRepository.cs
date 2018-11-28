using System.Threading.Tasks;

namespace Bucket.Config.Abstractions
{
    public interface IHttpUrlRepository
    {
        Task<string> GetApiUrl(long version);
    }
}
