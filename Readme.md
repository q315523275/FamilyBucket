# FamilyBucket应用框架介绍

FamilyBucket主要通过组合各个系统形成的直接应用的微服务系统，当前仅对各组件进行了简单基础实现

## 配置中心应用

在项目很多的情况下通过配置文件的方式，其对应的弊端越来越明显，管理麻烦、容易遗漏，尤其在更改生产环境的配置时更不用说了；

配置中心主要通过接口请求方式获取项目对应的配置信息，通过appid和serct方式进行认证，一个appid下可以挂在多个项目和通用配置；

目前配置更新的方式有两种，定时轮询和广播订阅，如果有共享redis环境，可以配置redis对应参数进行广播订阅实现实时订阅；

目前使用redis方式进行实时更新，如果没有对应的环境可以升级为http长连接推送方式；

## 日志中心应用

在很多时候基本都是人手一份日志，最后弄的服务器或者数据库到处都是日志文件；

当前使用事件队列方式进行日志传输，通过elasticsearch进行存储，kibana进行查看；

也可以使用ELK方式进行日志收集与存储，通过NLOG或者LOG4NET进行日志文件输出；

## 错误码应用

这个应用有点鸡肋，主要应用原因，是因为运营人员对于接口返回对应用业务描述（模型验证，业务办理失败，规则不满足等等），不断的修改，不胜其烦

原理是定义对应的Exception，遇到需要直接返回的时候，直接throw对应的异常，其中包含对应的错误码和对内描述，通过全局异常中间件进行对应转化；

从此过上清静的生活；

## 微服务网关

这个就不用说了，必然存在的东西，使用Ocelot作为服务的网关，网上资料很多；[项目地址](https://github.com/ThreeMammals/Ocelot)

## 服务注册发现

目前使用Consul进行服务的相关操作，Consul官方的UI缺少对应的服务管理，只能进行相关的查看；更多资料查看官方网址

在服务启动的时候通过配置文件信息进行服务注册，程序停止的时候进行服务的关闭；

接下来需要进行Eureka的服务管理的增加...

## 链路追踪应用

这个在业务办理系统中，起到非常重要的作用，业务系统一般都要穿越过很多的系统，如果没有这个，一有问题将会很难定位；

通过追踪的接入可以实现调用链信息、耗时分析定位、问题定位、请求出入参的查看与分析；

使用后可以发现可以有很多的扩展用户，比如定制的告警、用户的实时监控（测试人员输入手机号通过websocket实时展示其办理情况）等等；

当前使用事件队列方式进行传输，通过elasticsearch进行存储；

参考 [OpenSkywalking](https://github.com/OpenSkywalking/skywalking-netcore),[butterfly](https://github.com/liuhaoyang/butterfly),在butterfly扩展使用EventBus传输,(RPC构建中)，可直接用butterfly-ui查看结果

## 事件驱动应用

这个不用说了，在系统中可说是会经常用到的东西，如订单对应的相关事件，代码参考博友的文章；

## 度量监控应用

应用实时性能监控，流量走向，系统吞吐量等，主要通过App.Metrics客户端进行数据的提取，granfana + influx进行数据的展示与存储；

相关资料也有不少

## 用户认证应用

配置Ocelot路由的时候就会发现，网关是通过AuthenticationProviderKey和AllowedScopes进行认证授权的；

在/sample/Authentication demo项目中有一个简单的登陆应用；

子服务并未进行具体角色的验证，只进行token验证；

如果子服务也需要进行对应的角色验证，需要进行对应的扩展，目前资料也很多了；

## 其他

sample 里有配置中间的简单服务端，

mysql的几个基础库，包括用户的角色等

平台基础项目

FamilyBucket-UI正在开发中，其中包括很多页面操作，用户权限，项目资源、配置中心、网关路由、链路追踪等

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
        /// 配置服务
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
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
            // 添加事件队列日志
            services.AddEventLog();
            // 添加链路追踪
            services.AddTracer(Configuration);
            // 添加模型映射,需要映射配置文件(考虑到性能未使用自动映射)
            services.AddAutoMapper();
            // 添加业务注册

            // 添加过滤器
            services.AddMvc(options =>
            {
               options.Filters.Add(typeof(WebApiTraceFilterAttribute));
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
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Platform.WebApi.xml"));
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization: Bearer {token}",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                
            });
            // 添加工具
            services.AddUtil();
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
            app.UseTracer();
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
}
```