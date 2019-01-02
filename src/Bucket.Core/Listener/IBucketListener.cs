using System.Threading.Tasks;

namespace Bucket.Listener
{
    public interface IBucketListener
    {
        string ListenerName { get; }
        Task ExecuteAsync(string commandText);
    }
}
