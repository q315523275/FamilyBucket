using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlSugar;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Bucket.DbContext;
using Bucket.Utility;
using Bucket.ErrorCode.Extensions;
using Bucket.Config.Extensions;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ;
using Bucket.ServiceDiscovery.Extensions;
using Bucket.ServiceDiscovery.Consul;
using Bucket.AspNetCore.Extensions;
using Bucket.AspNetCore.Filters;
using Bucket.Tracing.Extensions;
using Bucket.Tracing.Events;
using Bucket.Logging;
using Bucket.Logging.Events;
using Bucket.Authorize;
using Bucket.EventBus.RabbitMQ.Extensions;
using Bucket.ServiceDiscovery.Consul.Extensions;
using Bucket.HostedService.AspNetCore;
using Bucket.Config.HostedService;

namespace Pinzhi.Identity.WebApi
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
            // 添加授权认证
            services.AddTokenJwtAuthorize(Configuration);
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
            // 添加事件队列日志
            services.AddEventLog();
            // 添加全局定时服务
            services.AddBucketHostedService(builder => { builder.AddConfig(); });
            // 添加链路追踪
            services.AddTracer(Configuration);
            services.AddEventTrace();
            // 添加过滤器
            services.AddMvc(options => { options.Filters.Add(typeof(WebApiActionFilterAttribute)); }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            });
            // 添加接口文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "认证授权中心", Version = "v1" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Pinzhi.Identity.WebApi.xml"));
                // Swagger验证部分
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "请输入带有Bearer的Token", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Enumerable.Empty<string>() } });
            });
            // 添加工具
            services.AddUtil();
            // 添加HttpClient工厂
            services.AddHttpClient();
            // 添加业务组件
            // 添加autofac容器替换，默认容器注册方式缺少功能
            var autofac_builder = new ContainerBuilder();
            autofac_builder.Populate(services);
            autofac_builder.RegisterModule<AutofacModuleRegister>();
            AutofacContainer = autofac_builder.Build();
            return new AutofacServiceProvider(AutofacContainer);
        }
        /// <summary>
        /// <summary>
        /// 配置请求管道
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // 日志,事件驱动日志
            loggerFactory.AddBucketLog(app, Configuration.GetValue<string>("Project:Name"));
            // 文档
            ConfigSwagger(app);
            // 公共配置
            CommonConfig(app);
            // Autofac容器释放
            appLifetime.ApplicationStopped.Register(() => { AutofacContainer.Dispose(); });
        }
        /// <summary>
        /// 配置Swagger
        /// </summary>
        private void ConfigSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
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
            app.UseMvc(routes =>
            {
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
                Assembly bus_assembly = Assembly.Load("Pinzhi.Identity.Business");
                builder.RegisterAssemblyTypes(bus_assembly)
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Business"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                // 业务仓储注册
                Assembly bus_rop_assembly = Assembly.Load("Pinzhi.Identity.Repository");
                builder.RegisterAssemblyTypes(bus_rop_assembly)
                    .Where(t => !t.IsAbstract && !t.IsInterface && t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                // 数据仓储泛型注册
                builder.RegisterGeneric(typeof(SqlSugarRepository<>)).As(typeof(IDbRepository<>))
                    .InstancePerLifetimeScope();
            }
        }
    }
}
