using Bucket.SkrTrace.Core;
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Diagnostics;
using Bucket.SkrTrace.Core.Implementation;
using Bucket.SkrTrace.Core.Implementation.Carrier;
using Bucket.SkrTrace.Core.Service;
using Bucket.SkrTrace.Core.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bucket.SkrTrace.DependencyInjection
{
    public class SkrTraceBuilder: ISkrTraceBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }

        public SkrTraceBuilder(IServiceCollection services, IConfiguration configurationRoot)
        {
            Configuration = configurationRoot;
            Services = services;
            services.Configure<SkrTraceOptions>(configurationRoot.GetSection("SkrTrace"));
            services.AddSingleton<IContextCarrierFactory, ContextCarrierFactory>();
            services.AddSingleton<ITraceDispatcher, AsyncQueueTraceDispatcher>();
            services.AddSingleton<TracingDiagnosticProcessorObserver>();
            services.AddSingleton(RuntimeEnvironment.Instance);
            services.AddSingleton<IExecutionService, TraceSegmentTransportService>();
            services.AddSingleton<ISkrTraceAgentStartup, SkrTraceAgentStartup>();
            services.AddSingleton<IHostedService, InstrumentationHostedService>();
            services.AddSingleton<ISkrTraceCollect, EmtpySkrTraceCollect>();
        }
    }
}
