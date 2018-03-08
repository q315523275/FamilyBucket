using System.Threading.Tasks;

namespace Bucket.ConfigCenter
{
    public interface IConfigCenter
    {
        string Get(string key);
        Task<string> GetAsync(string key);
    }
}
