using Bucket.AspNetCore.EventBus;
using Bucket.AspNetCore.Extensions;
using Bucket.EventBus.Common.Events;
using Bucket.Logging.EventHandlers;
using Bucket.Logging.Events;
using Bucket.Tracer;
using Bucket.Tracer.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Bucket.Logging.ConsoleApp
{
    class Program
    {
        private static IServiceCollection services;
        static void Main(string[] args)
        {
            Console.WriteLine("Bucket日志存储程序已启动");
            Initialize();
            services.AddBucket();
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "127.0.0.1";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                    opt.OnlyPublish = false;
                });
            });
            services.AddSingleton(p => new DbLogOptions
            {
                ConnectionString = "characterset=utf8;server=127.0.0.1;port=3306;user id=root;password=123;persistsecurityinfo=True;database=bucket",
                DbShardingRule = 0,
                DbType = "MySql",
                IsDbSharding = false,
                IsWriteConsole = false
            });
            services.AddSingleton<ITracerHandler, TracerHandler>();
            services.AddSingleton<ITracerStore, TracerEventStore>();
            services.AddSingleton<ElasticClientManager>();
            var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            // 事件订阅
            eventBus.Subscribe<LogEvent, DbLogEventHandler>();
            eventBus.Subscribe<TracerEvent, TracerEventHandler>();
        }
        private static void Initialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services = new ServiceCollection().AddLogging();
        }
    }
}
