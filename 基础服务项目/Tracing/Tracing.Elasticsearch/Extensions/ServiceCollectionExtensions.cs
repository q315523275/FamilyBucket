using System;
using Tracing.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Tracing.Elasticsearch
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.EnableElasticsearchStorage())
            {
                services.AddOptions();
                services.Configure<ElasticsearchOptions>(configuration);
                services.AddSingleton<IIndexManager, IndexManager>();
                services.AddSingleton<IElasticClientFactory, ElasticClientFactory>();
                services.AddScoped<ISpanStorage, ElasticsearchSpanStorage>();
                services.AddScoped<ISpanQuery, ElasticsearchSpanQuery>();
                services.AddScoped<IServiceQuery, ElasticSearchServiceQuery>();
                services.AddSingleton<IServiceStorage, ElasticSearchServiceStorage>();
            }

            return services;
        }
    }
}
