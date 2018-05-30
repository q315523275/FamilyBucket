using System.Linq;
using BaggageContract = Bucket.Tracing.DataContract.Baggage;
using LogFieldContract = Bucket.Tracing.DataContract.LogField;
using SpanReferenceContract = Bucket.Tracing.DataContract.SpanReference;
using SpanContract = Bucket.Tracing.DataContract.Span;
using LogContract = Bucket.Tracing.DataContract.Log;
using TagContract = Bucket.Tracing.DataContract.Tag;
using Bucket.OpenTracing;
using Bucket.Tracing.Extensions;

namespace Bucket.Tracing
{
    public static class SpanContractUtils
    {
        public static SpanContract CreateFromSpan(ISpan span)
        {
            var spanContract = new SpanContract
            {
                FinishTimestamp = span.FinishTimestamp,
                StartTimestamp = span.StartTimestamp,
                Sampled = span.SpanContext.Sampled,
                SpanId = span.SpanContext.SpanId,
                TraceId = span.SpanContext.TraceId,
                OperationName = span.OperationName,
                Duration = (span.FinishTimestamp - span.StartTimestamp).GetMicroseconds()
            };

            spanContract.Baggages = span.SpanContext.Baggage?.Select(x => new BaggageContract { Key = x.Key, Value = x.Value }).ToList();
            spanContract.Logs = span.Logs?.Select(x =>
                new LogContract
                {
                    Timestamp = x.Timestamp,
                    Fields = x.Fields.Select(f => new LogFieldContract { Key = f.Key, Value = f.Value?.ToString() }).ToList()
                }).ToList();

            spanContract.Tags = span.Tags?.Select(x => new TagContract { Key = x.Key, Value = x.Value }).ToList();

            spanContract.References = span.SpanContext.References?.Select(x =>
                new SpanReferenceContract { ParentId = x.SpanContext.SpanId, Reference = x.SpanReferenceOptions.ToString() }).ToList();

            return spanContract;
        }
    }
}