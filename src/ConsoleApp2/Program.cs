using Bucket.EventBus.AspNetCore;
using Bucket.EventBus.Common.Events;
using Bucket.EventBus.RabbitMQ;
using Bucket.Logging;
using Bucket.Logging.EventHandlers;
using Bucket.Logging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.IO;
using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Bucket.Utility.Helpers;

namespace ConsoleApp2
{
    class Program
    {
        private static IServiceCollection services;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Initialize();

            Console.WriteLine(Id.ObjectId());
            Console.WriteLine(Time.GetUnixTimestamp());
            Console.ReadLine();
        }
        private static void Initialize()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services = new ServiceCollection()
                .AddLogging();
        }
    }
}
