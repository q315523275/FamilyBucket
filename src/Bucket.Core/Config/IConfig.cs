using System.Threading.Tasks;

namespace Bucket.Config
{
    public interface IConfig
    {
        string Get(string key);
        string Get(string key, string defaultValue);
        Task<string> GetAsync(string key);
        Task<string> GetAsync(string key, string defaultValue);
    }
}
