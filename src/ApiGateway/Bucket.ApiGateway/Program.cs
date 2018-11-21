namespace Bucket.ApiGateway
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Builder;

    using global::Ocelot.DependencyInjection;
    using global::Ocelot.Middleware;
    using Ocelot.Cache.CacheManager;
    using Ocelot.Administration;
    using Ocelot.Provider.Consul;
    using Ocelot.Provider.Polly;

    using Bucket.Logging;
    using Bucket.Logging.Events;
    using Bucket.EventBus.Extensions;
    using Bucket.EventBus.RabbitMQ;
    using Bucket.Tracing.Extensions;
    using Bucket.Tracing.Events;

    using Newtonsoft.Json.Serialization;
    using Microsoft.AspNetCore.Http;

    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // 端口域名配置
            var hosting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();
            // 默认启动
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("ocelot.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .UseUrls(hosting.GetValue<string>("urls"))
                .ConfigureServices((hostingContext, s) => {
                    // 授权认证
                    AddOcelotJwtBearer(s, hostingContext.Configuration);
                    // 添加跨域
                    s.AddCors(options =>
                    {
                        options.AddPolicy("CorsPolicy", builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                        );
                    });
                    // 添加首字母大写
                    s.AddMvc().AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    });
                    // 添加事件驱动
                    s.AddEventBus(option => { option.UseRabbitMQ(hostingContext.Configuration); });
                    // 添加队列日志
                    s.AddEventLog();
                    // 添加链路
                    s.AddTracer(hostingContext.Configuration);
                    s.AddEventTrace();
                    // 添加网关
                    s.AddOcelot()
                     .AddCacheManager(x =>
                     {
                         x.WithDictionaryHandle();
                     })
                     .AddPolly()
                     .AddConsul()
                     .AddConfigStoredInConsul();
                     // 添加监控

                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .Configure(app =>
                {
                    var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>().AddBucketLog(app, "Pinzhi.ApiGateway");
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
                })
                .Build()
                .Run();
        }
        /// <summary>
        /// 授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="isHttps"></param>
        private static void AddOcelotJwtBearer(IServiceCollection services, IConfiguration configuration, bool isHttps = false)
        {
            var audienceConfig = configuration.GetSection("Audience");
            var defaultScheme = audienceConfig["defaultScheme"];
            var keyByteArray = Encoding.ASCII.GetBytes(audienceConfig["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication()
                .AddJwtBearer(defaultScheme, opt =>
                {
                    //不使用https
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}