namespace Pinzhi.Identity.Dto
{
    /// <summary>
    /// 分页入参基类
    /// </summary>
    public class BasePageInput: BaseInput
    {
        public BasePageInput()
        {
            this.PageIndex = 0;
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
