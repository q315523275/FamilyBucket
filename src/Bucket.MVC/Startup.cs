using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using SqlSugar;
using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
using Bucket.AspNetCore.ServiceDiscovery;
using Bucket.AspNetCore.Middleware;
using Bucket.AspNetCore.Filters;
using Bucket.DbContext;
using Newtonsoft.Json.Serialization;
using Bucket.EventBus.Common.Events;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Logging.EventHandlers;
using Bucket.Buried;
using Bucket.Buried.EventHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Bucket.ServiceDiscovery.Consul;

namespace Bucket.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region 全部组件测试
            // 授权认证
            var audienceConfig = Configuration.GetSection("Audience");
            services.AddBucketAuthentication(opt =>
            {
                opt.Audience = audienceConfig["Audience"]; ;
                opt.Issuer = audienceConfig["Issuer"];
                opt.Secret = audienceConfig["Secret"];
            });

            // ORM
            services.AddSQLSugarClient<SqlSugarClient>(config =>
            {
                config.ConnectionString = Configuration.GetSection("SqlSugarClient")["ConnectionString"];
                config.DbType = DbType.MySql;
                config.IsAutoCloseConnection = false;
                config.InitKeyType = InitKeyType.Attribute;
            });
            // 基础
            services.AddBucket();
            // 配置中心
            services.AddConfigService(opt =>
            {
                opt.AppId = "12313";
                opt.AppSercet = "213123123213";
                opt.RedisConnectionString = "";
                opt.RedisListener = false;
                opt.RefreshInteval = 30;
                opt.ServerUrl = "http://localhost:63430";
                opt.UseServiceDiscovery = false;
                opt.ServiceName = "BucketConfigService";
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
            // 服务发现
            //ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            //Configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);
            //services.AddServiceDiscovery(option =>
            //{
            //    option.UseConsul(opt =>
            //    {
            //        opt.HttpEndpoint = serviceDiscoveryOption.Consul.HttpEndpoint;
            //    });
            //});
            services.AddServiceDiscoveryConsul(Configuration);
            // 错误码中心
            services.AddErroCodeService(opt =>
            {
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://127.0.0.1:18080";
            });
            // 使用服务发现的子服务接口请求
            services.AddServiceClient();
            // 埋点服务
            services.AddBuriedService();
            // 添加过滤器、首字母大写
            services.AddMvc(options =>
            {
                options.Filters.Add<WebApiActionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            #region 测试
            // loggerFactory.AddConsole().AddDebug();
            // 事件总线
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            // 队列日志
            loggerFactory.AddBucketLog(eventBus);
            eventBus.Subscribe<PublishLogEvent, PublishLogEventHandler>();
            // 日志事件订阅
            eventBus.Subscribe<BuriedEvent, BuriedEventHandler>();
            // 编码格式
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 中间件
            app.UseErrorHandling();
            app.UseAuthentication();

            app.UseMvc();
            #endregion

            app.UseConsulRegisterService(Configuration);
        }
    }
}
