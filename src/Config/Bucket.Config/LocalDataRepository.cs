using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Logging;
using Bucket.Core;
using System.Collections.Concurrent;

namespace Bucket.Config
{
    public class LocalDataRepository: ILocalDataRepository
    {
        private readonly string localcachepath = Path.Combine(AppContext.BaseDirectory, "localconfig.json");
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<LocalDataRepository> _logger;

        public LocalDataRepository(IJsonHelper jsonHelper, ILogger<LocalDataRepository> logger)
        {
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public bool Set(ConcurrentDictionary<string, string> dic)
        {
            try
            {
                _logger.LogInformation($"配置中心配置信息写入本地文件:{localcachepath}");
                string dir = Path.GetDirectoryName(localcachepath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var json = _jsonHelper.SerializeObject(dic);
                File.WriteAllText(localcachepath, json);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "配置中心配置写入文件失败");
                return false;
            }
            return true;
        }
        public ConcurrentDictionary<string, string> Get()
        {
            try
            {
                if (File.Exists(localcachepath))
                {
                    var json = File.ReadAllText(localcachepath);
                    return _jsonHelper.DeserializeObject<ConcurrentDictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "本地配置写入文件失败");
            }
            return new ConcurrentDictionary<string, string>();
        }
    }
}
