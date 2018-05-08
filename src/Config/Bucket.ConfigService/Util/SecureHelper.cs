using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Bucket.ConfigCenter.Util
{
    public class SecureHelper
    {
        /// <summary>
        /// SHA256加密
        /// </summary>
        public static string SHA256(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            SHA256Managed managed = new SHA256Managed();
            return ToHexString(managed.ComputeHash(bytes));
        }
        /// <summary>
        /// Hex
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] text)
        {
            StringBuilder ret = new StringBuilder();
            foreach (byte b in text)
            {
                ret.AppendFormat("{0:x2}", b);
            }
            return ret.ToString();
        }
    }
}
