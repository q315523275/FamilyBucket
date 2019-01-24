using System.IO;
using Bucket.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Pinzhi.BackgroundTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 域名端口配置
            var hosting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .ConfigureAppConfiguration((hostingContext, _config) => {
                       _config
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", true, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddEnvironmentVariables();
                       var option = new BucketConfigOptions();
                       _config.Build().GetSection("ConfigServer").Bind(option);
                       _config.AddBucketConfig(option);
                   })
                   .UseStartup<Startup>()
                   .UseUrls(hosting.GetValue<string>("urls"))
                   .Build()
                   .Run();
        }
    }
}
