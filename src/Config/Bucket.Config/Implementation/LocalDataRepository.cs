using Bucket.Config.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Bucket.Config.Implementation
{
    public class LocalDataRepository : ILocalDataRepository
    {
        private readonly string localcachepath = Path.Combine(AppContext.BaseDirectory, "localconfig.json");
        private readonly ILogger<LocalDataRepository> _logger;

        public LocalDataRepository(ILogger<LocalDataRepository> logger)
        {
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
                var json = JsonConvert.SerializeObject(dic);
                File.WriteAllText(localcachepath, json);
            }
            catch (Exception ex)
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
                    return JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "本地配置加载失败");
            }
            return new ConcurrentDictionary<string, string>();
        }
    }
}
