using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    public class QueryAppProjectListInput : BasePageInput
    {
        public string AppId { set; get; }
        public int IsPublic { set; get; } = -1;
    }
}
