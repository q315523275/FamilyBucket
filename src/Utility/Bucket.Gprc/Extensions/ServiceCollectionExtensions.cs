using Bucket.Gprc.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Gprc.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpcClient(this IServiceCollection services)
        {
            services.AddSingleton<IGRpcConnection, GRpcConnection>();
            services.AddSingleton<IGrpcChannelFactory, GrpcChannelFactory>();
            return services;
        }
    }
}
