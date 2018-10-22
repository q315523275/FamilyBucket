using System;

namespace Tracing.DataContract.Tracing
{
    public class TraceHistogram
    {
        public DateTimeOffset Time { get; set; }

        public int Count { get; set; }
    }
}