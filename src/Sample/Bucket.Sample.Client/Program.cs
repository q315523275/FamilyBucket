using Bucket.AspNetCore.Extensions;
using Bucket.Caching.Extensions;
using Bucket.Caching.InMemory;
using Bucket.Caching.StackExchangeRedis;
using Bucket.Config;
using Bucket.Config.Extensions;
using Bucket.Config.HostedService;
using Bucket.DbContext;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.HostedService.AspNetCore;
using Bucket.Logging.Events;
using Bucket.Rpc;
using Bucket.Rpc.Codec.MessagePack;
using Bucket.Rpc.ProxyGenerator;
using Bucket.Rpc.Transport.DotNetty;
using Bucket.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Bucket.Sample.Client
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .ConfigureHostConfiguration(config =>
                   {
                       if (args != null)
                       {
                           config.AddCommandLine(args);
                       }
                   })
                   .ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", true, true)
                         .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                         .AddEnvironmentVariables();
                       var option = new BucketConfigOptions();
                       config.Build().GetSection("ConfigServer").Bind(option);
                       config.AddBucketConfig(option);
                   })
                   .ConfigureServices((hostContext, services) =>
                   {
                       // 添加基础
                       services.AddBucketAspNetCore();
                       // 添加日志
                       services.AddLogEventTransport();
                       // 添加数据库Orm
                       services.AddSqlSugarDbContext().AddSqlSugarDbRepository();
                       // 添加配置服务
                       services.AddConfigServer(hostContext.Configuration);
                       // 添加事件驱动
                       services.AddEventBus(option => { option.UseRabbitMQ(); });
                       // 添加HttpClient
                       services.AddHttpClient();
                       // 添加缓存
                       services.AddMemoryCache();
                       // 添加定时任务
                       services.AddBucketHostedService(builder => { builder.AddConfig(); });
                       // 添加工具
                       services.AddUtil();
                       // 添加缓存组件
                       services.AddCaching(build =>
                       {
                           build.UseInMemory("default");
                           build.UseStackExchangeRedis(new Caching.StackExchangeRedis.Abstractions.StackExchangeRedisOption
                           {
                               Configuration = "10.10.188.136:6379,allowadmin=true",
                               DbProviderName = "redis"
                           });
                       });
                       // 事件注册
                       RegisterEventBus(services);

                       services.AddRpcCore()
                               .UseDotNettyTransport()
                               .UseMessagePackCodec()
                               .AddClientRuntime()
                               .AddServiceProxy();
                   })
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                       //       .ClearProviders()
                       //       .AddBucketLog("Pinzhi.BackgroundTasks");
                       logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).AddConsole().AddDebug();
                   })
                   .UseConsoleLifetime()
                   .Build();

            hostBuilder.ConfigureEventBus();

            await hostBuilder.RunRpcClientAsync();
            await hostBuilder.RunAsync();
            await hostBuilder.WaitForShutdownAsync();
        }
        /// <summary>
        /// 注册消息事件
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterEventBus(IServiceCollection services)
        {
            // 事件

        }
        /// <summary>
        /// 配置并启动消息事件订阅
        /// </summary>
        /// <param name="serviceProvider"></param>
        private static IHost ConfigureEventBus(this IHost host)
        {
            return host;
        }
        private static IHost RunLoadBalancer(this IHost host)
        {
            var services = new List<string> { "A", "B", "C", "D" };
            var _last = -1;

            Bucket.Utility.Helpers.Thread.ParallelExecute(() =>
            {
                Interlocked.Increment(ref _last);
                if (_last >= services.Count)
                {
                    //_last = 0;
                    Interlocked.Exchange(ref _last, 0);
                }
                Console.Write(services[_last]);
            }, 3000);
            return host;
        }

        private static async Task<IHost> RunRpcClientAsync(this IHost host)
        {
            var serviceProxyProvider = host.Services.GetRequiredService<IServiceProxyProvider>();

            var ipAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id", 100 }
            };

            //var a = await serviceProxyProvider.InvokeAsync<object>(parameters, "Bucket.Sample.IUserService.GetUser", ipAddress);
            //Console.WriteLine(JsonConvert.SerializeObject(a));

            do
            {
                Console.WriteLine("正在循环 1w次调用 GetUser.....");

                //1w次调用
                var watch = Stopwatch.StartNew();
                for (var i = 0; i < 10000; i++)
                {
                    var a = await serviceProxyProvider.InvokeAsync<string>(parameters, "/User/GetUserName", ipAddress);
                }
                watch.Stop();
                Console.WriteLine($"1w次调用结束，执行时间：{watch.ElapsedMilliseconds}ms");
                Console.WriteLine("Press any key to continue, q to exit the loop...");
                var key = Console.ReadLine();
                if (key.ToLower() == "q")
                    break;
            } while (true);
            return host;
        }
    }
}
