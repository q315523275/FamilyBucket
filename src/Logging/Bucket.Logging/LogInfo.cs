using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.Logging
{
    public class LogInfo
    {
        public string Id { get; set; }
        public string ClassName { get; set; }
        public string ProjectName { get; set; }
        public string LogTag { get; set; }
        public string LogType { get; set; }
        public string LogMessage { get; set; }
        public string IP { get; set; }
        public DateTime AddTime { get; set; }
}
}
