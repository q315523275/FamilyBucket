using Microsoft.AspNetCore.Http;
using System;
using Bucket.Core;
namespace Bucket.AspNetCore.Commons
{
    public class HttpDataRepository : IRequestScopedDataRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpDataRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Add<T>(string key, T value)
        {
            try
            {
                _httpContextAccessor.HttpContext.Items.Add(key, value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public T Get<T>(string key)
        {
            if (_httpContextAccessor.HttpContext.Items.TryGetValue(key, out object obj))
            {
                return (T)obj;
            }
            return default;
        }

        public bool Update<T>(string key, T value)
        {
            try
            {
                _httpContextAccessor.HttpContext.Items[key] = value;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
