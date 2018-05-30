
using System;
using System.Collections.Generic;

namespace Bucket.Tracing.Diagnostics
{
    internal class TracingDiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly TracingDiagnosticMethodCollection _methodCollection;

        public TracingDiagnosticObserver(ITracingDiagnosticListener tracingdiagnosticListener)
        {
            _methodCollection = new TracingDiagnosticMethodCollection(tracingdiagnosticListener);
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