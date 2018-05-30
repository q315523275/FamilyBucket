using System.Collections.Generic;

namespace Bucket.OpenTracing
{
    public class LogField : Dictionary<string, object>
    {
        public LogField() : base()
        {
        }

        public LogField(IDictionary<string, object> collection)
            : base(collection)
        {
        }

        public static LogField CreateNew()
        {
            return new LogField();
        }
    }
}