using System.Collections.Generic;

namespace Tracing.Server.ViewModels
{
    public class PageViewModel<T>
    {
        public int TotalPageCount { get; set; }
        
        public int TotalMemberCount { get; set; }
        
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public IEnumerable<T> Data { get; set; }
    }
}