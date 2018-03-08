using System.Threading.Tasks;

namespace Bucket.ServiceDiscovery
{
    public interface IHaveKeyValues
    {
        Task KeyValuePutAsync(string key, string value);
        Task<string> KeyValueGetAsync(string key);
        Task KeyValueDeleteAsync(string key);
        Task KeyValueDeleteTreeAsync(string prefix);
        Task<string[]> KeyValuesGetKeysAsync(string prefix);
    }
}
