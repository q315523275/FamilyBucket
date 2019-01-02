
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Bucket.SkrTrace.Core.Diagnostics
{
    public class TracingDiagnosticProcessorObserver : IObserver<DiagnosticListener>
    {
        private readonly ILogger<TracingDiagnosticProcessorObserver> _logger;
        private readonly IEnumerable<ITracingDiagnosticProcessor> _tracingDiagnosticProcessors;

        public TracingDiagnosticProcessorObserver(IEnumerable<ITracingDiagnosticProcessor> tracingDiagnosticProcessors, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TracingDiagnosticProcessorObserver>();
            _tracingDiagnosticProcessors = tracingDiagnosticProcessors ??
                                           throw new ArgumentNullException(nameof(tracingDiagnosticProcessors));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener listener)
        {
            foreach (var diagnosticProcessor in _tracingDiagnosticProcessors.Distinct(x => x.ListenerName))
            {
                if (listener.Name == diagnosticProcessor.ListenerName)
                {
                    Subscribe(listener, diagnosticProcessor);
                    _logger.LogInformation(
                        $"Loaded diagnostic listener [{diagnosticProcessor.ListenerName}].");
                }
            }
        }

        protected virtual void Subscribe(DiagnosticListener listener,
            ITracingDiagnosticProcessor tracingDiagnosticProcessor)
        {
            listener.Subscribe(new TracingDiagnosticObserver(tracingDiagnosticProcessor));
        }
    }
}