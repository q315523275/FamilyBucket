using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
using Bucket.EventBus.Common.Events;
using Bucket.Logging.EventHandlers;
using Bucket.Logging.Events;
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
                    opt.HostName = "192.168.1.199";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
            });
            services.AddSingleton<DbLogOptions>(p => new DbLogOptions
            {
                ConnectionString = "characterset=utf8;server=192.168.1.199;port=3306;user id=root;password=123;persistsecurityinfo=True;database=Bucket",
                DbShardingRule = 0,
                DbType = "MySql",
                IsDbSharding = false,
                IsWriteConsole = false
            });
            var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            // 事件订阅
            eventBus.Subscribe<LogEvent, DbLogEventHandler>();
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
