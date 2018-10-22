using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Identity.Dto.Wx
{
    public class QueryOpenIdOutput
    {
        public string OpenId { set; get; }
        public string SessionKey { set; get; }
        public string UnionId { set; get; }
    }
}
