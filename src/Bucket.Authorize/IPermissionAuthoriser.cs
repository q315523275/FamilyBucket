using Microsoft.AspNetCore.Http;

namespace Bucket.Authorize
{
    public interface IPermissionAuthoriser
    {
        bool Authorise(HttpContext httpContext);
    }
}
