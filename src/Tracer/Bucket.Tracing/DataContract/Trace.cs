using System.Collections.Generic;
namespace Bucket.Tracing.DataContract
{
    public class Trace
    {
        public string TraceId { get; set; }
        public List<Span> Spans { get; set; }
    }
}