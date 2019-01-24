using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bucket.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Pinzhi.Trace.BackgroundTasks
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
