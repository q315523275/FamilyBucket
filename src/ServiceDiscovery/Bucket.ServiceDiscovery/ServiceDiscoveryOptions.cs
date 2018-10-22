using Bucket.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Bucket.ServiceDiscovery
{
    public class ServiceDiscoveryOptions
    {
        public ServiceDiscoveryOptions()
        {
            Extensions = new List<IOptionsExtension>();
        }

        internal IList<IOptionsExtension> Extensions { get; }
        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(IOptionsExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            Extensions.Add(extension);
        }
    }
}
