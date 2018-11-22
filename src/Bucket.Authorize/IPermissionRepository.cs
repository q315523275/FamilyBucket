using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Authorize
{
    public interface IPermissionRepository
    {
        List<Permission> Permissions { get; }
        Task Get();
    }
}
