using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.HangFire.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //注入Hangfire服务
            services.AddHangfire(config => {
                config.UseRedisStorage("127.0.0.1:6379,allowadmin=true");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire");

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
