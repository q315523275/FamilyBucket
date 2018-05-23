using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public interface IServiceTracer
    {
        ITraceSender TraceSender { get; }
        TraceSpan Start();
        void AsChildren(TraceSpan span);
    }
}
