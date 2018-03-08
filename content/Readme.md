## .net core 微服务系统
* 1、配置中心
* 2、日志中心
* 3、业务埋点
* 4、错误码中间件
* 5、服务注册发现
* 6、负载算法
* 7、子服务相互调用组件（使用5 6）
* 8、微服务网关
* 9、统一认证中心（jwt）
* 10、度量监控
* 11、链路追踪
* 12、事件驱动


## 1、配置中心
*  集中控制项目配置信息:

```csharp
using Bucket.AspNetCore;
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        //...

        services.AddConfigService(opt => {
		    opt.AppId = "12313",
            opt.AppSercet = "213123123213",
            opt.RedisConnectionString = "",
            opt.RedisListener = false,
            opt.RefreshInteval = 30,
            opt.ServerUrl = "http://localhost:63430",
            opt.UseServiceDiscovery = false,
            opt.ServiceName = "BucketConfigService"
		}); // << Add this line

        //...
    }
}
```

## 2、服务注册与发现
*  使用Consul实现服务注册与发现:

```csharp
using Bucket.AspNetCore;
using Bucket.AspNetCore.ServiceDiscovery;
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        //...

        services.AddServiceDiscovery(option => {
                option.UseConsul(opt =>
                {
                    opt.HostName = "localhost";
                    opt.Port = 8500;
                });
            });

        //...
    }
}
```

* 子服务注册服务发现:

```csharp
using Bucket.AspNetCore;
public class Startup
{
    //...
    private readonly CancellationTokenSource _consulConfigCancellationTokenSource = new CancellationTokenSource();
	public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
    {

            // add tenant & health check
            var localAddress = DnsHelper.GetIpAddressAsync().Result;
            var uri = new Uri($"http://{localAddress}:{Program.PORT}/");
            var registryInformation = app.AddTenant("values", "1.0.0-pre", uri, tags: new[] { "urlprefix-/values" });
            // register service & health check cleanup
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                app.RemoveTenant(registryInformation.Id);
                _consulConfigCancellationTokenSource.Cancel();
            });
    }
}
```
* 子服务间相互请求:

```csharp
using Bucket.AspNetCore;
public class Startup
{
    //...
	public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
    {

         // 使用服务发现的子服务接口请求
         services.AddServiceClient();
    }
}
// ceshi 
private readonly IServiceClient _serviceClient;
public UserService(IServiceClient serviceClient)
{
    _serviceClient = serviceClient;
}
```

## 3、事件驱动
* 使用RabbitMQ实现事件总线:

```csharp
using Bucket.AspNetCore;
using Bucket.AspNetCore.EventBus;
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        //...

         services.AddEventBus(option=> {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "localhost";
                    opt.Port = 8500;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
        });

        //...
    }
}
```

## 4、队列日志
* 使用事件驱动进行消息传输:

```csharp
public class Startup
{
    //...
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
          var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
		  loggerFactory.AddBucketLog(eventBus); // or loggerFactory.AddBucketLog(app);
		  // 日志消费
          eventBus.Subscribe<PublishLogEvent, PublishLogEventHandler>();
    }
}
```
* 控制台演示:

```csharp
class Program
    {
        private static IServiceCollection services;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Initialize();

            services.AddEventBus(option => {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "192.168.1.199";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
            });
            var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            // 日志初始化
            Func<string, LogLevel, bool> filter = (category, level) => true;
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddBucketLog(eventBus);
            services.AddSingleton(loggerFactory);
            ILogger logger = loggerFactory.CreateLogger<Program>();

            // 事件订阅
            eventBus.Subscribe<PublishLogEvent, PublishLogEventHandler>();
            var i = 0;
            while (i < 9)
            {
                i++;
                logger.LogError(new Exception($"我是错误日志{i.ToString()}"), "1");
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
```
![截图](http://10.10.141.47/tianliang/Microservice/blob/master/ConsoleApp2/logimg.png)


## 5、微服务网关
* 基于Consul + .NET Core + Polly + Ocelot + Exceptionless + IdentityServer

## 6、度量监控
* 基于App Metrics

## 7、统一认证
* 基于现有系统POC统一进行权限认证，使用Jwt
* 网关层进行用户授权验证，角色验证，权限验证
* 子服务使用内网，需要验证权限接口仅验证用户授权，不进行具体角色权限验证，通过HttpContext.User获取当前请求用户信息

```csharp
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        // 授权认证
        var audienceConfig = Configuration.GetSection("Audience");
        services.AddBucketAuthentication(opt =>
        {
            opt.Audience = audienceConfig["Audience"]; ;
            opt.DefaultScheme = audienceConfig["DefaultScheme"];
            opt.Issuer = audienceConfig["Issuer"];
            opt.Secret = audienceConfig["Secret"];
        });  
    }
}
[Authorize]
public OutputLogin Login([FromBody] InputLogin input){}
```

## 8、错误码中心
* 接口返回对应编码，需要进行运营专业描述语调整

```csharp
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        // 错误码中心
        services.AddErroCodeService(opt =>
        {
            opt.RefreshInteval = 300;
            opt.ServerUrl = "http://122.192.33.40:18080";
        });
    }
}
// 模型
[NotEmpty("001",ErrorMessage = "账号不能为空")]
public string UserName { set; get; }
// 错误码及对应业务描述
throw new BucketException(errorInfo.ErrorCode, errorInfo.Message);
```

## 9、埋点服务
* 

```csharp
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        // 埋点服务
        services.AddBuriedService();
    }
}
```