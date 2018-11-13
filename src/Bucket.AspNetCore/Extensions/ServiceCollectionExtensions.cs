
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

using Bucket.Core;
using Bucket.Redis;
using Bucket.ErrorCode;
using Bucket.AspNetCore.Commons;
using Bucket.AspNetCore.Serialize;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Bucket.AspNetCore.Authorize;
using Microsoft.AspNetCore.Authorization;

namespace Bucket.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 框架基础
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucket(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<RedisClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUser, HttpContextUser>();
            services.AddSingleton<IRequestScopedDataRepository, HttpDataRepository>();
            services.AddSingleton<IJsonHelper, DefaultJsonHelper>();
            services.AddSingleton<IErrorCode, EmptyErrorCode>();
            return services;
        }
        /// <summary>
        /// 跨域服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });
            return services;
        }

        /// <summary>
        /// In the API Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="validatePermission">validate permission action</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiJwtAuthorize(this IServiceCollection services, IConfiguration configuration, Func<HttpContext, bool> validatePermission)
        {
            var config = configuration.GetSection("JwtAuthorize");

            var keyByteArray = Encoding.ASCII.GetBytes(config["Secret"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config["Issuer"],
                ValidateAudience = true,
                ValidAudience = config["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = bool.Parse(config["RequireExpirationTime"])
            };
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var permissionRequirement = new JwtAuthorizationRequirement(config["Issuer"], config["Audience"], signingCredentials);
            permissionRequirement.ValidatePermission = validatePermission;

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton(permissionRequirement);
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(config["PolicyName"],
                          policy => policy.Requirements.Add(permissionRequirement));

            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = config["DefaultScheme"];
            })
            .AddJwtBearer(config["DefaultScheme"], o =>
            {
                o.RequireHttpsMetadata = bool.Parse(config["IsHttps"]);
                o.TokenValidationParameters = tokenValidationParameters;
            });
        }
        /// <summary>
        /// In the Authorize Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <returns></returns>
        public static IServiceCollection AddTokenJwtAuthorize(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("JwtAuthorize");
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Secret"])), SecurityAlgorithms.HmacSha256);
            var permissionRequirement = new JwtAuthorizationRequirement(config["Issuer"], config["Audience"], signingCredentials );
            services.AddSingleton<ITokenBuilder, TokenBuilder>();
            return services.AddSingleton(permissionRequirement);
        }
    }
}
