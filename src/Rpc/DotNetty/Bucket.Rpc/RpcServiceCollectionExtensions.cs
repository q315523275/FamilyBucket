using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bucket.Rpc.Client;
using Bucket.Rpc.Client.Implementation;
using Bucket.Rpc.Convertibles;
using Bucket.Rpc.Convertibles.Implementation;
using Bucket.Rpc.Ids;
using Bucket.Rpc.Ids.Implementation;
using Bucket.Rpc.Serialization;
using Bucket.Rpc.Serialization.Implementation;
using Bucket.Rpc.Server;
using Bucket.Rpc.Server.Implementation;
using Bucket.Rpc.Server.ServiceDiscovery;
using Bucket.Rpc.Server.ServiceDiscovery.Attributes;
using Bucket.Rpc.Server.ServiceDiscovery.Implementation;
using Bucket.Rpc.Transport.Codec;
using Bucket.Rpc.Transport.Codec.Implementation;
using System;
using System.Linq;
using Bucket.DependencyInjection;

namespace Bucket.Rpc
{
    /// <summary>
    /// 一个抽象的Rpc服务构建者。
    /// </summary>
    public interface IRpcBuilder
    {
        /// <summary>
        /// 服务集合。
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// 默认的Rpc服务构建者。
    /// </summary>
    internal sealed class RpcBuilder : IRpcBuilder
    {
        public RpcBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            Services = services;
        }

        #region Implementation of IRpcBuilder

        /// <summary>
        /// 服务集合。
        /// </summary>
        public IServiceCollection Services { get; }

        #endregion Implementation of IRpcBuilder
    }

    public static class RpcServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Json序列化支持。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddJsonSerialization(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ISerializer<string>, JsonSerializer>();
            services.AddSingleton<ISerializer<byte[]>, StringByteArraySerializer>();
            services.AddSingleton<ISerializer<object>, StringObjectSerializer>();

            return builder;
        }


        #region Codec Factory

        /// <summary>
        /// 使用编解码器。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <param name="codecFactory"></param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder, ITransportMessageCodecFactory codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);

            return builder;
        }

        /// <summary>
        /// 使用编解码器。
        /// </summary>
        /// <typeparam name="T">编解码器工厂实现类型。</typeparam>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseCodec<T>(this IRpcBuilder builder) where T : class, ITransportMessageCodecFactory
        {
            builder.Services.AddSingleton<ITransportMessageCodecFactory, T>();

            return builder;
        }

        /// <summary>
        /// 使用编解码器。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <param name="codecFactory">编解码器工厂创建委托。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder, Func<IServiceProvider, ITransportMessageCodecFactory> codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);

            return builder;
        }

        #endregion Codec Factory

        /// <summary>
        /// 使用Json编解码器。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseJsonCodec(this IRpcBuilder builder)
        {
            return builder.UseCodec<JsonTransportMessageCodecFactory>();
        }

        /// <summary>
        /// 添加客户端运行时服务。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddClientRuntime(this IRpcBuilder builder)
        {
            var services = builder.Services;

            //services.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
            //services.AddSingleton<IAddressResolver, DefaultAddressResolver>();
            services.AddSingleton<IRemoteInvokeService, RemoteInvokeService>();

            return builder;
        }

        /// <summary>
        /// 添加服务运行时服务。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddServiceRuntime(this IRpcBuilder builder)
        {
            var services = builder.Services;

            //            services.AddSingleton<IServiceInstanceFactory, DefaultServiceInstanceFactory>();
            services.AddSingleton<IClrServiceEntryFactory, ClrServiceEntryFactory>();
            services.AddSingleton<IServiceEntryProvider>(provider =>
            {
#if NET
                var assemblys = AppDomain.CurrentDomain.GetAssemblies();
#else
                //var assemblys = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default).Select(z => Assembly.Load(new AssemblyName(z.Name))));
#endif

                //var types = assemblys.Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray();

                //return new AttributeServiceEntryProvider(types, provider.GetRequiredService<IClrServiceEntryFactory>(),
                //provider.GetRequiredService<ILogger<AttributeServiceEntryProvider>>());

                return new AttributeServiceEntryProvider(AppDomain.CurrentDomain.GetAssemblies().Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray(), 
                    provider.GetRequiredService<IClrServiceEntryFactory>(),
                    provider.GetRequiredService<ILogger<AttributeServiceEntryProvider>>());
            });
            services.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
            services.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
            services.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();

            return builder;
        }

        /// <summary>
        /// 添加RPC核心服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddRpcCore(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();

            services.AddSingleton<ITypeConvertibleProvider, DefaultTypeConvertibleProvider>();
            services.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();

            // services.AddSingleton<IServiceRouteFactory, DefaultServiceRouteFactory>();

            return new RpcBuilder(services)
                .AddJsonSerialization()
                .UseJsonCodec();
        }

        /// <summary>
        /// 添加RPC核心服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder AddRpcCore(this IBucketBuilder builder)
        {
            return AddRpcCore(builder.Services);
        }
    }
}
