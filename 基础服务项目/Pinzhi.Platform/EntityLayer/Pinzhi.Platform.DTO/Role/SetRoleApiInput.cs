using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    public class SetRoleApiInput
    {
        /// <summary>
        /// 角色Id 
        /// </summary>
        public int RoleId { set; get; }
        /// <summary>
        /// 接口数组
        /// </summary>
        public List<int> ApiIdList { set; get; }
    }
}
