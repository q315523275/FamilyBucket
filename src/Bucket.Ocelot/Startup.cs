using System;
using CacheManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using Newtonsoft.Json.Serialization;

using Bucket.AspNetCore.Extensions;
using Bucket.AspNetCore.EventBus;
using Bucket.Logging;
using App.Metrics;

using System.Text;
using Bucket.AspNetCore.Filters;

namespace Bucket.Ocelot
{
    /// <summary>
    /// 启动配置
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化启动配置
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("configuration.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        /// <summary>
        /// 配置
        /// </summary>
        public IConfigurationRoot Configuration { get; }
        /// <summary>
        /// 配置服务
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // 授权认证
            AddOcelotJwtBearer(services);
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
            // 添加网关
            Action<ConfigurationBuilderCachePart> settings = (x) =>
            {
                x.WithMicrosoftLogging(log =>
                {
                    log.AddConsole(LogLevel.Debug);
                })
                .WithDictionaryHandle();
            };
            services.AddOcelot(Configuration)
                    .AddCacheManager(settings)
                    //.AddStoreOcelotConfigurationInConsul()
                    .AddAdministration("/administration", "axon@2018");
            // 添加首字母大写
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            // 添加事件驱动
            var eventConfig = Configuration.GetSection("EventBus").GetSection("RabbitMQ");
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = eventConfig["HostName"];
                    opt.Port = Convert.ToInt32(eventConfig["Port"]);
                    opt.ExchangeName = eventConfig["ExchangeName"];
                    opt.QueueName = eventConfig["QueueName"];
                });
            });
            // 添加队列日志
            services.AddEventLog();
            // 添加链路
            services.AddTracer(Configuration);
            // 添加统计
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
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddBucketLog(app, "Bucket.Ocelot");
            app.UseCors("CorsPolicy");
            //app.UseMetricsAllMiddleware();
            //app.UseMetricsAllEndpoints();
            app.UseOcelot().Wait();
        }
        /// <summary>
        /// 授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="isHttps"></param>
        private void AddOcelotJwtBearer(IServiceCollection services, bool isHttps = false)
        {
            var audienceConfig = Configuration.GetSection("Audience");
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
