using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.ErrorCode.Abstractions
{
    public interface IDataRepository
    {
        IList<ApiErrorCodeInfo> Data { get; }
        Task Get();
    }
}
