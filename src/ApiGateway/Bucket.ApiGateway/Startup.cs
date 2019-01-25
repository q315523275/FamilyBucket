
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Bucket.DbContext;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Tracing.Extensions;
using Bucket.Tracing.Events;
using Bucket.ApiGateway.ConfigStored.MySql;

using global::Ocelot.DependencyInjection;
using global::Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

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
        /// AutofacDI容器
        /// </summary>
        public IContainer AutofacContainer { get; private set; }
        /// <summary>
        /// 配置服务
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services)
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
            // 添加队列日志
            services.AddEventLog();
            // 添加链路
            //services.AddTracer(Configuration);
            //services.AddEventTrace();
            // 添加Orm
            services.AddSqlSugarDbContext(ServiceLifetime.Transient);
            // 添加网关
            services.AddOcelot()
             .AddCacheManager(x =>
             {
                 x.WithDictionaryHandle();
             })
             .AddPolly()
             .AddConsul()
             //.AddConfigStoredInConsul()
             .AddConfigStoredInRedis("Bucket.ApiGateway", "10.10.188.136:6379,allowadmin=true");
            //.AddConfigStoredInMySql("Bucket.ApiGateway");
            // 添加监控

            // 添加autofac容器替换，默认容器注册方式缺少功能
            var autofac_builder = new ContainerBuilder();
            autofac_builder.Populate(services);
            autofac_builder.RegisterModule<AutofacModuleRegister>();
            AutofacContainer = autofac_builder.Build();
            return new AutofacServiceProvider(AutofacContainer);
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
        /// Autofac扩展注册
        /// </summary>
        private class AutofacModuleRegister : Autofac.Module
        {
            /// <summary>
            /// 装载autofac方式注册
            /// </summary>
            /// <param name="builder"></param>
            protected override void Load(ContainerBuilder builder)
            {
                // 数据仓储泛型注册
                builder.RegisterGeneric(typeof(SqlSugarRepository<>)).As(typeof(IDbRepository<>))
                    .InstancePerLifetimeScope();
            }
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime"></param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // 使用跨域
            app.UseCors("CorsPolicy");
            // 健康检查地址
            var conf = new OcelotPipelineConfiguration()
            {
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
                }
            };
            // 使用监控

            // 使用网关
            app.UseOcelot(conf).Wait();
            // 日志,事件驱动日志
            loggerFactory.AddBucketLog(app, Configuration.GetValue<string>("Project:Name"));
            // Autofac容器释放
            appLifetime.ApplicationStopped.Register(() => { AutofacContainer.Dispose(); });
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
