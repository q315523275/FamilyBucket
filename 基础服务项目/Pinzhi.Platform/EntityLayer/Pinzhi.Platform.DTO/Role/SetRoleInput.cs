using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetRoleInput
    {
        /// <summary>
        /// 角色唯一标示符
        /// </summary>
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
        /// 描述
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSys { get; set; }
        /// <summary>
        /// 菜单数组
        /// </summary>
        public List<int> MenuIdList { set; get; }
        /// <summary>
        /// 接口数组
        /// </summary>
        public List<int> ApiIdList { set; get; }
    }
}
