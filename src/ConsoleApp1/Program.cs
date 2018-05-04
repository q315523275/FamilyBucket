using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.IO;

using Bucket.EventBus.Common.Events;
using Bucket.ConfigCenter;
using Bucket.ErrorCode;

using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Logging.EventHandlers;

using Bucket.AspNetCore.EventBus;
using Bucket.AspNetCore.Extensions;

namespace ConsoleApp1
{
    class Program
    {
        private static IServiceCollection services;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Initialize();

            // 基础
            services.AddBucket();
            services.AddEventBus(option => {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "192.168.1.199";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
            });
            var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            // 日志初始化
            Func<string, LogLevel, bool> filter = (category, level) => true;
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddBucketLog(eventBus, null, "test");
            services.AddSingleton(loggerFactory);
            ILogger logger = loggerFactory.CreateLogger<Program>();
            Console.WriteLine("事件驱动日志测试");
            Console.WriteLine("");
            // 事件订阅
            services.AddSingleton<DbLogOptions>(p => new DbLogOptions
            {
                ConnectionString = "characterset=utf8;server=192.168.1.199;port=3306;user id=root;password=123;persistsecurityinfo=True;database=Bucket",
                DbShardingRule = 0,
                DbType = "MySql",
                IsDbSharding = false,
                IsWriteConsole = false
            });
            eventBus.Subscribe<LogEvent, DbLogEventHandler>();
            // 配置中心
            services.AddConfigService(opt =>
            {
                opt.AppId = "Pinzhi.Platform";
                opt.AppSercet = "R9QaIZTc4WYcPaKFneKu6zKo4F34Vz5R";
                opt.RedisConnectionString = "";
                opt.RedisListener = false;
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://192.168.1.199:8091/";
                opt.UseServiceDiscovery = false;
                opt.ServiceName = "BucketConfigService";
            });
            var configHelper = services.BuildServiceProvider().GetRequiredService<IConfigCenter>();
            Console.WriteLine("配置中心测试");
            Console.WriteLine("");
            logger.LogInformation("key RedisDefaultServer值" + configHelper.Get("RedisDefaultServer"));

            // 错误码中心
            services.AddErrorCodeService(opt =>
            {
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://122.192.33.40:18080";
            });
            var codeHelper = services.BuildServiceProvider().GetRequiredService<IErrorCodeStore>();
            Console.WriteLine("错误码中心测试");
            Console.WriteLine("");
            logger.LogInformation("错误码GO_0004007值" + codeHelper.StringGet("GO_0004007"));

            Console.ReadKey();
        }
        private static void Initialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services = new ServiceCollection()
                .AddLogging();
        }
    }
}
