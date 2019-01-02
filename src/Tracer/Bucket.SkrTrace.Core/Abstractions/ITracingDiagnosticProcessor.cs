using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITracingDiagnosticProcessor
    {
        string ListenerName { get; }
    }
}
