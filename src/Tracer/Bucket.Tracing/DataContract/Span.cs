using System;
using System.Collections.Generic;
namespace Bucket.Tracing.DataContract
{
    public class Span
    {
        public string SpanId { get; set; }
        public string TraceId { get; set; }
        public bool Sampled { get; set; }
        public string OperationName { get; set; }

        /// <summary>
        /// duration(microsecond)
        /// </summary>
        public long Duration { get; set; }
        public DateTimeOffset StartTimestamp { get;  set;}
        public DateTimeOffset FinishTimestamp { get;  set;}
        public ICollection<Log> Logs { get; set; } = new List<Log>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();
        public ICollection<SpanReference> References { get; set; } = new List<SpanReference>();
    }
}