using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Dto.Auth
{
    public class UserTokenDto
    {
        public long Id { set; get; }
        public string RealName { set; get; }
        public string Mobile { set; get; }
        public string Email { set; get; }
    }
}
