using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.OpenTracing
{
    public sealed class LogData
    {
        public DateTime Timestamp { get; }

        public LogField Fields { get; }

        public LogData()
            : this(DateTime.Now, null)
        {
        }

        public LogData(IDictionary<string, string> fields)
            : this(DateTime.Now, fields)
        {
        }

        public LogData(DateTime timestamp, IDictionary<string, string> fields)
        {
            Timestamp = timestamp;
            if (fields == null)
            {
                Fields = new LogField();
            }
            else if (fields is LogField logField)
            {
                Fields = logField;
            }
            else
            {
                Fields = new LogField(fields);
            }
        }
    }
}
