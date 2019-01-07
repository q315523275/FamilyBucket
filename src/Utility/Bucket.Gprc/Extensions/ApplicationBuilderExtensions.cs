using Bucket.ServiceDiscovery;
using Grpc.Core;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using GRpcServer = Grpc.Core.Server;
namespace Bucket.Gprc.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGrpcService(this IApplicationBuilder app, IConfiguration configuration)
        {
            var option = new RpcEndpointOptions();
            configuration.GetSection("ServiceDiscovery").GetSection("RpcEndpoint").Bind(option);
            app.UseGrpcService(option);
            return app;
        }
        public static IApplicationBuilder UseGrpcService(this IApplicationBuilder app, RpcEndpointOptions rpcEndpointOptions)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ??
                throw new ArgumentException("Missing Dependency", nameof(IApplicationLifetime));

            // 开启rpc服务
            var grpcServer = InitializeGrpcServer(rpcEndpointOptions);

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    grpcServer.ShutdownAsync().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"grpcServer had shutown {ex}");
                }
            });

            return app;
        }
        private static GRpcServer InitializeGrpcServer(RpcEndpointOptions options)
        {
            var grpcServer = new GRpcServer
            {
                Ports = { new ServerPort(options.Address, options.Port, ServerCredentials.Insecure) },
                Services = { MagicOnionEngine.BuildServerServiceDefinition() }
            };
            grpcServer.Start();
            return grpcServer;
        }
    }
}
