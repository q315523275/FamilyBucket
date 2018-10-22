using Bucket.Tracing.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pinzhi.Tracing.EventSubscribe.Elasticsearch
{
    public interface IServiceStorage
    {
        Task StoreServiceAsync(IEnumerable<Service> services, CancellationToken cancellationToken = default(CancellationToken));
    }
}
