using Bucket.Values;
using System.Threading.Tasks;

namespace Bucket.Listener.Abstractions
{
    public interface IExtractCommand
    {
        Task CommandNotify(NetworkCommand command);
    }
}
