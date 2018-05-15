using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Pinzhi.Identity.WebApi
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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)   // 添加对命令参数的支持
                .AddEnvironmentVariables() // 添加环境变量
                .Build();

            WebHost.CreateDefaultBuilder(args)
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<Startup>()
                   .UseConfiguration(config)
                   .Build()
                   .Run();
        }
    }
}
