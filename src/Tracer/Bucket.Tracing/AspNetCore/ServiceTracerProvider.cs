using Bucket.OpenTracing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Net;

namespace Bucket.Tracing.AspNetCore
{
    public class ServiceTracerProvider : IServiceTracerProvider
    {
        private readonly ITracer _tracer;
        private readonly TracingOptions _options;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ServiceTracerProvider(ITracer tracer, IHostingEnvironment hostingEnvironment, IOptions<TracingOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
            _hostingEnvironment = hostingEnvironment;
        }

        public IServiceTracer GetServiceTracer()
        {
            var service = _options.SystemName ?? _hostingEnvironment.ApplicationName;
            var environmentName = _hostingEnvironment.EnvironmentName;
            var host = Dns.GetHostName();
            var identity = string.IsNullOrEmpty(_options.ServiceIdentity) ? $"{service}@{host}" : _options.ServiceIdentity;
            return new ServiceTracer(_tracer, service, environmentName, identity, host);
        }
    }
}
