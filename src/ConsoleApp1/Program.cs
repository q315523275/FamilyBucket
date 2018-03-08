
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

using Bucket.EventBus.Common.Events;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Logging.EventHandlers;
using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
using Bucket.ConfigCenter;
using Bucket.AspNetCore.ServiceDiscovery;
using Bucket.ServiceDiscovery;
using Bucket.LoadBalancer;
using Bucket.ErrorCode;

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
            loggerFactory.AddBucketLog(eventBus);
            services.AddSingleton(loggerFactory);
            ILogger logger = loggerFactory.CreateLogger<Program>();
            Console.WriteLine("事件驱动日志测试");
            Console.WriteLine("");
            // 事件订阅
            eventBus.Subscribe<PublishLogEvent, PublishLogEventHandler>();
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
            var configHelper = services.BuildServiceProvider().GetRequiredService<IConfigCenter>();
            Console.WriteLine("配置中心测试");
            Console.WriteLine("");
            logger.LogInformation("key hahaha值" + configHelper.Get("hahaha"));

            // 错误码中心
            services.AddErroCodeService(opt =>
            {
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://192.168.1.199:18080";
            });
            var codeHelper = services.BuildServiceProvider().GetRequiredService<IErrorCodeStore>();
            Console.WriteLine("错误码中心测试");
            Console.WriteLine("");
            logger.LogInformation("错误码GO_0004007值" + codeHelper.StringGet("GO_0004007"));

            // 服务发现
            services.AddServiceDiscovery(option =>
            {
                option.UseConsul(opt =>
                {
                    opt.HostName = "localhost";
                    opt.Port = 8500;
                });
            });
            Console.WriteLine("");
            Console.WriteLine("服务发现测试");
            Console.WriteLine("");

            var serviceDiscovery = services.BuildServiceProvider().GetRequiredService<IServiceDiscovery>();
            logger.LogInformation($"ConsulKV测试：{ JsonConvert.SerializeObject(serviceDiscovery.FindServiceInstancesAsync("consul").GetAwaiter().GetResult())}");

            Console.WriteLine("");
            Console.WriteLine("负载均衡测试");
            Console.WriteLine("");

            var loadBalancerHouse = services.BuildServiceProvider().GetRequiredService<ILoadBalancerHouse>();
            var _load = loadBalancerHouse.Get("consul").GetAwaiter().GetResult();
            var HostAndPort = _load.Lease().GetAwaiter().GetResult();
            logger.LogInformation($"负载后的接口地址：http://{HostAndPort.Address}:{HostAndPort.Port}");
            Console.WriteLine("");

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
