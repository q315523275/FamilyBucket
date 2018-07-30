using System;
using System.Collections.Generic;

namespace Tracing.Server.ViewModels
{
    public class SpanViewModel
    {
        public string SpanId { get; set; }

        public string TraceId { get; set; }

        public bool Sampled { get; set; }

        public string OperationName { get; set; }

        /// <summary>
        /// duration(microsecond)
        /// </summary>
        public long Duration { get; set; }

        public DateTime StartTimestamp { get; set; }

        public DateTime FinishTimestamp { get; set; }

        public IEnumerable<SpanViewModel> Children { get; set; }
        
        public string ServiceName { get; set; }
        
        public long Offset { get; set; }
    }
}