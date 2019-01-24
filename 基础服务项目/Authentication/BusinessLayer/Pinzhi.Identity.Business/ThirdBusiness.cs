using Bucket.Core;
using Pinzhi.Identity.Dto.Third;
using Pinzhi.Identity.Interface;
using Pinzhi.Identity.Model;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Bucket.DbContext;

namespace Pinzhi.Identity.Business
{
    public class ThirdBusiness : IThirdBusiness
    {
        private readonly IDbRepository<ThirdOAuthInfo> _oauthDbRepository;
        private readonly IUser _user;
        private readonly IWxRepository _wxRepository;
        private readonly IAuthRepository _authRepository;

        public ThirdBusiness(IDbRepository<ThirdOAuthInfo> oauthDbRepository, IUser user, IWxRepository wxRepository, IAuthRepository authRepository)
        {
            _oauthDbRepository = oauthDbRepository;
            _user = user;
            _wxRepository = wxRepository;
            _authRepository = authRepository;
        }



        /// <summary>
        /// 微信小程序Code验证
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WxMiniLoginOutput> WxMiniCodeVerify(WxMiniLoginInput input)
        {
            var openInfo = await _wxRepository.QueryOpenIdAsync(input.Code, input.AppId);
            return new WxMiniLoginOutput
            {
                Data = new
                {
                    openInfo.OpenId
                }
            };
        }
        /// <summary>
        /// 微信小程序登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WxMiniLoginOutput> WxMiniLogin(WxMiniLoginInput input)
        {
            var openInfo = await _wxRepository.QueryOpenIdAsync(input.Code, input.AppId);
            var authInfo = await _oauthDbRepository.GetFirstAsync(it => it.OpenId == openInfo.OpenId && it.AuthServer == "WxMini");
            if (authInfo == null)
                return new WxMiniLoginOutput { Data = null };
            var uid = authInfo.Uid;
            var token = _authRepository.CreateAccessToken(new Dto.Auth.UserTokenDto
            {
                Email = string.Empty,
                Id = uid,
                Mobile = "test",
                RealName = "test"
            }, new List<string>());
            return new WxMiniLoginOutput
            {
                Data = new
                {
                    AccessToken = $"Bearer {token}",
                    Expire = DateTimeOffset.Now.AddHours(4).ToUnixTimeSeconds(),
                    RealName = "test",
                    Mobile = "test",
                    Id = uid
                }
            };
        }
    }
}
