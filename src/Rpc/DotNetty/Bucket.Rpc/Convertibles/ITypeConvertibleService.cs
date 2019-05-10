using System;

namespace Bucket.Rpc.Convertibles
{
    /// <summary>
    /// 抽象的类型转换服务
    /// </summary>
    public interface ITypeConvertibleService
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="instance">需要转换的实例</param>
        /// <param name="conversionType">转换的类型</param>
        /// <returns>转换之后的类型，如果无法转换则返回null</returns>
        object Convert(object instance, Type conversionType);
    }
}
