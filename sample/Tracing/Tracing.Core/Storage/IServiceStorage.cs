using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;

namespace Tracing.Storage
{
    public interface IServiceStorage
    {
        Task StoreServiceAsync(IEnumerable<Service> services, CancellationToken cancellationToken);
    }
}
