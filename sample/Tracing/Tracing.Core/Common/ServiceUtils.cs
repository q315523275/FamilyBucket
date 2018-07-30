using System.Linq;
using Tracing.DataContract.Tracing;
using Tracing.Storage.Query;

namespace Tracing.Common
{
    public static class ServiceUtils
    {
        public const string UnKnownService = "unknown";
        
        public static string GetService(Span span)
        {
            return span?.Tags?.FirstOrDefault(x => x.Key == QueryConstants.Service)?.Value ?? UnKnownService;
        }
    }
}