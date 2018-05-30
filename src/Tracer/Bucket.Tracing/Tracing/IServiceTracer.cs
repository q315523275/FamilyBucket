using AspectCore.DynamicProxy;
using Bucket.OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing
{
    [NonAspect]
    public interface IServiceTracer
    {
        ITracer Tracer { get; }

        string ServiceName { get; }

        string Environment { get; }

        string Identity { get; }

        ISpan Start(ISpanBuilder spanBuilder);
    }
}
