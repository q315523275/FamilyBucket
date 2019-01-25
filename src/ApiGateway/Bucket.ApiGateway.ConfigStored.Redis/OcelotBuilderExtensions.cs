
using Bucket.ApiGateway.ConfigStored.Redis;
using Bucket.Redis;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;

namespace Bucket.ApiGateway.ConfigStored.MySql
{
    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddConfigStoredInRedis(this IOcelotBuilder builder, string apiGatewayKey, string redisConnectionString)
        {
            builder.Services.AddSingleton<RedisClient>();
            builder.Services.AddHostedService<FileConfigurationPoller>();
            builder.Services.AddSingleton<IFileConfigurationRepository>(sp =>
            {
                return new RedisFileConfigurationRepository(sp.GetRequiredService<RedisClient>(), apiGatewayKey, redisConnectionString);
            });
            return builder;
        }
    }
}
