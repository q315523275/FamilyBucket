namespace Bucket.ErrorCode
{
    public class DefaultErrorCode : IErrorCode
    {
        private readonly RemoteStoreRepository _storeRepository;
        public DefaultErrorCode(RemoteStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }
        public string StringGet(string code)
        {
            if (_storeRepository.GetStore().TryGetValue(code, out string value))
            {
                return value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
