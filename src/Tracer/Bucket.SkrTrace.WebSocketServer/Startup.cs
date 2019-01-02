using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Bucket.EventBus.Extensions;
using Bucket.EventBus.RabbitMQ;
using Bucket.WebSocketManager;
using Bucket.SkrTrace.WebSocketServer.Handler;
using Bucket.SkrTrace.Transport.EventBus;

namespace Bucket.SkrTrace.WebSocketServer
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
            // 添加事件驱动
            services.AddEventBus(builder => { builder.UseRabbitMQ(Configuration); });
            services.AddScoped<SkrTraceWeSocketServerHandler>();
            // 添加过滤器
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // 添加WebSocket
            services.AddWebSocketManager();
            // 跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 使用跨域
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseMvcWithDefaultRoute();
            app.MapWebSocketManager("/skrTrace", app.ApplicationServices.GetRequiredService<SkrTraceMessageHandler>());
            var eventBus = app.ApplicationServices.GetRequiredService<Bucket.EventBus.Abstractions.IEventBus>();
            eventBus.Subscribe<SkrTraceTransportEvent, SkrTraceWeSocketServerHandler>();
            eventBus.StartSubscribe();
        }
    }
}
