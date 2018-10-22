using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Bucket.Core;

namespace Bucket.AspNetCore.Commons
{
    public class HttpContextUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id => _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "Uid").Value;
        public string Name => _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "Name").Value;
        public string MobilePhone => _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "MobilePhone").Value;
        public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext.User.Claims;
    }
}
