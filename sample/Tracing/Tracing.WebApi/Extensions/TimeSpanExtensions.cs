using System;

namespace Tracing.Server
{
    public static class TimeSpanExtensions
    {
        public static long GetMicroseconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks / 10L;
        }
    }
}