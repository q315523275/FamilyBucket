using System;
using System.Collections.Generic;
using System.Text;

namespace Tracing.Storage.Query
{
    public class TimeRangeQuery
    {
        public DateTimeOffset? StartTimestamp { get; set; }

        public DateTimeOffset? FinishTimestamp { get; set; }

        public virtual void Ensure()
        {
            if (FinishTimestamp == null)
            {
                FinishTimestamp = DateTimeOffset.Now;
            }
            if (StartTimestamp == null)
            {
                StartTimestamp = FinishTimestamp.Value.AddHours(-1);
            }
        }
    }
}
