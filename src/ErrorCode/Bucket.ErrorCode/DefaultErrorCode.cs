using Bucket.ErrorCode.Abstractions;
using Bucket.ErrorCode.Utils;
using System.Linq;
namespace Bucket.ErrorCode
{
    public class DefaultErrorCode : IErrorCode
    {
        private readonly IDataRepository _dataRepository;
        private readonly ThreadSafe.Boolean _loaded;
        public DefaultErrorCode(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _loaded = new ThreadSafe.Boolean(false);
        }

        public string StringGet(string code)
        {
            if (!_loaded.ReadFullFence())
                _dataRepository.Get().ConfigureAwait(false).GetAwaiter().GetResult();

            _loaded.WriteFullFence(true);

            var rec = _dataRepository.Data.FirstOrDefault(it => it.ErrorCode == code);

            if (rec != null)
                return rec.ErrorMessage;

            return string.Empty;
        }
    }
}
