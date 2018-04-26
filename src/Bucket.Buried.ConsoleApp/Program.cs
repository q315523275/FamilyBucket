using Bucket.Buried.EventHandler.ElasticSearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Bucket.Buried.ConsoleApp
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        static void Main(string[] args)
        {
            Initialize();
            

            Console.WriteLine("Hello World!");
        }
        private static void Initialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<ESOptions>(op=> {  });
            services.AddSingleton<ESClientProvider>();

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
