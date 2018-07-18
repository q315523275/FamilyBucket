using System.Threading.Tasks;

namespace Bucket.Config
{
    public interface IConfig
    {
        string StringGet(string key);
        string StringGet(string key, string defaultValue);
    }
}
