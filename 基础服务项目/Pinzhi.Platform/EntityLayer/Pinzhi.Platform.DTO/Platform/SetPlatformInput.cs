using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    public class SetPlatformInput
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Key { set; get; }
        public string Icon { set; get; }
        public string Author { set; get; }
        public string Developer { set; get; }
        public string Remark { get; set; }
        public int SortId { get; set; }
        public DateTime AddTime { set; get; }
        public bool IsDel { set; get; }
    }
}
