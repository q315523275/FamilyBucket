using System.Collections.Generic;

namespace Bucket.ErrorCode.Abstractions
{
    public interface ILocalDataRepository
    {
        bool Set(IList<ApiErrorCodeInfo> list);
        IList<ApiErrorCodeInfo> Get();
    }
}
