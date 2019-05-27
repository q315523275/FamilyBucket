using System.Threading.Tasks;

namespace Bucket.Config
{
    public interface IConfig
    {
        string StringGet(string key);
        Task<string> StringGetAsync(string key);
        string StringGet(string key, string defaultValue);
        Task<TResult> StringGetAsync<TResult>(string key);
        TResult StringGet<TResult>(string key, TResult defaultValue);
    }
}
