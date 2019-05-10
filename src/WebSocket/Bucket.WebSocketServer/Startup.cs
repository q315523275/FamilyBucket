using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bucket.WebSocketManager;
using System;
using Bucket.WebSocketServer.MessageHandlers;
using Microsoft.Extensions.Logging;

namespace Bucket.WebSocketServer
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
            services.AddMvc();
            services.AddWebSocketManager();
            services.AddOptions();
            services.AddLogging();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole().AddDebug();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}/{id?}"
                );
            });
            app.MapWebSocketManager("/ws", app.ApplicationServices.GetService<BusMessageHandler>());
        }
    }
}
