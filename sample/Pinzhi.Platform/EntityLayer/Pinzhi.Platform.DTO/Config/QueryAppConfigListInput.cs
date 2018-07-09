using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class QueryAppConfigListInput: BasePageInput
    {
        public string AppId { set; get; }
        public string NameSpace { set; get; }
    }
}
