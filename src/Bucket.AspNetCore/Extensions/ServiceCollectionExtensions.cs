
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

using Bucket.Core;
using Bucket.Redis;
using Bucket.ErrorCode;
using Bucket.AspNetCore.Commons;
using Bucket.AspNetCore.Authentication;

using System;
using System.Text;

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
            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<IErrorCode, EmptyErrorCode>();
            return services;
        }
        /// <summary>
        /// Bucket授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationOptions>(configuration.GetSection("Audience"));
            AuthenticationOptions config = new AuthenticationOptions();
            configuration.GetSection("Audience").Bind(config);
            var keyByteArray = Encoding.ASCII.GetBytes(config.Secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = config.Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication(options =>
            {
                //认证middleware配置
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                //不使用https
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = tokenValidationParameters;
            });
            return services;
        }
        /// <summary>
        /// Bucket授权认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBucketAuthentication(this IServiceCollection services, Action<AuthenticationOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            var config = new AuthenticationOptions();
            configAction?.Invoke(config);

            var keyByteArray = Encoding.ASCII.GetBytes(config.Secret);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = config.Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };
            services.AddAuthentication(options =>
            {
                //认证middleware配置
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                //不使用https
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = tokenValidationParameters;
            });

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
    }
}
