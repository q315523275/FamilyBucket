using System;
using CacheManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bucket.Ocelot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("configuration.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Action<ConfigurationBuilderCachePart> settings = (x) =>
            {
                x.WithMicrosoftLogging(log =>
                {
                    log.AddConsole(LogLevel.Debug);
                })
                .WithDictionaryHandle();
            };

            var audienceConfig = Configuration.GetSection("Audience");
            var defaultScheme = audienceConfig["defaultScheme"];
            var keyByteArray = Encoding.ASCII.GetBytes(audienceConfig["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Issuer"],//发行人
                ValidateAudience = true,
                ValidAudience = audienceConfig["Audience"],//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication()
                .AddJwtBearer(defaultScheme, opt =>
                {
                    //不使用https
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddOcelot(Configuration);
                    //.AddStoreOcelotConfigurationInConsul();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            app.UseOcelot().Wait();
        }
    }
}
