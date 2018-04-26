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


## 1、子服务使用
*  配置信息:
{
  "Logging": {
    "IncludeScopes": true,
    "LogLevel": {
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ServiceDiscovery": {
    "ServiceName": "Pinzhi.Platform.Service",
    "Version": "1.0.0-pre",
    "HealthCheckTemplate": "",
    "Endpoints": [ "http://192.168.1.199:8093" ],
    "Consul": {
      "HttpEndpoint": "http://192.168.1.199:8500",
      "DnsEndpoint": {
        "Address": "192.168.1.199",
        "Port": 8500
      }
    }
  },
  "ConfigService": {
    "AppId": "PinzhiGO",
    "AppSercet": "R9QaIZTcu6zKo4F34Vz5R",
    "RedisConnectionString": "",
    "RedisListener": false,
    "RefreshInteval": 300,
    "ServerUrl": "http://192.168.1.199:8091/",
    "UseServiceDiscovery": false,
    "ServiceName": "BucketConfigService",
    "NamespaceName": "Platform"
  },
  "ErrorCodeService": {
    "RefreshInteval": 1800,
    "ServerUrl": "http://122.192.33.40:18080"
  },
  "EventBus": {
    "RabbitMQ": {
      "HostName": "192.168.1.199",
      "Port": 5672,
      "ExchangeName": "BucketEventBus",
      "QueueName": "BucketEvents"
    }
  },
  "Audience": {
    "Secret": "Itzg4e4asdS4SNpUvx6IoXQD",
    "Issuer": "poc",
    "Audience": "aon"
  },
  "SqlSugarClient": {
    "ConnectionString": "characterset=utf8;server=192.168.1.199;port=3306;user id=root;password=123;persistsecurityinfo=True;database=Bucket",
    "DbType": "MySql",
    "InitKeyType": "Attribute",
    "IsAutoCloseConnection": false
  }
}


*  子服务配置:

```csharp
using Bucket.AspNetCore;
public class Startup
{
    //...
    public void ConfigureServices(IServiceCollection services)
    {
        //...
            // 添加授权认证
            services.AddBucketAuthentication(Configuration);
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
            services.AddErrorCodeService(Configuration);
            // 添加配置服务
            services.AddConfigService(Configuration);
            // 添加事件驱动
            var eventConfig = Configuration.GetSection("EventBus").GetSection("RabbitMQ");
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = eventConfig["HostName"];
                    opt.Port = Convert.ToInt32(eventConfig["Port"]);
                    opt.ExchangeName = eventConfig["ExchangeName"];
                    opt.QueueName = eventConfig["QueueName"];
                });
            });
            // 添加服务发现
            services.AddServiceDiscoveryConsul(Configuration);
            // 添加追踪
            services.AddSingleton<ITracerHandler, TracerHandler>();
            services.AddSingleton<ITracerStore, TracerEventStore>();
            // 添加模型映射,需要映射配置文件(考虑到性能未使用自动映射)
            services.AddAutoMapper();
            // 添加业务注册

            // 添加过滤器
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WebApiTraceFilterAttribute));
                options.Filters.Add<WebApiActionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "接口文档", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Platform.WebApi.xml"));
            });

        //...
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
            // 认证授权
            app.UseAuthentication();
            // 链路追踪
            app.UseTracer(new AspNetCore.Middleware.TracerOptions { Environment = "test", SystemName = "Bucket" });
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
}
```