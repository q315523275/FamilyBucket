using System.Collections.Concurrent;

namespace Bucket.Config
{
    public interface ILocalDataRepository
    {
        bool Set(ConcurrentDictionary<string, string> dic);
        ConcurrentDictionary<string, string> Get();
    }
}
