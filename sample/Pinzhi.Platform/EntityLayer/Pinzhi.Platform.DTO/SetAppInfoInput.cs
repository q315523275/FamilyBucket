using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetAppInfoInput
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string AppId { set; get; }
        public string Secret { set; get; }
        public string Remark { set; get; }
    }
}
