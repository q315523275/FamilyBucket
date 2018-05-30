using System.Collections.Generic;

namespace Bucket.OpenTracing
{
    public interface ISpanContextFactory
    {
        ISpanContext Create(SpanContextPackage spanContextPackage);
    }
}