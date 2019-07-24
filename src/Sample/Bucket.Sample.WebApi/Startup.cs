using Bucket.Rpc;
using Bucket.Rpc.Codec.MessagePack;
using Bucket.Rpc.ProxyGenerator;
using Bucket.Rpc.Transport.DotNetty;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bucket.Sample.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddRpcCore().UseDotNettyTransport().UseMessagePackCodec().AddClientRuntime().AddServiceProxy();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();


            var serviceProxyProvider = app.ApplicationServices.GetRequiredService<IServiceProxyProvider>();

            var ipAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);

            var parameters = new Dictionary<string, object> { { "id", 100 } };

            var a = serviceProxyProvider.InvokeAsync<dynamic>(parameters, "Bucket.Sample.IUserService.GetUser", ipAddress).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(a));
        }
    }
}
