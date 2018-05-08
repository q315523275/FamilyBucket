using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bucket.Utility.Helpers
{
    /// <summary>
    /// 随机数操作
    /// </summary>
    public class Randoms
    {
        private static Random _random = new Random(); //随机发生器
        private readonly static string _randomlibrarystr = ""; //随机库
        private static char[] _randomlibrary = "123456789abcdefghjkmnpqrstuvwxy".ToCharArray(); //随机库
        /// <summary>
        /// 对集合随机排序
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="array">集合</param>
        public static List<T> Sort<T>(IEnumerable<T> array)
        {
            if (array == null)
                return null;
            var random = new System.Random();
            var list = array.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                int index1 = random.Next(0, list.Count);
                int index2 = random.Next(0, list.Count);
                T temp = list[index1];
                list[index1] = list[index2];
                list[index2] = temp;
            }
            return list;
        }
        /// <summary>
        /// 创建随机值
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="onlyNumber">是否只包含数字</param>
        /// <returns>随机值</returns>
        public static string CreateRandomValue(int length, bool onlyNumber)
        {
            int index;
            StringBuilder randomValue = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if (onlyNumber)
                    index = _random.Next(0, 9);
                else
                    index = _random.Next(0, _randomlibrary.Length);

                randomValue.Append(_randomlibrary[index]);
            }
            return randomValue.ToString();
        }
    }
}
