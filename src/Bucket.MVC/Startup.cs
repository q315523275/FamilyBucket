using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using SqlSugar;
using Bucket.AspNetCore.EventBus;
using Bucket.AspNetCore.Filters;
using Bucket.DbContext;
using Newtonsoft.Json.Serialization;
using Bucket.Logging;
using Bucket.AspNetCore.Extensions;

namespace Bucket.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region 全部组件测试
            // 授权认证
            var audienceConfig = Configuration.GetSection("Audience");
            services.AddBucketAuthentication(opt =>
            {
                opt.Audience = audienceConfig["Audience"]; ;
                opt.Issuer = audienceConfig["Issuer"];
                opt.Secret = audienceConfig["Secret"];
            });

            // ORM
            services.AddSQLSugarClient<SqlSugarClient>(config =>
            {
                config.ConnectionString = Configuration.GetSection("SqlSugarClient")["ConnectionString"];
                config.DbType = DbType.MySql;
                config.IsAutoCloseConnection = false;
                config.InitKeyType = InitKeyType.Attribute;
            });
            // 基础
            services.AddBucket();
            // 配置中心
            services.AddConfigService(opt =>
            {
                opt.AppId = "PinzhiGO";
                opt.AppSercet = "R9QaIZTc4WYcPaKFneKu6zKo4F34Vz5R";
                opt.RedisConnectionString = "";
                opt.RedisListener = false;
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://localhost:55523/";
                opt.NamespaceName = "Application";
                opt.UseServiceDiscovery = false;
                opt.ServiceName = "BucketConfigService";
            });
            // 事件驱动
            services.AddEventBus(option =>
            {
                option.UseRabbitMQ(opt =>
                {
                    opt.HostName = "192.168.1.199";
                    opt.Port = 5672;
                    opt.ExchangeName = "BucketEventBus";
                    opt.QueueName = "BucketEvents";
                });
            });
            // 服务发现
            //ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            //Configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);
            //services.AddServiceDiscovery(option =>
            //{
            //    option.UseConsul(opt =>
            //    {
            //        opt.HttpEndpoint = serviceDiscoveryOption.Consul.HttpEndpoint;
            //    });
            //});
            services.AddServiceDiscoveryConsul(Configuration);
            // 错误码中心
            services.AddErrorCodeService(opt =>
            {
                opt.RefreshInteval = 300;
                opt.ServerUrl = "http://122.192.33.40:18080";
            });
            // 使用服务发现的子服务接口请求
            services.AddServiceClient();
            // 埋点服务
            services.AddBuriedService();
            // 添加过滤器、首字母大写
            services.AddMvc(options =>
            {
                options.Filters.Add<WebApiActionFilterAttribute>();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            #endregion
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            #region 测试
            // loggerFactory.AddConsole().AddDebug();
            // 队列日志
            loggerFactory.AddBucketLog(app, "Bucket.MVC");
            // 编码格式
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 中间件
            app.UseErrorLog();
            app.UseAuthentication();

            app.UseMvc();
            #endregion

            // app.UseConsulRegisterService(Configuration);
        }
    }
}
