using Bucket.Authorize.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bucket.Authorize.Implementation
{
    public class EmptyPermissionRepository : IPermissionRepository
    {
        public List<Permission> Permissions => new List<Permission>();

        public Task Get()
        {
            return Task.CompletedTask;
        }
    }
}
