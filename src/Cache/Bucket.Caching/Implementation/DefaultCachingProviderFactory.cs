using Bucket.Caching.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Bucket.Caching.Implementation
{
    public class DefaultCachingProviderFactory : ICachingProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultCachingProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICachingProvider GetCachingProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            var provider = _serviceProvider.GetServices<ICachingProvider>()?
                                           .FirstOrDefault(x => x.ProviderName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
                throw new ArgumentException(nameof(provider));

            return provider;
        }

        public IRedisCachingProvider GetRedisProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name));

            var provider = _serviceProvider.GetServices<IRedisCachingProvider>()?
                                           .FirstOrDefault(x => x.ProviderName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
                throw new ArgumentException(nameof(provider));

            return provider;
        }
    }
}
