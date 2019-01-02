using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.OpenTracing
{
    public class LogField : Dictionary<string, string>
    {
        public LogField() : base()
        {
        }

        public LogField(IDictionary<string, string> collection)
            : base(collection)
        {
        }

        public static LogField CreateNew()
        {
            return new LogField();
        }
    }
}
