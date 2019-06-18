using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bucket.HangFire.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 默认配置
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddEnvironmentVariables() // 添加环境变量
                .Build();

            WebHost.CreateDefaultBuilder(args)
                  .UseKestrel()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .ConfigureAppConfiguration((hostingContext, _config) =>
                  {
                      _config
                      .AddJsonFile("appsettings.json", true, true)
                      .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                  })
                  .UseConfiguration(config)
                  .UseStartup<Startup>()
                  .Build()
                  .Run();
        }
    }
}
