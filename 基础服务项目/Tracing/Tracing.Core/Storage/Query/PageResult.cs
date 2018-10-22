using System.Collections.Generic;

namespace Tracing.Storage.Query
{
    public class PageResult<T>
    {
        public int TotalPageCount { get; set; }
        
        public int TotalMemberCount { get; set; }
        
        public int CurrentPageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public IEnumerable<T> Data { get; set; }
    }
}