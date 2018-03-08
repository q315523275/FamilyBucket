using System;
using System.ComponentModel;
using System.Linq;
namespace Bucket.Extend
{
    /// <summary>
    /// Enum 扩展类
    /// </summary>
    public static class ExtEnumHelper
    {
        /// <summary>
        /// 获取枚举成员的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(this Enum obj)
        {
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 尝试转换枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string obj) where T : struct
        {
            if (string.IsNullOrEmpty(obj))
            {
                return default(T);
            }
            try
            {
                return (T)Enum.Parse(typeof(T), obj, true);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 获取指定枚举成员的描述
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Enum obj)
        {
            var attribs = (DescriptionAttribute[])obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attribs.Length > 0 ? attribs[0].Description : obj.ToString();
        }

        /// <summary>
        /// 根据枚举值，获取指定枚举类的成员描述
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetDescriptionString(this Type type, int? id)
        {
            var values = from Enum e in Enum.GetValues(type)
                         select new { id = e.ToInt(), name = e.ToDescriptionString() };

            if (!id.HasValue) id = 0;

            try
            {
                return values.ToList().Find(c => c.id == id).name;
            }
            catch
            {
                return "";
            }
        }
    }
}
