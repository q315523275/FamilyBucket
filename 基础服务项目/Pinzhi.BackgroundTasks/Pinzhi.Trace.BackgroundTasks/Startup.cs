using Bucket.AspNetCore.Extensions;
using Bucket.Config.Extensions;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Tracing.Events;
using Bucket.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pinzhi.Tracing.EventSubscribe;
using Pinzhi.Tracing.EventSubscribe.Elasticsearch;

namespace Pinzhi.Trace.BackgroundTasks
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
        /// 配置服务
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // mvc
            services.AddMvc();
            // 添加基础设施服务
            services.AddBucket();
            // 添加日志
            services.AddEventLog();
            // 添加配置服务
            services.AddConfigService(Configuration);
            // 添加事件驱动
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(Configuration);
            });
            // 添加HttpClient
            services.AddHttpClient();
            // 添加工具
            services.AddUtil();
            // 添加缓存
            services.AddMemoryCache();
            // 事件注册
            RegisterEventBus(services);
        }
        /// <summary>
        /// 注册事件驱动
        /// </summary>
        /// <param name="services"></param>
        private void RegisterEventBus(IServiceCollection services)
        {
            // 日志统计库
            var dbConnectionString = Configuration.GetValue<string>("SqlSugarClient:ConnectionString");
            // 添加链路追踪ES消费配置
            services.Configure<ElasticsearchOptions>(Configuration.GetSection("Elasticsearch"));
            services.AddSingleton<IIndexManager, IndexManager>();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddScoped<ISpanStorage, ElasticsearchSpanStorage>();
            services.AddScoped<IServiceStorage, ElasticsearchServiceStorage>();
            // 事件
            services.AddScoped<TracingEventHandler>();
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
            // 默认启动
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
        /// <summary>
        /// 配置EventBus任务
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<Bucket.EventBus.Abstractions.IEventBus>();
            eventBus.Subscribe<TracingEvent, TracingEventHandler>(); // es异常
            // start consume
            eventBus.StartSubscribe();
        }
    }
}
