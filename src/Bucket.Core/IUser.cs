using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Bucket.Core
{
    public interface IUser
    {
        string Id { get; }
        string Name { get; }
        IEnumerable<Claim> Claims { get; }
    }
}
