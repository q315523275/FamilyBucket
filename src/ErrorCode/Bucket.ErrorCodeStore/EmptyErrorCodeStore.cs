using Bucket.ErrorCode;
using System.Threading.Tasks;

namespace Bucket.ErrorCodeStore
{
    public class EmptyErrorCodeStore : IErrorCodeStore
    {
        public EmptyErrorCodeStore()
        {
        }

        public string StringGet(string code)
        {
            return string.Empty;
        }

        public Task<string> StringGetAsync(string code)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
