
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

using Bucket.DbContext;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.Logging;
using Bucket.Logging.Events;


using Bucket.ApiGateway.ConfigStored.MySql;

using global::Ocelot.DependencyInjection;
using global::Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Bucket.SkyApm.Agent.AspNetCore;
using Bucket.SkyApm.Transport.EventBus;
using Bucket.DbContext.SqlSugar;

namespace Bucket.ApiGateway
{
    /// <summary>
    /// 启动配置
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化启动配置
        /// </summary>
        /// <param name="configuration">配置</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 配置服务
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // 授权认证
            AddOcelotJwtBearer(services, Configuration);
            // 添加跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            // 添加首字母大写
            services.AddMvc().AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });
            // 添加事件驱动
            services.AddEventBus(option => { option.UseRabbitMQ(); });
            // 添加日志消息传输
            services.AddLogEventTransport();
            // 添加链路
            services.AddBucketSkyApmCore().UseEventBusTransport();
            // 添加Orm
            services.AddSqlSugarDbContext();
            // 添加网关
            services.AddOcelot()
             .AddCacheManager(x =>
             {
                 x.WithDictionaryHandle();
             })
             .AddPolly()
             .AddConsul()
             //.AddConfigStoredInConsul()
             //.AddConfigStoredInRedis("Bucket.ApiGateway", "10.10.188.136:6379,allowadmin=true");
             .AddConfigStoredInMySql(Configuration.GetValue<string>("Project:Name"));
            // 添加监控

            // 添加Orm
            services.AddScoped(typeof(ISqlSugarDbRepository<>), typeof(ISqlSugarDbRepository<>));
        }
        /// <summary>
        /// 授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="isHttps"></param>
        private void AddOcelotJwtBearer(IServiceCollection services, IConfiguration configuration, bool isHttps = false)
        {
            var config = configuration.GetSection("JwtAuthorize");
            var keyByteArray = Encoding.ASCII.GetBytes(config["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = config["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = bool.Parse(config["RequireExpirationTime"])
            };
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = config["DefaultScheme"];
            })
            .AddJwtBearer(config["DefaultScheme"], opt =>
            {
                //不使用https
                opt.RequireHttpsMetadata = bool.Parse(config["IsHttps"]);
                opt.TokenValidationParameters = tokenValidationParameters;
            });
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            // 使用跨域
            app.UseCors("CorsPolicy");
            // 网关扩展中间件配置
            var configuration = new OcelotPipelineConfiguration()
            {
                // 扩展为健康检查地址
                PreErrorResponderMiddleware = async (ctx, next) =>
                {
                    if (ctx.HttpContext.Request.Path.Equals(new PathString("/")))
                    {
                        await ctx.HttpContext.Response.WriteAsync("ok");
                    }
                    else
                    {
                        await next.Invoke();
                    }
                },
            };
            // 增加DotNetty请求通道,因为最终会阻断通道,所以要包含部分中间件功能
            configuration.MapWhenOcelotPipeline.Add((build) =>
            {
                return (context) => context.DownstreamReRoute.DownstreamScheme.ToLower() == "netty";
            });
            // 使用监控

            // 使用网关
            app.UseOcelot(configuration).Wait();
            // Welcome
            Console.WriteLine(Welcome());
        }
        /// <summary>
        /// Welcome
        /// </summary>
        /// <returns></returns>
        private string Welcome()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Initializing ...");
            builder.AppendLine();
            builder.AppendLine("***************************************************************");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("*                Welcome To Pinzhi.ApiGateway                 *");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("***************************************************************");
            return builder.ToString();
        }
    }
}
