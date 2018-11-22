using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Bucket.Authorize
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// In the API Project, the Startup. Cs class ConfigureServices method is called
        /// </summary>
        /// <param name="services">Service Collection</param>
        /// <param name="configuration">IConfiguration</param>
        /// <returns></returns>
        public static AuthenticationBuilder AddApiJwtAuthorize(this IServiceCollection services, IConfiguration configuration)
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
        /// use Authoriser
        /// </summary>
        /// <param name="authenticationBuilder"></param>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IAuthoriserBuilder UseAuthoriser(this AuthenticationBuilder authenticationBuilder, IServiceCollection services, IConfiguration configuration)
        {
            return new AuthoriserBuilder(services, configuration);
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
