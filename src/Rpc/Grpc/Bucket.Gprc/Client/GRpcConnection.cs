using MagicOnion;
using MagicOnion.Client;
using System;
using System.Threading.Tasks;

namespace Bucket.Gprc.Client
{
    public class GRpcConnection : IGRpcConnection
    {
        private IGrpcChannelFactory _grpcChannelFactory;
        public GRpcConnection(IGrpcChannelFactory grpcChannelFactory)
        {
            this._grpcChannelFactory = grpcChannelFactory;
        }

        public TService GetRemoteService<TService>(string address, int port) where TService : IService<TService>
        {
            var serviceChannel = _grpcChannelFactory.Get(address, port);
            return MagicOnionClient.Create<TService>(serviceChannel);
        }

        public Task<TService> GetRemoteService<TService>(string serviceName) where TService : IService<TService>
        {
            throw new NotImplementedException();
        }
    }
}
