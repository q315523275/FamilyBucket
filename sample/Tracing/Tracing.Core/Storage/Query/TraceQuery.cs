using System;

namespace Tracing.Storage.Query
{
    public class TraceQuery : TimeRangeQuery
    {
        public string ServiceName { get; set; }

        public int? MinDuration { get; set; }

        public int? MaxDuration { get; set; }

        public int Limit { get; set; }

        public string Tags { get; set; }  
    }
}