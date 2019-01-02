using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Bucket.Authorize
{
    public class ScopesAuthoriser : IPermissionAuthoriser
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IClaimsParser _claimsParser;
        private readonly string _scope = "scope";
        private bool _loaded = false;
        public ScopesAuthoriser(IPermissionRepository permissionRepository, IClaimsParser claimsParser)
        {
            _permissionRepository = permissionRepository;
            _claimsParser = claimsParser;
        }

        public bool Authorise(HttpContext httpContext)
        {
            if (!_loaded && _permissionRepository.Permissions.Count == 0)
                _permissionRepository.Get();
            _loaded = true;

            var permission = _permissionRepository.Permissions
                .FirstOrDefault(it => string.Equals(it.Path, httpContext.Request.Path, StringComparison.CurrentCultureIgnoreCase) && it.Method == httpContext.Request.Method);

            if (permission == null)
                return true;

            var values = _claimsParser.GetValuesByClaimType(httpContext.User.Claims, _scope);

            var matchesScopes = permission.Scope.Intersect(values).ToList();

            if (matchesScopes.Count == 0)
                return false;

            return true;
        }
    }
}
