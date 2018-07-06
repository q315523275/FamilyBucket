using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Newtonsoft.Json.Serialization;

using Bucket.AspNetCore.Extensions;
using Bucket.AspNetCore.EventBus;
using Microsoft.AspNetCore.Builder;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Bucket.Logging;

namespace Bucket.Ocelot
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 应用程序入口点
        /// </summary>
        /// <param name="args">入口点参数</param>
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddJsonFile("ocelot.json")
                        //.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                        .AddEnvironmentVariables();
                })
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
                    var eventConfig = hostingContext.Configuration.GetSection("EventBus").GetSection("RabbitMQ");
                    s.AddEventBus(option =>
                    {
                        option.UseRabbitMQ(opt =>
                        {
                            opt.HostName = eventConfig["HostName"];
                            opt.Port = Convert.ToInt32(eventConfig["Port"]);
                            opt.QueueName = eventConfig["QueueName"];
                        });
                    });
                    // 添加队列日志
                    s.AddEventLog();
                    // 添加链路
                    s.AddTracer(hostingContext.Configuration);
                    // 添加网关
                    s.AddOcelot()
                        .AddCacheManager(x => {
                            x.WithDictionaryHandle();
                        })
                        .AddAdministration("/administration", "axon@2018");
                    // 添加统计
                    #region 监控统计
                    //var metrics = AppMetrics.CreateDefaultBuilder()
                    //    .Configuration.Configure(options =>{
                    //        options.AddAppTag("RepairApp");
                    //        options.AddEnvTag("stage");
                    //    })
                    //    .Report.ToInfluxDb(options => {
                    //        options.InfluxDb.BaseUri = new Uri("http://192.168.1.199:8086");
                    //        options.InfluxDb.Database = "MetricsDB";
                    //        options.InfluxDb.UserName = "bucket";
                    //        options.InfluxDb.Password = "123456";
                    //        options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                    //        options.HttpPolicy.FailuresBeforeBackoff = 5;
                    //        options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                    //        options.FlushInterval = TimeSpan.FromSeconds(5);
                    //    })
                    //    .Build();
                    //services.AddMetrics(metrics);
                    //services.AddMetricsReportScheduler();
                    //services.AddMetricsTrackingMiddleware();
                    //services.AddMetricsEndpoints();
                    #endregion
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .UseIISIntegration()
                .Configure(app =>
                {
                    var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>().AddBucketLog(app, "Bucket.Ocelot");
                    app.UseCors("CorsPolicy");
                    app.UseOcelot().Wait();
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
