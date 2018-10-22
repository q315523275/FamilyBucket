using System;
using System.Collections.Generic;

namespace Tracing.Server.ViewModels
{
    public class SpanDetailViewModel
    {
        public string SpanId { get; set; }

        public string TraceId { get; set; }

        public bool Sampled { get; set; }

        public string OperationName { get; set; }

        /// <summary>
        /// duration(microsecond)
        /// </summary>
        public long Duration { get; set; }

        public DateTime StartTimestamp { get;  set;}

        public DateTime FinishTimestamp { get;  set;}
        
        public string ServiceName { get; set; }

        public ICollection<LogViewModel> Logs { get; set; }

        public ICollection<TagViewModel> Tags { get; set; }

        public ICollection<ReferenceViewModel> References { get; set; }
    }

    public class LogViewModel
    {
        
        public DateTimeOffset Timestamp { get; set; }

        public ICollection<LogFieldViewModel> Fields { get; set; }
    }

    public class LogFieldViewModel
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class TagViewModel
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class ReferenceViewModel
    {
        public string Reference { get; set; }

        public string ParentId { get; set; }
    }
}