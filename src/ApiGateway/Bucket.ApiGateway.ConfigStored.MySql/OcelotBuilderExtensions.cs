using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;

namespace Bucket.ApiGateway.ConfigStored.MySql
{
    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddConfigStoredInMySql(this IOcelotBuilder builder, string apiGatewayKey)
        {
            // builder.Services.AddSingleton<IFileConfigurationPollerOptions, MySqlFileConfigurationPollerOptions>();
            builder.Services.AddHostedService<FileConfigurationPoller>();
            builder.Services.AddSingleton<IFileConfigurationRepository>(sp =>
            {
                return new MySqlFileConfigurationRepository(sp, apiGatewayKey);
            });
            return builder;
        }
    }
}
