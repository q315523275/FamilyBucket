using System.Threading.Tasks;

namespace Bucket.Logging
{
    public interface ILogStore
    {
        Task Post(LogInfo logs);
    }
}
