using Bucket.Authorize.Abstractions;
using Bucket.Authorize.Implementation;
using Bucket.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;

namespace Bucket.Authorize.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Jwt认证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiJwtAuthorize(this IServiceCollection services, IConfiguration configuration, string section = "JwtAuthorize")
        {
            var config = configuration.GetSection(section);

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

            services.AddSingleton<IClaimsParser, ClaimsParser>();
            services.AddSingleton<IPermissionAuthoriser, EmptyAuthoriser>();
            services.AddSingleton<IPermissionRepository, EmptyPermissionRepository>();
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
        /// 添加Jwt认证,IServiceCollection扩展
        /// </summary>
        /// <param name="services"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiJwtAuthorize(this IServiceCollection services, string section = "JwtAuthorize")
        {
            var configService = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)configService.ImplementationInstance;
            return AddApiJwtAuthorize(services, configuration, section);
        }
        /// <summary>
        /// 添加Jwt认证,IBucketBuilder扩展
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiJwtAuthorize(this IBucketBuilder builder, string section = "JwtAuthorize")
        {
            return AddApiJwtAuthorize(builder.Services, builder.Configuration, section);
        }
        /// <summary>
        /// 添加权限验证
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection UseAuthoriser(this AuthenticationBuilder authenticationBuilder, Action<AuthoriserBuilder> action)
        {
            var configService = authenticationBuilder.Services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)configService.ImplementationInstance;
            var builder = new AuthoriserBuilder(authenticationBuilder.Services, configuration);
            action(builder);
            return authenticationBuilder.Services;
        }
        /// <summary>
        /// 添加Jwt认证授权参数
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddTokenJwtAuthorize(this IServiceCollection services, IConfiguration configuration, string section = "JwtAuthorize")
        {
            var config = configuration.GetSection(section);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["Secret"])), SecurityAlgorithms.HmacSha256);
            var permissionRequirement = new JwtAuthorizationRequirement(config["Issuer"], config["Audience"], signingCredentials);
            services.AddSingleton<ITokenBuilder, TokenBuilder>();
            return services.AddSingleton(permissionRequirement);
        }
    }
}
