using Bucket.Config;
using Bucket.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Bucket.WebApi
{
    /// <summary>
    /// 应用程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 应用程序入口点
        /// </summary>
        /// <param name="args">入口点参数</param>
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((hostingContext, _config) =>
                   {
                       // 从配置中心拉取配置与appsettings.json配置进行合并,可用于组件注册
                       var option = new BucketConfigOptions();
                       _config.Build().GetSection("ConfigServer").Bind(option);
                       _config.AddBucketConfig(option);
                   })
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).ClearProviders()
                             .AddBucketLog(hostingContext.Configuration.GetValue<string>("Project:Name"));
                   })
                   .UseStartup<Startup>()
                   .Build()
                   .Run();
        }
    }
}
