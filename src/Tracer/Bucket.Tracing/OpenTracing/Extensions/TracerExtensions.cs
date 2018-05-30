using System;
using System.Collections;
using System.Collections.Generic;

namespace Bucket.OpenTracing
{
    public static class TracerExtensions
    {
        public static ISpan GetEntrySpan(this ITracer tracer)
        {
            if (SpanLocal.Current.TryGetValue("Entry", out ISpan value))
                return value;
            return null;
        }

        public static void SetEntrySpan(this ITracer tracer, ISpan spanContext)
        {
            SpanLocal.Current["Entry"] = spanContext;
        }

        public static ISpan GetExitSpan(this ITracer tracer)
        {
            if (SpanLocal.Current.TryGetValue("Exit", out ISpan value))
                return value;
            return null;
        }

        public static void SetExitSpan(this ITracer tracer, ISpan spanContext)
        {
            SpanLocal.Current["Exit"] = spanContext;
        }

        public static void Inject<T>(this ITracer tracer, ISpanContext spanContext, T carrier, Action<T, string, string> injector)
            where T : class, IEnumerable
        {
            if (tracer == null)
            {
                throw new ArgumentNullException(nameof(tracer));
            }

            tracer.Inject(spanContext, new TextMapCarrierWriter(), new DelegatingCarrier<T>(carrier, injector));
        }

        public static bool TryExtract<T>(this ITracer tracer, out ISpanContext spanContext, T carrier, Func<T, string, string> extractor, Func<T, IEnumerator<KeyValuePair<string, string>>> enumerator = null)
            where T : class, IEnumerable
        {
            spanContext = tracer.Extract(new TextMapCarrierReader(new SpanContextFactory()), new DelegatingCarrier<T>(carrier, extractor, enumerator));
            return spanContext != null;
        }
    }
}