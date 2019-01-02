using Bucket.SkrTrace.Core.Abstractions;
using System.Threading;

namespace Bucket.SkrTrace.Core.Implementation
{
    public class ContextManager: ITracingContextListener, IIgnoreTracerContextListener
    {
        static ContextManager()
        {
            var manager = new ContextManager();
            TracingContext.ListenerManager.Add(manager);
        }
        private static readonly AsyncLocal<ITracerContext> _context = new AsyncLocal<ITracerContext>();
        private static ITracerContext GetOrCreateContext()
        {
            var context = _context.Value;
            if (context == null)
            {
                _context.Value = new TracingContext();
            }
            return _context.Value;
        }
        private static ITracerContext Context => _context.Value;
        public static ISpan CreateEntrySpan(string operationName, IContextCarrier carrier)
        {
            if (carrier != null && carrier.IsValid)
            {
                var context = GetOrCreateContext();
                var span = context.CreateEntrySpan(operationName);
                context.Extract(carrier);
                return span;
            }
            else
            {
                var context = GetOrCreateContext();
                return context.CreateEntrySpan(operationName);
            }
        }
        public static ISpan CreateExitSpan(string operationName, IContextCarrier carrier, string remotePeer)
        {
            var context = GetOrCreateContext();
            var span = context.CreateExitSpan(operationName, remotePeer);
            context.Inject(carrier);
            return span;
        }
        public static ISpan CreateExitSpan(string operationName, string remotePeer)
        {
            var context = GetOrCreateContext();
            var span = context.CreateExitSpan(operationName, remotePeer);
            return span;
        }
        public static void Inject(IContextCarrier carrier)
        {
            Context?.Inject(carrier);
        }
        public static void Extract(IContextCarrier carrier)
        {
            Context?.Extract(carrier);
        }
        public static void StopSpan()
        {
            StopSpan(ActiveSpan);
        }
        public static void SetIdentity(string identity)
        {
            Context?.SetIdentity(identity);
        }
        public static ISpan ActiveSpan
        {
            get { return Context?.ActiveSpan; }
        }
        public static void StopSpan(ISpan span)
        {
            Context?.StopSpan(span);
        }
        public void AfterFinished(ITraceSegment traceSegment)
        {
            _context.Value = null;
        }
        public void AfterFinish(ITracerContext tracerContext)
        {
            _context.Value = null;
        }
    }
}
