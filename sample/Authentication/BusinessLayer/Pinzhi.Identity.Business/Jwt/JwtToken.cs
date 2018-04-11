using Pinzhi.Identity.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Pinzhi.Identity.Business.Jwt
{
    public static class JwtToken
    {
        /// <summary>
        /// 获取基于JWT的Token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Dictionary<string, object> BuildJwtToken(IEnumerable<Claim> claims, PermissionRequirement permissionRequirement)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: permissionRequirement.Issuer,
                audience: permissionRequirement.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(permissionRequirement.Expiration),
                signingCredentials: permissionRequirement.SigningCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new Dictionary<string, object>
            {
                { "AccessToken", encodedJwt },
                { "ExpiresIn", permissionRequirement.Expiration.TotalMilliseconds },
                { "TokenType", "Bearer" }
            };
            return responseJson;
        }
    }
}
