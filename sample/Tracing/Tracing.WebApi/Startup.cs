using AutoMapper;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Tracing.Common;
using Tracing.Elasticsearch;

namespace Tracing.WebApi
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
            var mvcBuilder = services.AddMvc(option =>
            {
                option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
                option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Instance));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            }); ;


            services.AddResponseCompression();

            services.AddCors();

            services.AddAutoMapper();

            services.AddSwaggerGen(option => { option.SwaggerDoc("v1", new Info { Title = "tracing http api", Version = "v1" }); });

            services.AddElasticsearch(Configuration);
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "butterfly http api v1");
            });

            app.UseResponseCompression();

            app.UseCors(cors => cors.AllowAnyOrigin());

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
