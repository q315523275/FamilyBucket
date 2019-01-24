using System;
using System.Security.Cryptography;
using System.Text;
using XC.RSAUtil;

namespace Bucket.Rsa
{
    public static class RsaHelper
    {
        /// <summary>
        /// JavaRsa公钥签名验证RSAWithSHA256
        /// </summary>
        /// <param name="text">源数据</param>
        /// <param name="sign">签名</param>
        /// <param name="publicKey">java pem公钥</param>
        /// <param name="signFormat">base64/hex</param>
        public static bool JavaRsaSignVerify(string text, string sign, string publicKey, RsaSignFormat signFormat = RsaSignFormat.Base64)
        {
            var rsaProvider = new RsaPkcs8Util(Encoding.UTF8, publicKey);
            if (signFormat == RsaSignFormat.Hex) // => byte => base64
                sign = Convert.ToBase64String(FromHexString(sign));
            return rsaProvider.VerifyData(text, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        /// <summary>
        /// JavaRsa私钥签名RSAWithSHA256
        /// </summary>
        /// <param name="text"></param>
        /// <param name="privateKey"></param>
        /// <param name="signFormat"></param>
        /// <returns></returns>
        public static string JavaRsaSign(string text, string privateKey, RsaSignFormat signFormat = RsaSignFormat.Base64)
        {
            var rsaProvider = new RsaPkcs8Util(Encoding.UTF8, string.Empty, privateKey);
            var sign = rsaProvider.SignDataGetBytes(text, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return (signFormat == RsaSignFormat.Base64) ? Convert.ToBase64String(sign) : ToHexString(sign);
        }
        /// <summary>
        /// JavaRsa公钥加密,SHA256,字符长度不能大于公钥长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="publicKey"></param>
        /// <param name="signFormat"></param>
        /// <returns></returns>
        public static string JavaRsaEncrypt(string text, string publicKey, RsaSignFormat signFormat = RsaSignFormat.Base64)
        {
            var rsaProvider = new RsaPkcs8Util(Encoding.UTF8, publicKey);
            var encryptData = rsaProvider.Encrypt(text, RSAEncryptionPadding.Pkcs1);
            if (signFormat == RsaSignFormat.Hex) // => byte => hex
                encryptData = ToHexString(Convert.FromBase64String(encryptData));
            return encryptData;
        }
        /// <summary>
        /// JavaRsa私钥解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="privateKey"></param>
        /// <param name="signFormat"></param>
        /// <returns></returns>
        public static string JavaRsaDecrypt(string text, string privateKey, RsaSignFormat signFormat = RsaSignFormat.Base64)
        {
            var rsaProvider = new RsaPkcs8Util(Encoding.UTF8, string.Empty, privateKey);
            if (signFormat == RsaSignFormat.Hex) // => byte => base64
                text = Convert.ToBase64String(FromHexString(text));
            return rsaProvider.Decrypt(text, RSAEncryptionPadding.Pkcs1);
        }
        /// <summary>
        /// 16进制数据转换
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
                        data[i] = Convert.ToByte(hexString.Substring(2 * i, 2), 16);
                    }
                }
            }
            catch
            {

            }
            return data;
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
    }
    public enum RsaSignFormat
    {
        Base64,
        Hex
    }
}
