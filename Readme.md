# FamilyBucket应用框架介绍

FamilyBucket主要通过组合各个系统形成的直接应用的微服务系统，当前仅对各组件进行了简单基础实现，[FamilyBucket-UI](https://github.com/q315523275/FamilyBucket-UI)持续开发中

## 配置中心应用

配置中心主要解决：单文件配置管理麻烦、容易遗漏、更新麻烦、配置共享、环境切换等情况

配置服务端主要提供http接口请求访问配置信息，通过appid和serct进行认证，一个appid下可以挂在多个项目和通用配置

目前配置更新的方式有两种，定时轮询和广播订阅，如果有共享redis环境，可以配置redis对应参数进行广播订阅实现实时订阅；

当前已支持net core默认IConfiguration的使用，组件相关appsetting配置可以移至配置中心
**一直配置的key的设置，如key JwtAuthorize:Secret

使用配置与方法

```csharp
  "ConfigService": {
    "AppId": "",
    "AppSercet": "",
    "RedisConnectionString": "",
    "RedisListener": false,
    "RefreshInteval": 300,
    "ServerUrl": "https://www.xxxx.cn/",
    "UseServiceDiscovery": false,
    "ServiceName": "Pinzhi.Config.WebApi",
    "NamespaceName": "Pinzhi.Credit",
    "Env": "dev"
  }
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      // 添加配置服务
      services.AddConfigService(Configuration);
  }
```
net core默认IConfiguration支持使用，当前代码比较low0.0待重构
```csharp
   .ConfigureAppConfiguration((hostingContext, _config) =>
   {
       _config
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", true, true)
       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
       .AddEnvironmentVariables(); // 添加环境变量
       var option = new BucketConfigOptions(); _config.Build().GetSection("ConfigService").Bind(option);
        _config.AddBucketConfig(option);
   })
```

## 日志中心应用

主要用户手机所有服务日志信息，方便统计管理、查看、告警

当前使用消息总线进行日志传输，通过elasticsearch进行存储，kibana进行查看

下一步将进行NLog、Log4集成扩展，增加存储收集方式，集成都以原生日志组件实现方式进行

使用配置与方法

```csharp
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      // 添加事件队列日志
      services.AddEventLog();
  }
  public void Configure(IApplicationBuilder app, IILoggerFactory loggerFactory)
  {
      // 日志,事件驱动日志
      loggerFactory.AddBucketLog(app, Configuration.GetValue<string>("Project:Name"));
   }
```


## 错误码应用

主要解决运营人员对于接口返回对应用业务描述（模型验证，业务办理失败，规则不满足等等），可设置错误码级别，是否告警通知，配置告警人员等等

原理是定义对应的Exception，遇到需要直接返回的时候，直接throw对应的异常，其中包含对应的错误码和对内描述，通过全局异常中间件进行对应转化；

```csharp
  "ErrorCodeService": {
    "RefreshInteval": 1800,
    "ServerUrl": "http://xxxx"
  },
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      // 添加错误码服务
      services.AddErrorCodeServer(Configuration);
  }
```

## 微服务网关

Ocelot作为服务的网关，已经很强大；[项目地址](https://github.com/ThreeMammals/Ocelot)；当前使用Consul进行网关配置信息存储

## 服务注册发现

目前使用Consul进行实现

```csharp
  "ServiceDiscovery": {
    "ServiceName": "xxxxx",
    "Version": "1.1.0",
    "HealthCheckTemplate": "",
    "Endpoint": "http://xxxx",
    "Consul": {
      "HttpEndpoint": "http://127.0.0.1:8500",
      "DnsEndpoint": {
        "Address": "127.0.0.1",
        "Port": 8500
      }
    }
  },
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      // 添加服务发现
      services.AddServiceDiscovery(build => { build.UseConsul(configuration); });
      services.AddLoadBalancer();
  }
  public void Configure(IApplicationBuilder app, IILoggerFactory loggerFactory)
  {
      app.UseConsulRegisterService(Configuration);
  }
```
服务地址查询

```csharp
  static void Main(string[] args)
  {
     Initialize();
     Console.WriteLine("Hello World!");
     var _loadBalancerHouse = serviceProvider.GetRequiredService<ILoadBalancerHouse>();
     var _rpcChannelFactory = serviceProvider.GetRequiredService<IGrpcChannelFactory>();
     // 服务发现地址
     var endpoints = _loadBalancerHouse.Get("Bucket.Grpc.Server").Result;
     var endpoint = endpoints.Lease().Result;
     var channel = _rpcChannelFactory.Get(endpoint.Address, endpoint.Port);
  }
```


## 链路追踪应用

参考 

[OpenSkywalking](https://github.com/OpenSkywalking/skywalking-netcore)
[butterfly](https://github.com/liuhaoyang/butterfly)

由于一些使用需求特性，故没有直接使用直接组件，进行了一些二次开发，目前正在重构

实现APM、调用链信息、耗时分析定位、问题定位、请求出入参的查看与分析；

目前使用消息总线进行传输、相关数据可以进行一些比较实用的扩展；


## 消息总线应用

在微服务中起到比较重要的作用，用法多样，可实现多种功能，分布式事务、消息队列、事件驱动等等

```csharp
  "EventBus": {
    "RabbitMQ": {
      "HostName": "10.10.133.205",
      "Port": 5672,
      "UserName": "guest",
      "Password": "guest",
      "QueueName": "xxxx"
    }
  },
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
     // 添加事件驱动
     services.AddEventBus(builder => { builder.UseRabbitMQ(Configuration); });
  }
```

## 度量监控应用

应用实时性能监控，流量走向，系统吞吐量等，主要通过App.Metrics客户端进行数据的提取，granfana + influx 进行数据的展示与存储；

## 用户认证应用

Ocelot网关路由，通过AuthenticationProviderKey 和 AllowedScopes 进行认证和权限验证, 路由一般我配置是整个子项目的基地址，所以认证授权已移至后向子服务

组件为Bucket.Authorize Bucket.Authorize.MySql

标签属性
```csharp
[Authorize("permission")]
```
当只需要认证token是否有效时
```csharp
services.AddApiJwtAuthorize(Configuration)
```
当需要对角色进行验证时
```csharp
services.AddApiJwtAuthorize(Configuration).UseAuthoriser(services, Configuration).UseMySqlAuthorize();
```
全配置
```csharp
"JwtAuthorize": {
    "Secret": "xxxxxxxxxxxxxxxxxxxxx",
    "Issuer": "poc",
    "Audience": "axon",
    "PolicyName": "permission",
    "DefaultScheme": "Bearer",
    "IsHttps": false,
    "RequireExpirationTime": true,
    "MySqlConnectionString": "characterset=utf8;server=127.0.0.1;port=3306;database=bucket;uid=root;pwd=123;",
    "ProjectName": "Pinzhi.Platform",
    "RefreshInteval": 300
  },
```

## 使用

mysql初始化文件 /基础服务项目/init_mysql.sql

基础服务项目

配置中心服务端项目  /基础服务项目/ConfigService

用户登陆项目  /基础服务项目/Authentication

基础消息总线消费端  /基础服务项目/Pinzhi.BackgroundTasks

基础管理项目  /基础服务项目/Pinzhi.Platform (微服务配置、服务管理、配置中心设置、用户角色菜单平台管理)

链路追踪查询项目  /基础服务项目/Tracing

前端VUE项目
[FamilyBucket-UI](https://github.com/q315523275/FamilyBucket-UI)正在开发中，其中包括很多页面操作，用户权限，项目资源、配置中心、网关路由、链路追踪等

## 项目包引用

Nuget搜索Bucket.xxxx

## 进程守护

对比之下，pm2比较好用，linux cd到目录直接 pm2 start pm2.json; 网关中带有示例


## 应用示例

```csharp
namespace Platform.WebApi
{
    /// <summary>
    /// 启动配置
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化启动配置
        /// </summary>
        /// <param name="configuration">配置</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// AutofacDI容器
        /// </summary>
        public IContainer AutofacContainer { get; private set; }
        /// <summary>
        /// 配置服务
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // 添加认证+MySql权限认证
            services.AddApiJwtAuthorize(Configuration).UseAuthoriser(services, Configuration).UseMySqlAuthorize();
            // 添加基础设施服务
            services.AddBucket();
            // 添加数据ORM
            services.AddSQLSugarClient<SqlSugarClient>(config => {
                config.ConnectionString = Configuration.GetSection("SqlSugarClient")["ConnectionString"];
                config.DbType = DbType.MySql;
                config.IsAutoCloseConnection = false;
                config.InitKeyType = InitKeyType.Attribute;
            });
            // 添加错误码服务
            services.AddErrorCodeServer(Configuration);
            // 添加配置服务
            services.AddConfigService(Configuration);
            // 添加事件驱动
            services.AddEventBus(option => { option.UseRabbitMQ(Configuration); });
            // 添加服务发现
            services.AddServiceDiscovery(option => { option.UseConsul(Configuration); });
            // 添加服务路由
            services.AddLoadBalancer();
            // 添加事件队列日志
            services.AddEventLog();
            // 添加链路追踪
            services.AddTracer(Configuration);
            services.AddEventTrace();
            // 添加模型映射,需要映射配置文件(考虑到性能未使用自动映射)
            services.AddAutoMapper();
            // 添加过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WebApiTracingFilterAttribute));
                options.Filters.Add(typeof(WebApiActionFilterAttribute));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "接口文档", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Pinzhi.Platform.WebApi.xml"));
                // Swagger验证部分
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() } });
            });
            // 添加工具
            services.AddUtil();
            // 添加HttpClient管理
            services.AddHttpClient();
            // 添加autofac容器替换，默认容器注册方式缺少功能
            var autofac_builder = new ContainerBuilder();
            autofac_builder.Populate(services);
            autofac_builder.RegisterModule<AutofacModuleRegister>();
            AutofacContainer = autofac_builder.Build();
            return new AutofacServiceProvider(AutofacContainer);
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // 日志
            loggerFactory.AddBucketLog(app, "Pinzhi.Platform");
            // 文档
            ConfigSwagger(app);
            // 公共配置
            CommonConfig(app);
        }
        /// <summary>
        /// 配置Swagger
        /// </summary>
        private void ConfigSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1");
            });
        }
        /// <summary>
        /// 公共配置
        /// </summary>
        private void CommonConfig(IApplicationBuilder app)
        {
            // 全局错误日志
            app.UseErrorLog();
            // 静态文件
            app.UseStaticFiles();
            // 路由
            ConfigRoute(app);
            // 服务注册
            app.UseConsulRegisterService(Configuration);
        }
        /// <summary>
        /// 路由配置,支持区域
        /// </summary>
        private void ConfigRoute(IApplicationBuilder app)
        {
            app.UseMvc(routes => {
                routes.MapRoute("areaRoute", "view/{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            });
        }
        /// <summary>
        /// Autofac扩展注册
        /// </summary>
        public class AutofacModuleRegister : Autofac.Module
        {
            /// <summary>
            /// 装载autofac方式注册
            /// </summary>
            /// <param name="builder"></param>
            protected override void Load(ContainerBuilder builder)
            {
                // 业务应用注册
                Assembly bus_assembly = Assembly.Load("Pinzhi.Platform.Business");
                builder.RegisterAssemblyTypes(bus_assembly)
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Business"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                // 数据仓储泛型注册
                builder.RegisterGeneric(typeof(RepositoryBase<>)).As(typeof(IRepositoryBase<>))
                    .InstancePerLifetimeScope();
            }
        }
    }
}
```