using Microsoft.AspNetCore.Http;

namespace Bucket.Authorize
{
    public class EmptyAuthoriser : IPermissionAuthoriser
    {
        public bool Authorise(HttpContext httpContext)
        {
            return true;
        }
    }
}
