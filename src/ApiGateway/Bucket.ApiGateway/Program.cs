namespace Bucket.ApiGateway
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore;
    using Microsoft.Extensions.Logging;
    using Bucket.Logging;
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
            // 域名与端口配置
            var hostingconfig = new ConfigurationBuilder()
                .AddJsonFile("hosting.json", optional: false)
                .Build();
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
                   })
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).ClearProviders()
                              .AddBucketLog(hostingContext.Configuration.GetValue<string>("Project:Name"));
                   })
                   .UseStartup<Startup>()
                   .UseUrls(hostingconfig.GetValue<string>("urls"))
                   .Build()
                   .Run();
        }
    }
}