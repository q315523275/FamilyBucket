using Bucket.SkrTrace.Core.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bucket.SkrTrace.Core.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Bucket.SkrTrace.Core
{
    public class SkrTraceAgentStartup : ISkrTraceAgentStartup
    {
        private readonly TracingDiagnosticProcessorObserver _observer;
        private readonly IEnumerable<IExecutionService> _services;
        private readonly ILogger _logger;
        private readonly IRuntimeEnvironment RuntimeEnvironment;
        private readonly SkrTraceOptions _skrTraceOptions;

        public SkrTraceAgentStartup(TracingDiagnosticProcessorObserver observer, 
            IEnumerable<IExecutionService> services, 
            ILogger<SkrTraceAgentStartup> logger, 
            IRuntimeEnvironment runtimeEnvironment, IOptions<SkrTraceOptions> skrTraceOptions)
        {
            _observer = observer;
            _services = services;
            _logger = logger;
            RuntimeEnvironment = runtimeEnvironment;
            _skrTraceOptions = skrTraceOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine(Welcome());
            RuntimeEnvironment.ApplicationCode = _skrTraceOptions.ApplicationCode;
            foreach (var service in _services)
                await service.StartAsync(cancellationToken);
            DiagnosticListener.AllListeners.Subscribe(_observer);
            _logger.LogInformation("Started SkrTrace .NET Core Agent.");
        }
        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var service in _services)
                await service.StopAsync(cancellationToken);
            _logger.LogInformation("Stopped SkrTrace .NET Core Agent.");
        }


        private string Welcome()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Initializing ...");
            builder.AppendLine();
            builder.AppendLine("***************************************************************");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("*                    Welcome To SkrTrace                      *");
            builder.AppendLine("*                                                             *");
            builder.AppendLine("***************************************************************");
            return builder.ToString();
        }
    }
}
