using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Repository
{
    public class CacheKeys
    {
        public const string SmsCodeVerifyErr = "SmsCode:VerifyErrCount:{0}";
        public const string SmsCodeSendIdentity = "SmsCode:SendIdentity:{0}";
        public const string SmsCodeLoginCode = "SmsCode:LoginCode:{0}";
    }
}
