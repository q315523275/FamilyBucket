using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

using SqlSugar;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

using Bucket.Logging;
using Bucket.DbContext;
using Bucket.Config.Extensions;
using Bucket.ErrorCode.Extensions;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.AspNetCore.Filters;
using Bucket.AspNetCore.Extensions;
using Bucket.ServiceDiscovery.Extensions;
using Bucket.ServiceDiscovery.Consul.Extensions;
using Bucket.Logging.Events;
using Bucket.Authorize;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bucket.Authorize.MySql;
using Bucket.Listener.Extensions;
using Bucket.Listener.Zookeeper;
using Bucket.Authorize.Listener;
using Bucket.Authorize.HostedService;
using Bucket.Config.Listener;
using Bucket.Config.HostedService;
using Bucket.ErrorCode.Listener;
using Bucket.ErrorCode.HostedService;
using Bucket.HostedService.AspNetCore;

namespace Bucket.MVC
{
    /// <summary>
    /// 启动
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
            // 添加授权认证
            services.AddApiJwtAuthorize(Configuration).UseAuthoriser(services, builder => { builder.UseMySqlAuthorize(); });
            // 添加基础设施服务
            services.AddBucket();
            // 添加数据ORM
            services.AddSqlSugarDbContext();
            // 添加错误码服务
            services.AddErrorCodeServer(Configuration);
            // 添加配置服务
            services.AddConfigServer(Configuration);
            // 添加事件驱动
            services.AddEventBus(builder => { builder.UseRabbitMQ(); });
            // 添加服务发现
            services.AddServiceDiscovery(builder => { builder.UseConsul(); });
            // 添加链路追踪
            // 添加过滤器, 模型过滤器,追踪过滤器
            services.AddMvc(options => { options.Filters.Add(typeof(WebApiActionFilterAttribute)); }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            });
            // 添加事件队列日志
            services.AddEventLog();
            // 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "品值POC接口文档", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Bucket.MVC.xml"));
            });
            services.AddHttpClient();
            // 添加应用监听
            services.AddListener(builder => {
                //builder.UseRedis();
                builder.UseZookeeper();
                builder.AddAuthorize().AddConfig().AddErrorCode();
            });
            // 添加全局定时任务
            services.AddBucketHostedService(builder => {
                builder.AddAuthorize().AddConfig().AddErrorCode();
            });
            // 添加autofac容器替换，默认容器注册方式缺少功能
            var autofac_builder = new ContainerBuilder();
            autofac_builder.Populate(services);
            autofac_builder.RegisterModule<AutofacModuleRegister>();
            AutofacContainer = autofac_builder.Build();
            return new AutofacServiceProvider(AutofacContainer);
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
                // 数据仓储泛型注册
                builder.RegisterGeneric(typeof(SqlSugarRepository<>)).As(typeof(IDbRepository<>))
                    .InstancePerLifetimeScope();
            }
        }
        /// <summary>
        /// 配置请求管道
        /// </summary>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // 日志
            loggerFactory.AddBucketLog(app, "Bucket.MVC");
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
                routes.MapRoute("default", "{controller=Tool}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            });
        }
    }
}
