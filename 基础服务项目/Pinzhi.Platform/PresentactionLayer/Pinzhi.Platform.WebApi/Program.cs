using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Pinzhi.Platform.WebApi
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
            // 默认配置
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((hostingContext, _config) => {
                       _config
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddEnvironmentVariables();
                   })
                   .UseKestrel()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<Startup>()
                   .UseUrls(config.GetValue<string>("urls"))
                   .Build()
                   .Run();
        }
    }
}
