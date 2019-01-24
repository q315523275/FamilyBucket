using Bucket.Rsa;
using System;
using System.Text;
using XC.RSAUtil;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var keyList = RsaKeyGenerator.Pkcs8Key(2048, true);
            var privateKey = keyList[0];
            var publicKey = keyList[1];
            var str = "我是原字符串";
            Console.WriteLine($"原字符串:{str}");
            str = RsaHelper.JavaRsaEncrypt(str, publicKey, RsaSignFormat.Hex);
            Console.WriteLine($"Rsa加密符串:{str}");
            str = RsaHelper.JavaRsaDecrypt(str, privateKey, RsaSignFormat.Hex);
            Console.WriteLine($"Rsa解密符串:{str}");
            var sign = RsaHelper.JavaRsaSign(str, privateKey, RsaSignFormat.Hex);
            Console.WriteLine($"Rsa签名:{sign}");
            Console.WriteLine($"Rsa验签:{RsaHelper.JavaRsaSignVerify(str, sign, publicKey, RsaSignFormat.Hex)}");
            Console.Read();
        }
    }
}
