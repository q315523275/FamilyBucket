using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using SqlSugar;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Bucket.DbContext;
using Bucket.Utility;
using Bucket.Authorize;
using Bucket.Authorize.Listener;
using Bucket.Authorize.HostedService;
using Bucket.Authorize.MySql;
using Bucket.Config.Extensions;
using Bucket.Config.Listener;
using Bucket.Config.HostedService;
using Bucket.ErrorCode.Extensions;
using Bucket.ErrorCode.Listener;
using Bucket.ErrorCode.HostedService;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Listener.Extensions;
using Bucket.Listener.Redis;
using Bucket.HostedService.AspNetCore;
using Bucket.LoadBalancer.Extensions;
using Bucket.ServiceDiscovery.Extensions;
using Bucket.ServiceDiscovery.Consul.Extensions;
using Bucket.AspNetCore.Extensions;
using Bucket.AspNetCore.Filters;
using Bucket.Tracing.Extensions;
using Bucket.Tracing.Events;

namespace Pinzhi.Platform.WebApi
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
            services.AddApiJwtAuthorize(Configuration).UseAuthoriser(services, builder => { builder.UseMySqlAuthorize(); });
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
            services.AddConfigServer(Configuration);
            // 添加事件驱动
            services.AddEventBus(option => { option.UseRabbitMQ(); });
            // 添加服务发现
            services.AddServiceDiscovery(option => { option.UseConsul(); });
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
                // options.Filters.Add(typeof(WebApiTracingFilterAttribute));
                options.Filters.Add(typeof(WebApiActionFilterAttribute));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // 添加Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "品值POC接口文档", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Pinzhi.Platform.WebApi.xml"));
                // Swagger验证部分
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() } });
            });
            // 添加工具
            services.AddUtil();
            // 添加应用监听
            services.AddListener(builder => {
                builder.UseRedis();
                // builder.UseZookeeper();
                builder.AddAuthorize().AddConfig().AddErrorCode();
            });
            // 添加全局启动任务
            services.AddBucketHostedService(builder => {
                builder.AddAuthorize().AddConfig().AddErrorCode();
            });
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
        /// <summary>
        /// 配置请求管道
        /// </summary>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // 日志
            loggerFactory.AddBucketLog(app, "Pinzhi.Platform.WebApi");
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
            // 认证授权
            app.UseAuthentication();
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
