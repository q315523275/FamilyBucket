using Tracing.Common;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Tracing.Elasticsearch
{
    internal static class ConfigurationExtensions
    {
        public static bool EnableElasticsearchStorage(this IConfiguration configuration)
        {
            var storageType = configuration[EnvironmentUtils.StorageType];
            return storageType?.ToLower() == EnvironmentUtils.Elasticsearch;
        }
    }
}
