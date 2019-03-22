using System;

namespace Bucket.Logging
{
    public class LogMessageEntry
    {
        public string ProjectName { get; set; }
        public string ClassName { get; set; }
        public string LogTag { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
        public string LocalIp { get; set; }
        public DateTime AddTime { get; set; }
        public string TraceHead { get; set; }
    }
}
