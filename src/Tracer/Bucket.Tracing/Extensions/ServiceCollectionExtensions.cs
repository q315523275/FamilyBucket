using Bucket.OpenTracing;
using Bucket.Tracing.AspNetCore;
using Bucket.Tracing.Components;
using Bucket.Tracing.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Bucket.Tracing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 链路追踪(基于EventBus)
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTracer(this IServiceCollection services, Action<TracingOptions> configAction)
        {
            if (configAction == null) throw new ArgumentNullException(nameof(configAction));

            services.Configure(configAction);

            services.TryAddSingleton<ISpanContextFactory, SpanContextFactory>();
            services.TryAddSingleton<ISampler, FullSampler>();
            services.TryAddSingleton<ITracer, Tracer>();
            services.TryAddSingleton<IServiceTracerProvider, ServiceTracerProvider>();
            services.TryAddSingleton<IServiceTracer, ServiceTracer>();
            services.TryAddSingleton<ITraceIdGenerator, TraceIdGenerator>();

            services.AddSingleton<IServiceTracer>(provider => provider.GetRequiredService<IServiceTracerProvider>().GetServiceTracer());
            services.AddSingleton<TracingDiagnosticListenerObserver>();
            services.AddSingleton<IHostedService, TracingHostedService>();
            services.AddSingleton<ITracingDiagnosticListener, HostingDiagnosticListener>();
            services.AddSingleton<ITracingDiagnosticListener, HttpClientDiagnosticListener>();

            return services;
        }
        /// <summary>
        /// 链路追踪(基于EventBus)
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTracer(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<TracingOptions>(configuration.GetSection("Tracing"));

            services.TryAddSingleton<ISpanContextFactory, SpanContextFactory>();
            services.TryAddSingleton<ISampler, FullSampler>();
            services.TryAddSingleton<ITracer, Tracer>();
            services.TryAddSingleton<IServiceTracerProvider, ServiceTracerProvider>();
            services.TryAddSingleton<IServiceTracer, ServiceTracer>();
            services.TryAddSingleton<ITraceIdGenerator, TraceIdGenerator>();

            services.AddSingleton<IServiceTracer>(provider => provider.GetRequiredService<IServiceTracerProvider>().GetServiceTracer());
            services.AddSingleton<TracingDiagnosticListenerObserver>();
            services.AddSingleton<IHostedService, TracingHostedService>();
            services.AddSingleton<ITracingDiagnosticListener, HostingDiagnosticListener>();
            services.AddSingleton<ITracingDiagnosticListener, HttpClientDiagnosticListener>();

            return services;
        }
    }
}
