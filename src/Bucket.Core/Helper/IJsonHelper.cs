namespace Bucket.Core
{
    public interface IJsonHelper
    {
        /// <summary>
        /// Json.NET
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string SerializeObject(object value);
        /// <summary>
        /// Json.NET
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        T DeserializeObject<T>(string value);
    }
}
