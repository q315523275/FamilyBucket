using System;

namespace Tracing.Server.Common
{
    public static class TimestampHelpers
    {
        public static DateTimeOffset? Convert(long? timestamp)
        {
            if (!timestamp.HasValue)
            {
                return null;
            }
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp.Value);
        }
    }
}
