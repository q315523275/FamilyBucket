using System.Collections.ObjectModel;

namespace Bucket.OpenTracing
{
    public class SpanReferenceCollection : Collection<SpanReference>
    {
        public static readonly SpanReferenceCollection Empty = new SpanReferenceCollection();
    }
}