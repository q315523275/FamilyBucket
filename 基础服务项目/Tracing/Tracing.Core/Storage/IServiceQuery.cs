using System.Collections.Generic;
using System.Threading.Tasks;
using Tracing.DataContract.Tracing;
using Tracing.Storage.Query;

namespace Tracing.Storage
{
    public interface IServiceQuery
    {
        Task<IEnumerable<Service>> GetServices(TimeRangeQuery query);
    }
}