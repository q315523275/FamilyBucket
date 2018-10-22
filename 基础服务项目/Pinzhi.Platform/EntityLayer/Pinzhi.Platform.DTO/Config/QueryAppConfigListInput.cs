namespace Pinzhi.Platform.DTO
{
    public class QueryAppConfigListInput: BasePageInput
    {
        public string AppId { set; get; }
        public string NameSpace { set; get; }
        public string Environment { set; get; }
    }
}
