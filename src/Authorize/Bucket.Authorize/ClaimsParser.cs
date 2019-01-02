using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Bucket.Authorize
{
    public class ClaimsParser: IClaimsParser
    {
        public List<string> GetValuesByClaimType(IEnumerable<Claim> claims, string claimType)
        {
            List<string> values = new List<string>();

            values.AddRange(claims.Where(x => x.Type == claimType).Select(x => x.Value).ToList());

            return values;
        }
    }
}
