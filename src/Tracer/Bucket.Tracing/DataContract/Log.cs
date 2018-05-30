using System;
using System.Collections.Generic;
using MessagePack;

namespace Bucket.Tracing.DataContract
{
    [MessagePackObject]
    public class Log
    {
        [Key(0)]
        public DateTimeOffset Timestamp { get; set; }

        [Key(1)]
        public ICollection<LogField> Fields { get; set; } = new List<LogField>();
    }
}