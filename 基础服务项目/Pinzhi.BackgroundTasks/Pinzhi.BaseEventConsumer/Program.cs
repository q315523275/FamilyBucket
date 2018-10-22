
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

using Bucket.AspNetCore.Extensions;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.Abstractions;
using Bucket.EventBus.RabbitMQ;
using Bucket.Logging.Events;

using Pinzhi.Sms.Event;
using Pinzhi.Sms.EventSubscribe;
using Pinzhi.Logging.EventSubscribe;
using Bucket.Config.Extensions;
using Bucket.Tracing.Events;
using Pinzhi.Tracing.EventSubscribe;
using Pinzhi.Tracing.EventSubscribe.Elasticsearch;

namespace Pinzhi.BaseEventConsumer
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            // 初始化
            Initialize();
            // 业务
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            // 事件订阅
            eventBus.Subscribe<LogEvent, DbLogEventHandler>();
            eventBus.Subscribe<SmsEvent, SmsEventHandler>();
            eventBus.Subscribe<TracingEvent, TracingEventHandler>();
            eventBus.StartSubscribe();
            Console.WriteLine("品值基础事件订阅消费启动!");
        }
        private static IConfiguration configuration { get; set; }
        private static void Initialize()
        {
            // 添加配置文件
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", true, true);
            configuration = builder.Build();
            // 添加DI容器
            var services = new ServiceCollection().AddOptions().AddLogging();
            // 添加基础设施服务
            services.AddBucket();
            services.AddMemoryCache();
            // 添加配置服务
            services.AddConfigService(configuration);
            // 添加事件驱动
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(configuration);
            });
            // 添加HttpClient工厂
            services.AddHttpClient();
            // 事件注册
            RegisterEventBus(services);
            // 容器
            serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 注册事件驱动
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterEventBus(IServiceCollection services)
        {
            // 添加链路追踪ES消费配置
            services.Configure<ElasticsearchOptions>(configuration.GetSection("Elasticsearch"));
            services.AddSingleton<IIndexManager, IndexManager>();
            services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
            services.AddScoped<ISpanStorage, ElasticsearchSpanStorage>();
            services.AddScoped<IServiceStorage, ElasticsearchServiceStorage>();
            // 添加日志消费数据库配置
            services.AddSingleton(p => new DbLogOptions
            {
                ConnectionString = configuration.GetSection("SqlSugarClient")["ConnectionString"],
                DbShardingRule = 0,
                DbType = "MySql",
                IsDbSharding = false,
                IsWriteConsole = true
            });
            // 事件
            services.AddTransient<DbLogEventHandler>();
            services.AddTransient<SmsEventHandler>();
            services.AddTransient<TracingEventHandler>();
        }
    }
}
