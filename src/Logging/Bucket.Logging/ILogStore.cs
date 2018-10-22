using System.Threading.Tasks;

namespace Bucket.Logging
{
    public interface ILogStore
    {
        void Post(LogInfo logs);
    }
}
