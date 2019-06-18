using System.Threading.Tasks;

namespace Bucket.ErrorCode
{
    public class EmptyErrorCode : IErrorCode
    {
        public EmptyErrorCode()
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
