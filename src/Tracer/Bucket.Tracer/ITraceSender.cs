using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Tracer
{
    public interface ITraceSender
    {
        Task SendAsync(TraceSpan traceSpan);
    }
}