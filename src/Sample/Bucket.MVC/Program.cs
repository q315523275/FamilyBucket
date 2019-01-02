using Bucket.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Bucket.MVC
{
    public class Program
    {
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
