using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Bucket.SkrTrace.WebSocketServer
{
    public class Program
    {
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
                   .UseStartup<Startup>()
                   .UseUrls(hostingconfig.GetValue<string>("urls"))
                   .Build()
                   .Run();
        }
    }
}
