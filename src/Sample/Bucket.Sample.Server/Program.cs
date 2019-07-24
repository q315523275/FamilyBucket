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
using Bucket.Rpc.Server;
using Bucket.Rpc.Transport.DotNetty;
using Bucket.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Bucket.Sample.Server
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
                       // 添加Rpc服务
                       services.AddRpcCore()
                               .UseDotNettyTransport()
                               .UseMessagePackCodec()
                               .AddServiceRuntime();

                       //
                       services.AddScoped<IUserService, UserService>();
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

            await hostBuilder.RunServerAsync();
            await hostBuilder.RunAsync();
        }
        private static async Task<IHost> RunServerAsync(this IHost host)
        {
            var serviceHost = host.GetService<IServiceHost>();
            var config = host.GetService<IConfiguration>();
            //启动主机
            await serviceHost.StartAsync(new IPEndPoint(IPAddress.Parse(config.GetValue<string>("Address")), config.GetValue<int>("Port")));

            Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            return host;
        }
        private static T GetService<T>(this IHost host)
        {
            return host.Services.GetRequiredService<T>();
        }
    }
}
