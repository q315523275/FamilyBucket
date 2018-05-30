using MessagePack;

namespace Bucket.Tracing.DataContract
{
    [MessagePackObject]
    public class SpanReference
    {
        [Key(0)]
        public string Reference { get; set; }

        [Key(1)]
        public string ParentId { get; set; }
    }
}