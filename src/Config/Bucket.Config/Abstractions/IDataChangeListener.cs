using System.Collections.Concurrent;

namespace Bucket.Config.Abstractions
{
    public interface IDataChangeListener
    {
        void OnDataChange(ConcurrentDictionary<string, string> changeData);
    }
}
