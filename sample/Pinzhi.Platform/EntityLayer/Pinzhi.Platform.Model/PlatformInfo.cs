using SqlSugar;
using System;

namespace Pinzhi.Platform.Model
{
    [SugarTable("tb_platforms")]
    public class PlatformInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
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
