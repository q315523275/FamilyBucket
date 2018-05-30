using System;
using System.Collections.Generic;
using MessagePack;

namespace Bucket.Tracing.DataContract
{

    [MessagePackObject]
    public class Span
    {
        [Key(0)]
        public string SpanId { get; set; }

        [Key(1)]
        public string TraceId { get; set; }

        [Key(2)]
        public bool Sampled { get; set; }

        [Key(3)]
        public string OperationName { get; set; }

        /// <summary>
        /// duration(microsecond)
        /// </summary>
        [Key(4)]
        public long Duration { get; set; }

        [Key(5)]
        public DateTimeOffset StartTimestamp { get;  set;}

        [Key(6)]
        public DateTimeOffset FinishTimestamp { get;  set;}

        [Key(7)]
        public ICollection<Log> Logs { get; set; } = new List<Log>();

        [Key(8)]
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        [Key(9)]
        public ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();

        [Key(10)]
        public ICollection<SpanReference> References { get; set; } = new List<SpanReference>();
    }
}