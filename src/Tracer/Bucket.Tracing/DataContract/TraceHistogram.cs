using System;

namespace Bucket.Tracing.DataContract
{
    public class TraceHistogram
    {
        public DateTimeOffset Time { get; set; }

        public int Count { get; set; }
    }
}