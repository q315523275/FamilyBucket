using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.AspNetCore.Authentication
{
    public class AuthenticationOptions
    {
        public string Secret { set; get; }
        public string Issuer { set; get; }
        public string Audience { set; get; }
    }
}
