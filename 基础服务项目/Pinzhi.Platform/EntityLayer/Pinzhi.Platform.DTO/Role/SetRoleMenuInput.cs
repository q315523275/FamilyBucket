using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.DTO
{
    public class SetRoleMenuInput
    {
        /// <summary>
        /// 角色Id 
        /// </summary>
        public int RoleId { set; get; }
        /// <summary>
        /// 菜单数组
        /// </summary>
        public List<int> MenuIdList { set; get; }
    }
}
