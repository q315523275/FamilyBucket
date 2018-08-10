using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;

using Bucket.AspNetCore.Extensions;
using Bucket.Logging.EventSubscribe;
using Bucket.Logging.Events;
using Bucket.Tracing.EventSubscribe.Elasticsearch;
using Bucket.Tracing.Events;
using Bucket.Tracing.EventSubscribe;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.Abstractions;
using Bucket.EventBus.RabbitMQ;
using Bucket.Tracing.Extensions;
using Microsoft.Extensions.Logging;
using Bucket.Config.Extensions;

namespace Bucket.ConsoleApp
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        private static IConfiguration configuration { get; set; }
        private static void Initialize()
        {
            // 添加配置文件
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", true, true);
            configuration = builder.Build();
            // 添加DI容器
            var services = new ServiceCollection().AddOptions();
            // 添加基础设施服务
            services.AddBucket();
            // 添加日志及级别
            services.AddLogging(op => {
                op.AddConfiguration(configuration.GetSection("Logging"));
            });
            services.AddEventLog();
            // 添加配置服务
            services.AddConfigService(configuration);
            // 添加事件驱动
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(configuration);
            });
            // 添加链路追踪
            services.AddTracer(configuration);
            services.AddEventTrace();
            // 添加HttpClient
            services.AddHttpClient();
            //.SetHandlerLifetime(TimeSpan.FromMinutes(5));
            // 事件注册
            RegisterEventBus(services);
            // 容器
            serviceProvider = services.BuildServiceProvider();
            // 日志使用
        }
        /// <summary>
        /// 注册事件驱动
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterEventBus(IServiceCollection services)
        {
            services.AddSingleton(p => new DbLogOptions
            {
                ConnectionString = configuration.GetSection("SqlSugarClient")["ConnectionString"],
                DbShardingRule = 0,
                DbType = "MySql",
                IsDbSharding = false,
                IsWriteConsole = false
            });
            // 添加链路追踪ES消费配置
            services.Configure<ElasticsearchOptions>(configuration.GetSection("Elasticsearch"));
            services.AddSingleton<IIndexManager, IndexManager>();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddScoped<ISpanStorage, ElasticsearchSpanStorage>();
            // 事件执行器
            services.AddTransient<DbLogEventHandler>();
            services.AddTransient<TracingEventHandler>();
        }
        static void Main(string[] args)
        {
            // 初始化
            Initialize();
            // 业务
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            // 事件订阅
            eventBus.Subscribe<LogEvent, DbLogEventHandler>();
            eventBus.Subscribe<TracingEvent, TracingEventHandler>();
            eventBus.StartSubscribe();
            // 
            Console.WriteLine("基础事件订阅消费启动!");
        }
    }
}
