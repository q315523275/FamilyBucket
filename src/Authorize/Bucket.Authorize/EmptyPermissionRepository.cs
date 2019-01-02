using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bucket.Authorize
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
