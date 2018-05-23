using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracer
{
    public interface ITracingDiagnosticListener
    {
        string ListenerName { get; }
    }
}
