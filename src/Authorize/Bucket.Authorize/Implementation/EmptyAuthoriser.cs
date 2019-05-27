using Bucket.Authorize.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Bucket.Authorize.Implementation
{
    public class EmptyAuthoriser : IPermissionAuthoriser
    {
        public bool Authorise(HttpContext httpContext)
        {
            return true;
        }
    }
}
