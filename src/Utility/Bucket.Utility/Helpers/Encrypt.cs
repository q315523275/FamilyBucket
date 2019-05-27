using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Bucket.Utility.Helpers
{
    /// <summary>
    /// 加密操作
    /// </summary>
    public static class Encrypt
    {

        #region Md5加密

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By16(string value)
        {
            return Md5By16(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By16(string value, Encoding encoding)
        {
            return Md5(value, encoding, 4, 8);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        private static string Md5(string value, Encoding encoding, int? startIndex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex.SafeValue(), length.SafeValue());
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "").ToLower();
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By32(string value)
        {
            return Md5By32(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By32(string value, Encoding encoding)
        {
            return Md5(value, encoding, null, null);
        }

        #endregion

        #region AES加密
        /// <summary>
        /// AES密钥
        /// </summary>
        private static string _aeskey = "0123456789abcdef";
        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AesEncrypt(string content)
        {
            return AesEncrypt(content, _aeskey);
        }
        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key">常规字符串</param>
        /// <returns></returns>
        public static string AesEncrypt(string content, string key)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);
            var des = CreateSymmetricAlgorithm(key);

            using (var cTransform = des.CreateEncryptor())
            {
                byte[] resultArray = GetTransformFinalBlock(cTransform, toEncryptArray);
                return System.Convert.ToBase64String(resultArray);
            }
        }
        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AesDecrypt(string content)
        {
            return AesDecrypt(content, _aeskey);
        }
        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key">常规字符串</param>
        /// <returns></returns>
        public static string AesDecrypt(string content, string key)
        {
            byte[] toEncryptArray = System.Convert.FromBase64String(content);
            var des = CreateSymmetricAlgorithm(key);

            using (var cTransform = des.CreateDecryptor())
            {
                byte[] resultArray = GetTransformFinalBlock(cTransform, toEncryptArray);
                return Encoding.Default.GetString(resultArray);
            }
        }
        /// <summary>
        /// 创建SymmetricAlgorithm
        /// </summary>
        /// <param name="aesKey"></param>
        /// <returns></returns>
        private static SymmetricAlgorithm CreateSymmetricAlgorithm(string aesKey)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(aesKey);
            SymmetricAlgorithm des = Aes.Create();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            return des;
        }
        private static byte[] GetTransformFinalBlock(ICryptoTransform cTransform, byte[] dateArray)
        {
            return cTransform.TransformFinalBlock(dateArray, 0, dateArray.Length);
        }
        #endregion

        #region SHA256加密
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
        /// <summary>
        /// ToHexString
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToHexString(string text)
        {
            return ToHexString(Encoding.UTF8.GetBytes(text));
        }
        /// <summary>
        /// FromHexString
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] FromHexString(string hexString)
        {
            byte[] data = null;
            try
            {
                if (!string.IsNullOrEmpty(hexString))
                {
                    int length = hexString.Length / 2;
                    data = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        data[i] = System.Convert.ToByte(hexString.Substring(2 * i, 2), 16);
                    }
                }
            }
            catch
            {

            }
            return data;
        }
        #endregion

        #region HmacSha256加密

        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="key">密钥</param>
        public static string HmacSha256(string value, string key)
        {
            return HmacSha256(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// HMACSHA256加密
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码</param>
        public static string HmacSha256(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var sha256 = new HMACSHA256(encoding.GetBytes(key));
            var hash = sha256.ComputeHash(encoding.GetBytes(value));
            return string.Join("", hash.ToList().Select(t => t.ToString("x2")).ToArray());
        }

        #endregion
    }
}
