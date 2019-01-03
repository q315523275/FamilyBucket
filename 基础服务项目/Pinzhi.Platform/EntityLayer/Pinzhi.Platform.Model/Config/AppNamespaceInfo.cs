using SqlSugar;
using System;

namespace Pinzhi.Platform.Model
{
    [SugarTable("tb_appnamespace")]
    public class AppNamespaceInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        public string Name { set; get; }
        public string AppId { set; get; }
        public bool IsPublic { set; get; }
        public string Comment { set; get; }
        public bool IsDeleted { set; get; }
        public DateTime CreateTime { set; get; }
        public long CreateUid { set; get; }
        public DateTime LastTime { set; get; }
        public long LastUid { set; get; }
    }
}
