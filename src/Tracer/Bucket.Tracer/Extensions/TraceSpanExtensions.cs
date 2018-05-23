using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public static class TraceSpanExtensions
    {
        public static TraceSpan SetSuccess(this TraceSpan traceSpan, bool isSuccess)
        {
            traceSpan.IsSuccess = isSuccess;
            return traceSpan;
        }

        public static TraceSpan SetOperationName(this TraceSpan traceSpan, string operationName)
        {
            traceSpan.OperationName = operationName;
            return traceSpan;
        }

        public static TraceSpan Finish(this TraceSpan traceSpan)
        {
            traceSpan.EndTime = DateTime.Now;
            traceSpan.TimeLength = Math.Round((traceSpan.EndTime - traceSpan.StartTime).TotalMilliseconds, 4);
            return traceSpan;
        }
        public static TraceSpan Exception(this TraceSpan span, Exception exception)
        {
            if (span == null)
            {
                return span;
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            span.IsSuccess = false;
            span.Tags.Error(exception.Message);

            return span;
        }
    }
}
