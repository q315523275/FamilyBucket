using Bucket.ErrorCode.Model;
using System.Collections.Generic;

namespace Bucket.ErrorCode
{
    public interface ILocalDataRepository
    {
        bool Set(IList<ApiErrorCodeInfo> list);
        IList<ApiErrorCodeInfo> Get();
    }
}
