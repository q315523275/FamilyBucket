using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.Core.Transport;
using Newtonsoft.Json;
namespace Bucket.SkrTrace.Core.Implementation
{
    public class EmtpySkrTraceCollect : ISkrTraceCollect
    {
        public Task CollectAsync(IEnumerable<TraceSegmentRequest> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));
            return Task.CompletedTask;
        }
    }
}
