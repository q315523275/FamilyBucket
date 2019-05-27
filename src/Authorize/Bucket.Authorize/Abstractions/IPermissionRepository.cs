using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Authorize.Abstractions
{
    public interface IPermissionRepository
    {
        List<Permission> Permissions { get; }
        Task Get();
    }
}
