using SqlSugar;
using System;
namespace Pinzhi.Identity.Model
{
    /// <summary>
    /// 接口角色信息类
    /// </summary>
    [SugarTable("tb_roles")]
    public class RoleInfo
    {
        /// <summary>
        /// 角色唯一标示符
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 角色唯一标示符
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        ///角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
    }
}
