using MagicOnion;
using System.Threading.Tasks;

namespace Bucket.Gprc.Client
{
    public interface IGRpcConnection
    {
        TService GetRemoteService<TService>(string address, int port) where TService : IService<TService>;
        Task<TService> GetRemoteService<TService>(string serviceName) where TService : IService<TService>;
    }
}
