using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigService.Model
{
    [SugarTable("tb_appnamespace")]
    public class AppNamespaceInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        /// <summary>
        /// 全局唯一
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { set; get; }
        /// <summary>
        /// namespace是否为公共
        /// </summary>
        public bool IsPublic { set; get; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { set; get; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public long CreateUid { set; get; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastTime { set; get; }
        /// <summary>
        /// 最后修改人Id
        /// </summary>
        public long LastUid { set; get; }
    }
}
