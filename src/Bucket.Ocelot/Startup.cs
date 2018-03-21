using System;
using CacheManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
using Bucket.Logging;
using App.Metrics;

namespace Bucket.Ocelot
{
    public class Startup
    {
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

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Action<ConfigurationBuilderCachePart> settings = (x) =>
            {
                x.WithMicrosoftLogging(log =>
                {
                    log.AddConsole(LogLevel.Debug);
                })
                .WithDictionaryHandle();
            };
            // 认证参数
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
            // 跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            // 认证授权
            services.AddAuthentication()
                .AddJwtBearer(defaultScheme, opt =>
                {
                    //不使用https
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = tokenValidationParameters;
                });
            // 网关
            services.AddOcelot(Configuration)
                    //.AddStoreOcelotConfigurationInConsul()
                    .AddAdministration("/administration", "axon@2018");
            // 首字母大写
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            // 事件驱动
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "192.168.1.199";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
            });
            // 统计
            var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(options =>{
                    options.AddAppTag("RepairApp");
                    options.AddEnvTag("stage");
                })
                .Report.ToInfluxDb(options => {
                    options.InfluxDb.BaseUri = new Uri("http://192.168.1.199:8086");
                    options.InfluxDb.Database = "MetricsDB";
                    options.InfluxDb.UserName = "bucket";
                    options.InfluxDb.Password = "123456";
                    options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                    options.HttpPolicy.FailuresBeforeBackoff = 5;
                    options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                    options.FlushInterval = TimeSpan.FromSeconds(5);
                })
                .Build();
            services.AddMetrics(metrics);
            services.AddMetricsReportScheduler();
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddBucketLog(app, "Bucket.Ocelot");
            app.UseCors("CorsPolicy");
            app.UseMetricsAllMiddleware();
            app.UseMetricsAllEndpoints();
            app.UseOcelot().Wait();
        }
    }
}
