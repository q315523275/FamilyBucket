using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing.Diagnostics
{
    public interface ITracingDiagnosticListener
    {
        string ListenerName { get; }
    }
}
