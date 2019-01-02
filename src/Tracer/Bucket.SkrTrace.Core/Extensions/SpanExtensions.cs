using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.OpenTracing;
using System;

namespace Bucket.SkrTrace.Core.Extensions
{
    public static class SpanExtensions
    {
        public static ISpan Log(this ISpan span, DateTime timestamp, LogField fields)
        {
            if (span == null)
            {
                throw new ArgumentNullException(nameof(span));
            }
            span.Logs.Add(new LogData(timestamp, fields));
            return span;
        }

        public static ISpan Log(this ISpan span, LogField fields)
        {
            return Log(span, DateTime.Now, fields);
        }

        public static ISpan Exception(this ISpan span, Exception exception)
        {
            if (span == null)
            {
                return span;
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            span.ErrorOccurred();
            span.Log(LogField.CreateNew().EventError().ErrorKind(exception).Message(exception.Message).Stack(exception.StackTrace));
            return span;
        }
    }
}
