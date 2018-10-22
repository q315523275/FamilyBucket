namespace Pinzhi.Identity.Dto
{
    /// <summary>
    /// 返回值
    /// </summary>
    public class BaseOutput
    {
        public BaseOutput()
        {
            this.ErrorCode = "000000";
            this.Message = "操作成功";
        }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
