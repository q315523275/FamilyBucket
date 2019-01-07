using Bucket.Values;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.ServiceDiscovery.Consul.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsulRegisterService(this IApplicationBuilder app, IConfiguration configuration)
        {
            ServiceDiscoveryOption serviceDiscoveryOption = new ServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);
            app.UseConsulRegisterService(serviceDiscoveryOption);
            return app;
        }

        public static IApplicationBuilder UseConsulRegisterService(this IApplicationBuilder app, ServiceDiscoveryOption serviceDiscoveryOption)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ??
               throw new ArgumentException("Missing Dependency", nameof(IApplicationLifetime));

            if (string.IsNullOrEmpty(serviceDiscoveryOption.ServiceName))
                throw new ArgumentException("service name must be configure", nameof(serviceDiscoveryOption.ServiceName));

            Uri address = null;
            if (!string.IsNullOrWhiteSpace(serviceDiscoveryOption.Endpoint))
                address = new Uri(serviceDiscoveryOption.Endpoint);
            else
            {
                var features = app.Properties["server.Features"] as FeatureCollection;
                address = features.Get<IServerAddressesFeature>()?.Addresses?.Select(p => new Uri(p))?.FirstOrDefault();
            }

            if (address != null)
            {
                Uri healthCheck = null;
                if (!string.IsNullOrEmpty(serviceDiscoveryOption.HealthCheckTemplate))
                    healthCheck = new Uri(serviceDiscoveryOption.HealthCheckTemplate);

                var registryInformation = app.AddTenant(serviceDiscoveryOption.ServiceName,
                    serviceDiscoveryOption.Version,
                    address,
                    serviceType: serviceDiscoveryOption.ServiceType,
                    healthCheckUri: healthCheck,
                    tags: new[] { $"urlprefix-/{serviceDiscoveryOption.ServiceName}"
                    });

                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    app.RemoveTenant(registryInformation.Id);
                });
            }
            return app;
        }

        private static string GetServiceId(string serviceName, Uri uri)
        {
            return $"{serviceName}_{uri.Host.Replace(".", "_")}_{uri.Port}";
        }

        public static ServiceInformation AddTenant(this IApplicationBuilder app, string serviceName, string version, Uri uri, ServiceType serviceType = ServiceType.HTTP, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
            var registryInformation = serviceRegistry.RegisterServiceAsync(serviceName, version, uri, serviceType, healthCheckUri, tags)
                .Result;

            return registryInformation;
        }

        public static bool RemoveTenant(this IApplicationBuilder app, string serviceId)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (string.IsNullOrEmpty(serviceId))
            {
                throw new ArgumentNullException(nameof(serviceId));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
            return serviceRegistry.DeregisterServiceAsync(serviceId)
                .Result;
        }

        public static string AddHealthCheck(this IApplicationBuilder app, ServiceInformation registryInformation, Uri checkUri, ServiceType serviceType = ServiceType.HTTP, TimeSpan? interval = null, string notes = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (registryInformation == null)
            {
                throw new ArgumentNullException(nameof(registryInformation));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
            string checkId = serviceRegistry.RegisterHealthCheckAsync(registryInformation.Name, registryInformation.Id, checkUri, serviceType, interval, notes)
                .Result;

            return checkId;
        }

        public static bool RemoveHealthCheck(this IApplicationBuilder app, string checkId)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (string.IsNullOrEmpty(checkId))
            {
                throw new ArgumentNullException(nameof(checkId));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
            return serviceRegistry.DeregisterHealthCheckAsync(checkId)
                .Result;
        }
    }
}
