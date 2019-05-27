using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bucket.Authorize.Abstractions;

namespace Bucket.Authorize.Implementation
{
    /// <summary>
    /// TokenBuilder
    /// </summary>
    public class TokenBuilder : ITokenBuilder
    {
        /// <summary>
        /// JwtAuthorizationRequirement
        /// </summary>
        readonly JwtAuthorizationRequirement _jwtAuthorizationRequirement;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="jwtAuthorizationRequirement"></param>
        public TokenBuilder(JwtAuthorizationRequirement jwtAuthorizationRequirement)
        {
            _jwtAuthorizationRequirement = jwtAuthorizationRequirement;
        }
        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        public Token BuildJwtToken(IEnumerable<Claim> claims, DateTime? expires = null)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _jwtAuthorizationRequirement.Issuer,
                audience: _jwtAuthorizationRequirement.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _jwtAuthorizationRequirement.SigningCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new Token
            {
                TokenValue = encodedJwt,
                Expires = expires,
                TokenType = "Bearer"
            };
            return responseJson;
        }
        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        public Token BuildJwtToken(IEnumerable<Claim> claims, DateTime notBefore, DateTime? expires = null)
        {
            var claimList = new List<Claim>(claims);
            var jwt = new JwtSecurityToken(
                issuer: _jwtAuthorizationRequirement.Issuer,
                audience: _jwtAuthorizationRequirement.Audience,
                claims: claimList.ToArray(),
                notBefore: notBefore,
                expires: expires,
                signingCredentials: _jwtAuthorizationRequirement.SigningCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new Token
            {
                TokenValue = encodedJwt,
                Expires = expires,
                TokenType = "Bearer"
            };
            return responseJson;
        }

        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="ip">ip</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        public Token BuildJwtToken(IEnumerable<Claim> claims, string ip, DateTime? notBefore = null, DateTime? expires = null)
        {
            var claimList = claims.ToList();
            claimList.Add(new Claim("ip", ip));
            var now = notBefore.HasValue ? notBefore.Value : DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: _jwtAuthorizationRequirement.Issuer,
                audience: _jwtAuthorizationRequirement.Audience,
                claims: claimList,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: _jwtAuthorizationRequirement.SigningCredentials
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var responseJson = new Token
            {
                TokenValue = encodedJwt,
                Expires = expires,
                TokenType = "Bearer"
            };
            return responseJson;
        }
        
    }
}
