namespace Pinzhi.Identity.Dto
{
    /// <summary>
    /// 分页返回基类
    /// </summary>
    public class BasePageOutput : BaseOutput
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { set; get; }
        /// <summary>
        /// 总条数
        /// </summary>
        public long Total { set; get; }
    }
}
