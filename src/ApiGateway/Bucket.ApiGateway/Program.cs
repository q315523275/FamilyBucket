using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Bucket.ApiGateway
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureLogging((hostingContext, logging) =>
                   {
                       //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).ClearProviders()
                       //       .AddBucketLog(hostingContext.Configuration.GetValue<string>("Project:Name"));
                       logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")).AddConsole().AddDebug();
                   })
                   .UseStartup<Startup>()
                   .Build()
                   .Run();
        }
    }
}