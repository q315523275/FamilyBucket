using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Bucket.Authorize.Abstractions
{
    /// <summary>
    /// TokenBuilder interface
    /// </summary>
    public interface ITokenBuilder
    {
        /// <summary>
        /// 创建JwtToken
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <returns></returns>
        Token BuildJwtToken(IEnumerable<Claim> claims, DateTime? expires = null);
        /// <summary>
        /// 创建JwtToken
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        Token BuildJwtToken(IEnumerable<Claim> claims, DateTime notBefore, DateTime? expires = null);
        /// <summary>
        /// 创建JwtToken
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="ip">ip</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        Token BuildJwtToken(IEnumerable<Claim> claims, string ip, DateTime? notBefore = null, DateTime? expires = null);
    }
}
