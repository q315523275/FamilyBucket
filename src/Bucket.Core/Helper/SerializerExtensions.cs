namespace Bucket.Core.Helper
{
    /// <summary>
    /// 序列化器扩展方法
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">序列化内容类型</typeparam>
        /// <typeparam name="TResult">对象类型</typeparam>
        /// <param name="serializer">序列化器</param>
        /// <param name="content">序列化的内容</param>
        /// <returns>一个对象实例</returns>
        public static TResult Deserialize<T, TResult>(this ISerializer<T> serializer, T content)
        {
            return (TResult)serializer.Deserialize(content, typeof(TResult));
        }
    }
}
