using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Bucket.Config
{
    public interface IDataRepository
    {
        ConcurrentDictionary<string, string> Data { get; }
        Task Get(bool reload = false);
    }
}
