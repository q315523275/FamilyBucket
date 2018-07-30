using System;
using System.Collections.Generic;

namespace Tracing.Server.ViewModels
{
    public class TraceDetailViewModel
    {
        public string TraceId { get; set; }

        public long Duration { get; set; }

        public DateTime StartTimestamp { get; set; }

        public DateTime FinishTimestamp { get; set; }

        public IEnumerable<SpanViewModel> Spans { get; set; }
    }
}