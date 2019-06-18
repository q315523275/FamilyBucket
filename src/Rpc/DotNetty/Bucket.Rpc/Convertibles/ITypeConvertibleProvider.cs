using Bucket.Rpc.Convertibles.Implementation;
using System.Collections.Generic;

namespace Bucket.Rpc.Convertibles
{
    /// <summary>
    /// 抽象的类型转换提供程序
    /// </summary>
    public interface ITypeConvertibleProvider
    {
        /// <summary>
        /// 获取类型转换器
        /// </summary>
        /// <returns>类型转换器集合</returns>
        IEnumerable<TypeConvertDelegate> GetConverters();
    }
}
