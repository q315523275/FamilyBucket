using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using Pinzhi.Identity.Business.Jwt;
using Pinzhi.Identity.Interface;
using Pinzhi.Identity.Model;
using Pinzhi.Identity.DTO.Auth;

using SqlSugar;
using Bucket.Exceptions;
using Bucket.Utility;
using Bucket.Utility.Helpers;
namespace Pinzhi.Identity.Business.Auth
{
    public class AuthBusiness: IAuthBusiness
    {
        /// <summary>
        /// 自定义策略参数
        /// </summary>
        private readonly PermissionRequirement _requirement;
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<AuthBusiness> _logger;
        public AuthBusiness(PermissionRequirement requirement, SqlSugarClient dbContext, ILogger<AuthBusiness> logger)
        {
            _requirement = requirement;
            _dbContext = dbContext;
            _logger = logger;
        }
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<LoginOutput> LoginAsync(LoginInput input)
        {
            // 用户验证
            var userInfo = await _dbContext.Queryable<UserInfo>().Where(it => it.UserName == input.UserName).FirstAsync();
            if (userInfo == null)
                throw new BucketException("GO_0004007", "账号不存在");
            if (userInfo.State != 1)
                throw new BucketException("GO_0004008", "账号状态异常");
            if (userInfo.Password != Encrypt.SHA256(input.Password + userInfo.Salt))
                throw new BucketException("GO_4009", "账号或密码错误");

            // 用户角色
            var roleList = await _dbContext.Queryable<RoleInfo, UserRoleInfo>((role, urole) => new object[] { JoinType.Inner, role.Id == urole.RoleId })
                 .Where((role, urole) => urole.Uid == userInfo.Id)
                 .Where((role, urole) => role.IsDel == false)
                 .Select((role, urole) => new { Id = role.Id, Key = role.Key })
                 .ToListAsync();
            // 用户基本信息
            var claims = new List<Claim> {
                new Claim("Uid", userInfo.Id.ToString()),
                new Claim("Name", userInfo.RealName.SafeString()),
                new Claim("MobilePhone", userInfo.Mobile.SafeString()),
                new Claim("Email", userInfo.Email.SafeString())
            };
            // 角色数据
            foreach (var info in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, info.Key));
                claims.Add(new Claim("scope", info.Key));
            }
            // 用户身份标识
            var identity = new ClaimsIdentity();
            identity.AddClaims(claims);
            // accessToken
            var token = JwtToken.BuildJwtToken(claims, _requirement);
            token.Add("RealName", userInfo.RealName.SafeString());
            token.Add("Mobile", userInfo.Mobile.SafeString());
            token.Add("Id", userInfo.Id);
            return new LoginOutput { Data = token };
        }
    }
}
