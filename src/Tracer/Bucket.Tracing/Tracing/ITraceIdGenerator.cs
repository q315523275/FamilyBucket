using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing
{
    public interface ITraceIdGenerator
    {
        string Next();
    }
}
