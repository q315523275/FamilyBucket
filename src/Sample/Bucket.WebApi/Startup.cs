using Bucket.AspNetCore.Extensions;
using Bucket.AspNetCore.Filters;
using Bucket.Authorize;
using Bucket.Authorize.MySql;
using Bucket.Authorize.HostedService;
using Bucket.Authorize.Listener;
using Bucket.Config.Extensions;
using Bucket.Config.HostedService;
using Bucket.Config.Listener;
using Bucket.DbContext;
using Bucket.Utility;
using Bucket.ErrorCode.Extensions;
using Bucket.ErrorCode.HostedService;
using Bucket.ErrorCode.Listener;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.ServiceDiscovery.Extensions;
using Bucket.ServiceDiscovery.Consul.Extensions;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.SkyApm.Agent.AspNetCore;
using Bucket.SkyApm.Transport.EventBus;
using Bucket.HostedService.AspNetCore;
using Bucket.Listener.Extensions;
using Bucket.Listener.Redis;
using Bucket.WebApi.Hangfire;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Hangfire;
using Hangfire.Console;
using Hangfire.RecurringJobExtensions;

namespace Bucket.WebApi
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
            // 添加授权认证
            services.AddApiJwtAuthorize(Configuration)
                .UseAuthoriser(services, builder => { builder.UseMySqlAuthorize(); }); ;
            // 添加基础设施服务
            services.AddBucket();
            // 添加数据ORM
            services.AddSqlSugarDbContext();
            // 添加错误码服务
            services.AddErrorCodeServer(Configuration);
            // 添加配置服务
            services.AddConfigServer(Configuration);
            // 添加事件驱动
            services.AddEventBus(builder => { builder.UseRabbitMQ(); });
            // 添加服务发现
            services.AddServiceDiscovery(builder => { builder.UseConsul(); });
            // 添加事件队列日志和告警信息
            services.AddLogEventTransport();
            // 添加链路追踪
            services.AddBucketSkyApmCore().UseEventBusTransport();
            // 添加过滤器
            services.AddMvc(option => { option.Filters.Add(typeof(WebApiActionFilterAttribute)); }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // 添加接口文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "微服务全家桶接口服务", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Pinzhi.Credit.WebApi.xml"));
                c.CustomSchemaIds(x => x.FullName);
                    // Swagger验证部分
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() } });
            });
            // 添加工具
            services.AddUtil();
            // 添加HttpClient管理
            services.AddHttpClient();
            // 添加业务组件注册
            // 添加组件定时任务
            services.AddBucketHostedService(builder => { builder.AddConfig().AddErrorCode().AddAuthorize(); });
            // 添加组件任务订阅
            services.AddListener(builder =>
            {
                //builder.UseZookeeper();
                builder.UseRedis().AddAuthorize().AddConfig().AddErrorCode();
            });
            // 添加事件消息
            RegisterEventBus(services);
            // 添加数据仓储操作
            services.AddScoped(typeof(IDbRepository<>), typeof(SqlSugarRepository<>));
            // 添加应用批量注册
            // services.BatchRegisterService(Assembly.Load("Bucket.Demo.Repository"), "Repository", ServiceLifetime.Scoped);
            // 注册调度任务
            RegisterScheduler(services);
        }
        /// <summary>
        /// 注册事件驱动
        /// </summary>
        /// <param name="services"></param>
        private void RegisterEventBus(IServiceCollection services)
        {
            // services.AddScoped<Event>();
        }
        /// <summary>
        /// 注册调度任务
        /// </summary>
        /// <param name="services"></param>
        private void RegisterScheduler(IServiceCollection services)
        {
            if (Configuration.GetSection("Scheduler").GetValue<bool>("Enable"))
            {
                services.AddHangfire(build =>
                {
                    build.UseRedisStorage(Configuration.GetSection("Scheduler").GetValue<string>("RedisServer"));
                    build.UseConsole();
                    build.UseRecurringJob("recurringjob.json");
                    build.UseDefaultActivator();
                });
            }
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // 日志,事件驱动日志
            loggerFactory.AddBucketLog(app, Configuration.GetValue<string>("Project:Name"));
            // 文档
            ConfigSwagger(app);
            // 公共配置
            CommonConfig(app);
            // Use EventBus
            ConfigureEventBus(app);
            // 调度任务
            ConfigureScheduler(app);
        }
        /// <summary>
        /// 配置Swagger
        /// </summary>
        private void ConfigSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1");
            });
        }
        /// <summary>
        /// 公共配置
        /// </summary>
        private void CommonConfig(IApplicationBuilder app)
        {
            // 全局错误日志
            app.UseErrorLog();
            // 静态文件
            app.UseStaticFiles();
            // 路由
            ConfigRoute(app);
            // 服务注册
            app.UseConsulRegisterService(Configuration);
        }
        /// <summary>
        /// 路由配置,支持区域
        /// </summary>
        private void ConfigRoute(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "view/{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            });
        }
        /// <summary>
        /// 配置EventBus任务
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<Bucket.EventBus.Abstractions.IEventBus>();
            // event subscribe


            // start consume
            eventBus.StartSubscribe();
        }
        /// <summary>
        /// 开启调度任务
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureScheduler(IApplicationBuilder app)
        {
            if (Configuration.GetSection("Scheduler").GetValue<bool>("Enable"))
            {
                // 容器载入Job服务
                HangfireServiceProvider.ServiceProvider = app.ApplicationServices;
                // 启动Job服务
                app.UseHangfireServer(new BackgroundJobServerOptions
                {
                    ServerName = Configuration.GetSection("Scheduler").GetValue<string>("ServerName")
                });
                // 启动Job界面UI
                app.UseHangfireDashboard(pathMatch: "/TaskManager", options: new DashboardOptions()
                {
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                });
            }
        }
    }
}