using Bucket.Rpc.Server;
using Bucket.Rpc.Server.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Rpc.Transport.DotNetty
{
    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 使用DotNetty进行传输。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseDotNettyTransport(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ITransportClientFactory, DotNettyTransportClientFactory>();

            services.AddSingleton<DotNettyServerMessageListener>();

            services.AddSingleton<IServiceHost, DefaultServiceHost>(provider => new DefaultServiceHost(async endPoint =>
            {
                var messageListener = provider.GetRequiredService<DotNettyServerMessageListener>();
                await messageListener.StartAsync(endPoint);
                return messageListener;
            }, provider.GetRequiredService<IServiceExecutor>()));

            return builder;
        }
    }
}
