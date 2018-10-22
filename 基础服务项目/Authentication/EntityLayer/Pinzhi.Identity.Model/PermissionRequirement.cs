using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Model
{
    public class PermissionRequirement
    {
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }
        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">订阅人</param>
        /// <param name="signingCredentials">签名验证实体</param>
        /// <param name="expiration">声明类型</param>
        public PermissionRequirement(string issuer, string audience, SigningCredentials signingCredentials, TimeSpan expiration)
        {
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            SigningCredentials = signingCredentials;
        }
    }
}
