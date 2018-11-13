using System;
using System.Security.Claims;

namespace Bucket.AspNetCore.Authorize
{
    /// <summary>
    /// TokenBuilder interface
    /// </summary>
    public interface ITokenBuilder
    {
        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <returns></returns>
        Token BuildJwtToken(Claim[] claims, DateTime? expires = null);
        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        Token BuildJwtToken(Claim[] claims, DateTime notBefore, DateTime? expires = null);

        /// <summary>
        /// get the token of jwt
        /// </summary>
        /// <param name="claims">claim array</param>
        /// <param name="ip">ip</param>
        /// <param name="notBefore">not Before time</param>
        /// <param name="expires">expires</param>
        /// <returns></returns>
        Token BuildJwtToken(Claim[] claims, string ip, DateTime? notBefore = null, DateTime? expires = null);
    }
}
