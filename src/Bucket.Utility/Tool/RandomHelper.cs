using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Bucket.Tool
{
    /// <summary>
    /// 随机数帮助类
    /// </summary>
    public class RandomHelper
    {
        private static Random _random = new Random(); //随机发生器
        private readonly static string _randomlibrarystr = ""; //随机库
        private static char[] _randomlibrary; //随机库
        static RandomHelper()
        {
            if (string.IsNullOrWhiteSpace(_randomlibrarystr))
                _randomlibrary = "123456789abcdefghjkmnpqrstuvwxy".ToCharArray();
            else
                _randomlibrary = _randomlibrarystr.ToCharArray();
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
        /// <summary>
        /// 创建随机对
        /// </summary>
        /// <param name="length">长度</param>
        /// <param name="onlyNumber">是否只包含数字</param>
        /// <param name="randomKey">随机键</param>
        /// <param name="randomValue">随机值</param>
        public static void CreateRandomPair(int length, bool onlyNumber, out string randomKey, out string randomValue)
        {
            StringBuilder randomKeySB = new StringBuilder();
            StringBuilder randomValueSB = new StringBuilder();

            int index1;
            int index2;
            for (int i = 0; i < length; i++)
            {
                if (onlyNumber)
                {
                    index1 = _random.Next(0, 10);
                    index2 = _random.Next(0, 10);
                }
                else
                {
                    index1 = _random.Next(0, _randomlibrary.Length);
                    index2 = _random.Next(0, _randomlibrary.Length);
                }

                randomKeySB.Append(_randomlibrary[index1]);
                randomValueSB.Append(_randomlibrary[index2]);
            }
            randomKey = randomKeySB.ToString();
            randomValue = randomValueSB.ToString();
        }
    }
}
