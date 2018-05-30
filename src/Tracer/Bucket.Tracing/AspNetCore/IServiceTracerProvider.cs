using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing.AspNetCore
{
    public interface IServiceTracerProvider
    {
        IServiceTracer GetServiceTracer();
    }
}
