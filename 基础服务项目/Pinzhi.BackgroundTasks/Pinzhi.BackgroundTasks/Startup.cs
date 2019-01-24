using Bucket.AspNetCore.Extensions;
using Bucket.Config.Extensions;
using Bucket.Config.HostedService;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.HostedService.AspNetCore;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Utility;
using Bucket.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pinzhi.Sms.Event;
using Pinzhi.Sms.EventSubscribe;
using Pinzhi.WxAppletTemplateMessage.Event;
using Pinzhi.WxAppletTemplateMessage.EventSubscribe;
using Pinzhi.User.EventSubscribe;
using Pinzhi.Logging.EventSubscribe;
using Pinzhi.User.Event;
using Autofac;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using System;

namespace Pinzhi.BackgroundTasks
{
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
            // mvc
            services.AddMvc();
            // 添加基础设施服务
            services.AddBucket();
            // 添加日志
            services.AddEventLog();
            // 添加数据库Orm
            services.AddSqlSugarDbContext();
            // 添加配置服务
            services.AddConfigServer(Configuration);
            // 添加事件驱动
            services.AddEventBus(option => { option.UseRabbitMQ(); });
            // 添加HttpClient
            services.AddHttpClient();
            // 添加工具
            services.AddUtil();
            // 添加缓存
            services.AddMemoryCache();
            // 添加定时任务
            services.AddBucketHostedService(builder => { builder.AddConfig(); });
            // 事件注册
            RegisterEventBus(services);
            // 添加autofac容器替换，默认容器注册方式缺少功能
            var autofac_builder = new ContainerBuilder();
            autofac_builder.Populate(services);
            autofac_builder.RegisterModule<AutofacModuleRegister>();
            AutofacContainer = autofac_builder.Build();
            return new AutofacServiceProvider(AutofacContainer);
        }
        /// <summary>
        /// 注册事件驱动
        /// </summary>
        /// <param name="services"></param>
        private void RegisterEventBus(IServiceCollection services)
        {
            // 事件
            services.AddScoped<ErrorLogEventHandler>();
            services.AddScoped<SmsEventHandler>();
            services.AddScoped<WxAppletTemplateMessageEventHandler>();
            services.AddScoped<UserActionEventHandler>();
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // 日志,事件驱动日志
            loggerFactory.AddBucketLog(app, Configuration.GetValue<string>("Project:Name"));
            // 事件订阅
            ConfigureEventBus(app);
        }
        /// <summary>
        /// 配置EventBus任务
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<Bucket.EventBus.Abstractions.IEventBus>();
            eventBus.Subscribe<LogEvent, ErrorLogEventHandler>();
            eventBus.Subscribe<SmsEvent, SmsEventHandler>();
            eventBus.Subscribe<WxAppletTemplateMessageEvent, WxAppletTemplateMessageEventHandler>();
            eventBus.Subscribe<UserActionEvent, UserActionEventHandler>();
            // start consume
            eventBus.StartSubscribe();
        }
    }
    /// <summary>
    /// Autofac扩展注册
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
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
}
