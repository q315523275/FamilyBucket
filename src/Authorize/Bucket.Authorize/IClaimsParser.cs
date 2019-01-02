using System.Collections.Generic;
using System.Security.Claims;

namespace Bucket.Authorize
{
    public interface IClaimsParser
    {
        List<string> GetValuesByClaimType(IEnumerable<Claim> claims, string claimType);
    }
}
