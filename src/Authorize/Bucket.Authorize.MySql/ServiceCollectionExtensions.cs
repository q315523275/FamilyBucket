using Bucket.Authorize.Abstractions;
using Bucket.Authorize.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize.MySql
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// mysql user scope authorize
        /// </summary>
        /// <param name="authoriserBuilder"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IAuthoriserBuilder UseMySqlAuthorize(this IAuthoriserBuilder authoriserBuilder, string section = "JwtAuthorize")
        {
            var config = authoriserBuilder.Configuration.GetSection(section);

            authoriserBuilder.Services.AddSingleton<IPermissionRepository>(sp => new MySqlPermissionRepository(config["MySqlConnectionString"], config["ProjectName"]));
            authoriserBuilder.Services.AddSingleton<IPermissionAuthoriser, ScopesAuthoriser>();
            return authoriserBuilder;
        }
    }
}
