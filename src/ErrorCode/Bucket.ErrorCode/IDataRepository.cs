using Bucket.ErrorCode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.ErrorCode
{
    public interface IDataRepository
    {
        IList<ApiErrorCodeInfo> Data { get; }
        Task Get();
    }
}
