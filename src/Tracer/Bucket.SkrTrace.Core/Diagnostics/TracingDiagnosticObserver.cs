
using Bucket.SkrTrace.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.Diagnostics
{
    internal class TracingDiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly TracingDiagnosticMethodCollection _methodCollection;

        public TracingDiagnosticObserver(ITracingDiagnosticProcessor tracingDiagnosticProcessor)
        {
            _methodCollection = new TracingDiagnosticMethodCollection(tracingDiagnosticProcessor);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            foreach (var method in _methodCollection)
            {
                method.Invoke(value.Key, value.Value);
            }
        }
    }
}