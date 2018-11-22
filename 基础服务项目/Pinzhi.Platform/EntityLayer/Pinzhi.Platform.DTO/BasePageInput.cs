using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Dto
{
    /// <summary>
    /// 分页入参基类
    /// </summary>
    public class BasePageInput
    {
        public BasePageInput()
        {
            this.PageIndex = 1;
            this.PageSize = 20;
        }
        /// <summary>
        /// 当前分页
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { set; get; }
    }
}
