
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Bucket.Tracing.Diagnostics
{
    internal class TracingDiagnosticMethodCollection : IEnumerable<TracingDiagnosticMethod>
    {
        private readonly List<TracingDiagnosticMethod> _methods;

        public TracingDiagnosticMethodCollection(ITracingDiagnosticListener diagnosticListener)
        {
            _methods = new List<TracingDiagnosticMethod>();
            foreach (var method in diagnosticListener.GetType().GetMethods())
            {
                var diagnosticName = method.GetCustomAttribute<DiagnosticName>();
                if(diagnosticName==null)
                    continue;
                _methods.Add(new TracingDiagnosticMethod(diagnosticListener, method, diagnosticName.Name));
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