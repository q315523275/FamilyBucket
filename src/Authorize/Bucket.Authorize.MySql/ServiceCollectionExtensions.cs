using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize.MySql
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// mysql user scope authorize
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IAuthoriserBuilder UseMySqlAuthorize(this IAuthoriserBuilder authoriserBuilder)
        {
            var config = authoriserBuilder.Configuration.GetSection("JwtAuthorize");

            authoriserBuilder.Services.AddSingleton<IPermissionRepository>(sp => new MySqlPermissionRepository(config["MySqlConnectionString"], config["ProjectName"]));
            authoriserBuilder.Services.AddSingleton<IPermissionAuthoriser, ScopesAuthoriser>();
            return authoriserBuilder;
        }
    }
}
