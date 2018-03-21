using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace Bucket.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder builder = new WebHostBuilder();
            builder.ConfigureServices(s => {
                s.AddSingleton(builder);
            });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 //.AddJsonFile("host.json", optional: true) //配置文件设置urls
                .AddCommandLine(args)   //添加对命令参数的支持
                .Build();

            builder.UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseConfiguration(config);
            var host = builder.Build();
            host.Run();
        }
    }
}
