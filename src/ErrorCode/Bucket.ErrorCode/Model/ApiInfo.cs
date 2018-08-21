using System.Collections.Generic;

namespace Bucket.ErrorCode.Model
{
    public class ApiInfo
    {
        public string StatusCode { get; set; }
        public List<ApiErrorCodeInfo> Value { get; set; }
}
    public class ApiErrorCodeInfo
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
