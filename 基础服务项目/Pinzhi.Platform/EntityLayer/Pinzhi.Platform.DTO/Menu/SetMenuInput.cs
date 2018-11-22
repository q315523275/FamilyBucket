using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    public class SetMenuInput
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 平台编号
        /// </summary>
        public int PlatformId { get; set; }
        /// <summary>
        /// 父编号
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
    }
}
