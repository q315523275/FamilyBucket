
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bucket.Tracing.Diagnostics
{
    public class TracingDiagnosticListenerObserver : IObserver<DiagnosticListener>
    {

        private readonly IEnumerable<ITracingDiagnosticListener> _tracingdiagnosticListeners;

        public TracingDiagnosticListenerObserver(IEnumerable<ITracingDiagnosticListener> tracingdiagnosticListeners)
        {
            _tracingdiagnosticListeners = tracingdiagnosticListeners ??
                                           throw new ArgumentNullException(nameof(tracingdiagnosticListeners));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(DiagnosticListener listener)
        {
            foreach (var diagnosticListener in _tracingdiagnosticListeners.Distinct(x => x.ListenerName))
            {
                if (listener.Name == diagnosticListener.ListenerName)
                {
                    Subscribe(listener, diagnosticListener);
                }
            }
        }

        protected virtual void Subscribe(DiagnosticListener listener,
            ITracingDiagnosticListener diagnosticListener)
        {
            listener.Subscribe(new TracingDiagnosticObserver(diagnosticListener));
        }
    }
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, K>(this IEnumerable<T> source, Func<T, K> predicate)
        {
            HashSet<K> sets = new HashSet<K>();
            foreach (var item in source)
            {
                if (sets.Add(predicate(item)))
                {
                    yield return item;
                }
            }
        }
    }
}