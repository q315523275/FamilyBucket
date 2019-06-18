
using Bucket.Config.Abstractions;
using Bucket.Config.Implementation;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
namespace Bucket.Config.Configuration
{
    /// <summary>
    /// 翻看底层源码,暂未找到合理方案,除非改变整体结构
    /// </summary>
    public class BucketConfigurationProvider : ConfigurationProvider, IDataChangeListener, IConfigurationSource
    {
        private readonly ConfigurationHelper _configurationHelper;
        public BucketConfigurationProvider(BucketConfigOptions options)
        {
            _configurationHelper = new ConfigurationHelper(options);
            Data = new ConcurrentDictionary<string, string>();
        }

        public override void Load()
        {
            DataChangeListenerDictionary.Add(this);
            Data = _configurationHelper.Get().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void SetData(ConcurrentDictionary<string, string> changeData)
        {
            foreach (var dic in changeData)
            {
                if (Data.ContainsKey(dic.Key))
                    Data[dic.Key] = dic.Value;
                else
                    Data.Add(dic);
            }
            // Data = new Dictionary<string, string>(_configRepository.Data, StringComparer.OrdinalIgnoreCase);
        }

        public void OnDataChange(ConcurrentDictionary<string, string> changeData)
        {
            SetData(changeData);
            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}
