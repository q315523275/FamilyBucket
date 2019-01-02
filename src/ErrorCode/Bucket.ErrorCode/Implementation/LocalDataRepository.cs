using Bucket.Core;
using Bucket.ErrorCode.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bucket.ErrorCode.Implementation
{
    public class LocalDataRepository: ILocalDataRepository
    {
        private readonly string localcachepath = Path.Combine(AppContext.BaseDirectory, "localerrorcode.json");
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<LocalDataRepository> _logger;

        public LocalDataRepository(IJsonHelper jsonHelper, ILogger<LocalDataRepository> logger)
        {
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public bool Set(IList<ApiErrorCodeInfo> list)
        {
            try
            {
                _logger.LogInformation($"错误码信息写入本地文件:{localcachepath}");
                string dir = Path.GetDirectoryName(localcachepath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var json = _jsonHelper.SerializeObject(list);
                File.WriteAllText(localcachepath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "本地错误码写入文件失败");
                return false;
            }
            return true;
        }
        public IList<ApiErrorCodeInfo> Get()
        {
            try
            {
                if (File.Exists(localcachepath))
                {
                    var json = File.ReadAllText(localcachepath);
                    return _jsonHelper.DeserializeObject<List<ApiErrorCodeInfo>>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "本地错误码加载失败");
            }
            return new List<ApiErrorCodeInfo>();
        }
    }
}
