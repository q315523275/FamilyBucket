
using Bucket.SkrTrace.Core.Abstractions;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Bucket.SkrTrace.Core.Diagnostics
{
    internal class TracingDiagnosticMethodCollection : IEnumerable<TracingDiagnosticMethod>
    {
        private readonly List<TracingDiagnosticMethod> _methods;

        public TracingDiagnosticMethodCollection(ITracingDiagnosticProcessor tracingDiagnosticProcessor)
        {
            _methods = new List<TracingDiagnosticMethod>();
            foreach (var method in tracingDiagnosticProcessor.GetType().GetMethods())
            {
                var diagnosticName = method.GetCustomAttribute<DiagnosticName>();
                if(diagnosticName==null)
                    continue;
                _methods.Add(new TracingDiagnosticMethod(tracingDiagnosticProcessor, method, diagnosticName.Name));
            }
        }
        
        public IEnumerator<TracingDiagnosticMethod> GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _methods.GetEnumerator();
        }
    }
}