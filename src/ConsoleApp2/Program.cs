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

namespace ConsoleApp2
{
    class Program
    {
        private static IServiceCollection services;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Initialize();

            //services.AddEventBus(option => {
            //    option.UseRabbitMQ(opt =>
            //    {
            //        opt.HostName = "192.168.1.199";
            //        opt.Port = 5672;
            //        opt.ExchangeName = "BucketEventBus";
            //        opt.QueueName = "BucketEvents";
            //    });
            //});
            //var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            //// 日志初始化
            //Func<string, LogLevel, bool> filter = (category, level) => true;
            //ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddBucketLog(eventBus);
            //services.AddSingleton(loggerFactory);
            //ILogger logger = loggerFactory.CreateLogger<Program>();

            //// 事件订阅
            //eventBus.Subscribe<PublishLogEvent, PublishLogEventHandler>();
            //var i = 0;
            //while (i < 9)
            //{
            //    i++;
            //    logger.LogError(new Exception($"我是错误日志{i.ToString()}"), "1");
            //}

            HttpClient _httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

            _httpClient.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri("http://api.51pinzhi.cn/")
            });

            var i = 0;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
           
            while (i < 10)
            {
                watch.Start();
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"http://api.51pinzhi.cn/order/api/Query/QueryPrePayIP8"),
                    Method = HttpMethod.Post
                };
                request.Headers.Clear();
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                var httpResponseMessage = _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                httpResponseMessage.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                watch.Stop();
                Console.WriteLine($"耗时：{watch.Elapsed.TotalMilliseconds}ms");
                watch.Reset();
                i++;
            }

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
