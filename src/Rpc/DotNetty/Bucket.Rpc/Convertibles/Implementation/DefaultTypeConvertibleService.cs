using Bucket.Rpc.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bucket.Rpc.Convertibles.Implementation
{
    /// <summary>
    /// 默认类型转换服务（需要注入）
    /// </summary>
    public class DefaultTypeConvertibleService : ITypeConvertibleService
    {
        private readonly IEnumerable<TypeConvertDelegate> _converters;
        private readonly ILogger<DefaultTypeConvertibleService> _logger;

        public DefaultTypeConvertibleService(IEnumerable<ITypeConvertibleProvider> providers,
            ILogger<DefaultTypeConvertibleService> logger)
        {
            _logger = logger;
            providers = providers.ToArray();

            _logger.LogInformation($"发现了以下类型转换提供程序：{string.Join(",", providers.Select(p => p.ToString()))}");
            _converters = providers.SelectMany(p => p.GetConverters()).ToArray();
        }

        /// <summary>
        /// 将未知类型转换为指定的类型
        /// </summary>
        /// <param name="instance">需要转换的实例</param>
        /// <param name="conversionType">转换的类型</param>
        /// <returns>转换之后的类型，如果无法转换则返回null</returns>
        public object Convert(object instance, Type conversionType)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (conversionType == null) throw new ArgumentNullException(nameof(conversionType));
            // Determines whether the specified object is an instance of the current Type
            // 确定指定对象是否为当前类型的实例
            if (conversionType.GetTypeInfo().IsInstanceOfType(instance)) return instance;

            _logger.LogInformation($"准备将 {instance.GetType()} 转换为：{conversionType}");

            object result = null;
            foreach (var converter in _converters)
            {
                result = converter(instance, conversionType);
                if (result != null) break;
            }

            if (result != null) return result;

            var exception = new RpcException($"无法将实例：{instance}转换为{conversionType}");
            throw exception;
        }
    }
}
