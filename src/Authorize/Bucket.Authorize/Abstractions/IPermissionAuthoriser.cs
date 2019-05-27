using Microsoft.AspNetCore.Http;

namespace Bucket.Authorize.Abstractions
{
    public interface IPermissionAuthoriser
    {
        bool Authorise(HttpContext httpContext);
    }
}
