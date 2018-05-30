using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Tracing.Extensions
{
    public static class TimeSpanExtensions
    {
        public static long GetMicroseconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / 10L;
        }
    }
}
