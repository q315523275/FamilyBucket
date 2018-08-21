using Bucket.Gprc.Client;
using Bucket.Gprc.Extensions;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Bucket.Grpc.Client
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        private static IConfiguration configuration { get; set; }
        private static void Initialize()
        {
            // 添加配置文件
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", true, true);
            configuration = builder.Build();
            // 添加DI容器
            var services = new ServiceCollection();
            services.AddGrpcClient();
            // 容器
            serviceProvider = services.BuildServiceProvider();
            // 日志使用
        }
        static void Main(string[] args)
        {
            Initialize();

            Console.WriteLine("Hello World!");

            var _rpcChannelFactory = serviceProvider.GetRequiredService<IGrpcChannelFactory>();
            var channel = _rpcChannelFactory.Get("10.10.133.235", 5004);
            var StartTime = DateTime.Now;     
            for (var i = 0; i < 10000; i++)
            {
                var hel = MagicOnionClient.Create<IHello>(channel);
                var result = hel.Hello("haha test");
                Console.WriteLine(result.GetAwaiter().GetResult());
            }
            var TimeLength = Math.Round((DateTime.Now - StartTime).TotalMilliseconds, 4);
            Console.WriteLine("GRPC 10000次请求耗时" + TimeLength + "毫秒");
            Console.ReadLine();
        }
    }
    public interface IHello : IService<IHello>
    {
        UnaryResult<string> Hello(string name);
    }
}
