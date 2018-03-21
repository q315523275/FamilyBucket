using Bucket.AspNetCore.Authentication;
using Bucket.AspNetCore.EventBus;
using Bucket.AspNetCore.ServiceDiscovery;
using Bucket.ConfigCenter;
using Bucket.EventBus.AspNetCore;
using Bucket.EventBus.Common.Events;
using Bucket.LoadBalancer;
using Bucket.Redis;
using Bucket.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Bucket.ErrorCode;
using Bucket.ErrorCodeStore;
using Bucket.ServiceClient;
using Bucket.ServiceClient.Http;
using Bucket.Buried;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Bucket.ServiceDiscovery.Consul;
using Microsoft.Extensions.Configuration;
using Bucket.ServiceDiscovery;

namespace Bucket.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 框架基础
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucket(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<RedisClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUser, HttpContextUser>();
            services.AddSingleton<IRequestScopedDataRepository, HttpDataRepository>();
            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<IBuriedContext, EmptyBuriedContext>();
            services.AddSingleton<IErrorCodeStore, EmptyErrorCodeStore>();
            return services;
        }
        /// <summary>
        /// Bucket授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationOptions>(configuration.GetSection("Audience"));
            AuthenticationOptions authenticationOption = new AuthenticationOptions();
            configuration.GetSection("Audience").Bind(authenticationOption);
            services.AddBucketAuthentication((x) => {   });
            return services;
        }
        /// <summary>
        /// Bucket授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketAuthentication(this IServiceCollection services, Action<AuthenticationOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var config = new AuthenticationOptions();
            configAction?.Invoke(config);

            var keyByteArray = Encoding.ASCII.GetBytes(config.Secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = config.Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication(options =>
            {
                //认证middleware配置
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                //不使用https
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }
        /// <summary>
        /// 配置中心
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddConfigService(this IServiceCollection services, Action<ConfigCenterConfiguration> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var config = new ConfigCenterConfiguration();
            configAction?.Invoke(config);
            if (config.UseServiceDiscovery)
            {
                // 使用服务发现控制配置中心服务地址
                services.AddSingleton(f => new RemoteConfigRepository(
                    config,
                    f.GetRequiredService<RedisClient>(),
                    f.GetRequiredService<ILoadBalancerHouse>(),
                    f.GetRequiredService<ILoggerFactory>(),
                    f.GetRequiredService<IJsonHelper>())
                    );
            }
            else
            {
                // 指定配置中心服务地址
                services.AddSingleton(f => new RemoteConfigRepository(
                    config, 
                    null, 
                    null, 
                    f.GetRequiredService<ILoggerFactory>(),
                    f.GetRequiredService<IJsonHelper>())
                    );
            }
            services.AddSingleton<IConfigCenter, DefaultConfig>();
            return services;
        }
        /// <summary>
        /// 错误码服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddErroCodeService(this IServiceCollection services, Action<ErrorCodeConfiguration> configAction)
        {

            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var config = new ErrorCodeConfiguration();
            configAction?.Invoke(config);
            // 移除空注入
            services.Remove(new ServiceDescriptor(typeof(IErrorCodeStore), typeof(EmptyErrorCodeStore), ServiceLifetime.Singleton));
            // 错误码中心服务地址
            services.AddSingleton(f => new RemoteStoreRepository(config, f.GetRequiredService<ILoggerFactory>(), f.GetRequiredService<IJsonHelper>()));
            services.AddSingleton<IErrorCodeStore, DefaultErrorCodeStore>();

            return services;
        }
        /// <summary>
        /// 埋点服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBuriedService(this IServiceCollection services)
        {
            services.Remove(new ServiceDescriptor(typeof(IBuriedContext), typeof(EmptyBuriedContext), ServiceLifetime.Singleton));
            services.AddSingleton<IBuriedContext, BuriedContext>();
            return services;
        }
        /// <summary>
        /// 事件驱动
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, Action<EventBusOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.AddSingleton<IEventHandlerExecutionContext>(new EventHandlerExecutionContext(services, sc => sc.BuildServiceProvider()));

            var options = new EventBusOptions();
            configAction?.Invoke(options);

            foreach (var serviceExtension in options.Extensions)
                serviceExtension.AddServices(services);

            return services;
        }
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, Action<ServiceDiscoveryOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.AddSingleton<ILoadBalancerFactory, LoadBalancerFactory>();
            services.AddSingleton<ILoadBalancerHouse, LoadBalancerHouse>();

            var options = new ServiceDiscoveryOptions();
            configAction?.Invoke(options);
            foreach (var serviceExtension in options.Extensions)
                serviceExtension.AddServices(services);

            return services;
        }
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceDiscoveryConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulServiceDiscoveryOption>(configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<ILoadBalancerFactory, LoadBalancerFactory>();
            services.AddSingleton<ILoadBalancerHouse, LoadBalancerHouse>();

            ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);

            services.AddSingleton<IServiceDiscovery>(p => new ConsulServiceDiscovery(serviceDiscoveryOption.Consul));

            return services;
        }
        /// <summary>
        /// 服务发现接口请求
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceClient(this IServiceCollection services)
        {
            services.AddScoped<IServiceClient, BucketHttpClient>();
            return services;
        }
        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            return services;
        }
    }
}
