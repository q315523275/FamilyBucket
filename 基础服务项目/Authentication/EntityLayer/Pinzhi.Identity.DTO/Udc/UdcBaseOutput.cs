namespace Pinzhi.Identity.Dto.Udc
{
    public class UdcBaseOutput<T>
    {
        public bool success { set; get; }
        public T data { set; get; }
    }
}
