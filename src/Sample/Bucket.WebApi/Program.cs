using System.IO;
using Bucket.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
                  .UseKestrel()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .ConfigureAppConfiguration((hostingContext, _config) =>
                  {
                      _config
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", true, true)
                      .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                      .AddEnvironmentVariables(); // 添加环境变量

                      // 从配置中心拉取配置与appsettings.json配置进行合并,可用于组件注册
                      var option = new BucketConfigOptions();
                      _config.Build().GetSection("ConfigServer").Bind(option);
                      _config.AddBucketConfig(option);
                  })
                  .UseStartup<Startup>()
                  .Build()
                  .Run();
        }
    }
}
