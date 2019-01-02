using Bucket.OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing
{
    public class TraceIdGenerator : ITraceIdGenerator
    {
        public string Next()
        {
            return RandomUtils.NextLong().ToString();
        }
    }
}
