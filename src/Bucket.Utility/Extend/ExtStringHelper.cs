using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.Extend
{
    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class ExtStringHelper
    {
        /// <summary>
        /// 部分字符串获取
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlen"></param>
        /// <returns></returns>
        public static string SubString2(this string str, int maxlen)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str.Length <= maxlen)
                return str;
            return str.Substring(0, maxlen);
        }
        /// <summary>
        /// 如果string空引用转空内容
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NullToEmpty(this string str)
        {
            if (str == null)
                return "";
            return str;
        }
        /// <summary>
        /// 去除html标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string Htmlstring)
        {
            if (!string.IsNullOrWhiteSpace(Htmlstring))
            {
                //删除脚本
                Htmlstring = Htmlstring.Replace("\r\n", "");
                Htmlstring = Regex.Replace(Htmlstring, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<style.*?</style>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase);
                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "\\s{2,}", "", RegexOptions.IgnoreCase);
                Htmlstring = Htmlstring.Replace("<", "");
                Htmlstring = Htmlstring.Replace(">", "");
                Htmlstring = Htmlstring.Replace("\r\n", "");
                Htmlstring = Htmlstring.Replace("\n", "");
            }
            return Htmlstring;
        }
        /// <summary>
        /// 去除2个以上的空格
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveMoreSpace(this string content)
        {
            if (string.IsNullOrEmpty(content)) return "";
            Regex r = new Regex(@"\s{2,}", RegexOptions.Multiline);
            return r.Replace(content, " ");
        }
        /// <summary>
        /// json特殊字符处理
        /// </summary>
        public static string EscapeJson(this string s)
        {
            if (s == null || s.Length == 0)
                return s;
            s = s.Replace("\\", "\\\\");
            s = s.Replace("\"", "\\\"");
            s = s.Replace("\r", "\\r");
            s = s.Replace("\n", "\\n");
            return s;
        }
        /// <summary>
        /// 获取字符串Byte值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByte(this string value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }
        
        public static string ToUnicode(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                builder.Append("\\u" + ((int)value[i]).ToString("x"));
            }
            return builder.ToString();
        }

        private static readonly Regex emailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex webUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex stripHTMLExpression = new Regex("<\\S[^><]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// 字符串替换
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string instance, params object[] args)
        {
            return string.Format(instance, args);
        }
        /// <summary>
        /// 枚举匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string instance, T defaultValue) where T : struct, IComparable, IFormattable
        {
            T convertedValue = defaultValue;
            if (!string.IsNullOrWhiteSpace(instance) && !Enum.TryParse(instance.Trim(), true, out convertedValue))
            {
                convertedValue = defaultValue;
            }
            return convertedValue;
        }
        /// <summary>
        /// 枚举匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int instance, T defaultValue) where T : struct, IComparable, IFormattable
        {
            if (!Enum.TryParse(instance.ToString(), true, out T convertedValue))
            {
                convertedValue = defaultValue;
            }
            return convertedValue;
        }

        public static string StripHtml(this string instance)
        {
            return stripHTMLExpression.Replace(instance, string.Empty);
        }
        /// <summary>
        /// 是否邮箱
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsEmail(this string instance)
        {
            return !string.IsNullOrWhiteSpace(instance) && emailExpression.IsMatch(instance);
        }
        /// <summary>
        /// 是否Url地址
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsWebUrl(this string instance)
        {
            return !string.IsNullOrWhiteSpace(instance) && webUrlExpression.IsMatch(instance);
        }
        /// <summary>
        /// 转bool
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool AsBool(this string instance)
        {
            bool.TryParse(instance, out bool result);
            return result;
        }
        /// <summary>
        /// 转datetime
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static DateTime AsDateTime(this string instance)
        {
            DateTime result = DateTime.MinValue;
            DateTime.TryParse(instance, out result);
            return result;
        }
        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Decimal AsDecimal(this string instance)
        {
            var result = (decimal)0.0;
            Decimal.TryParse(instance, out result);
            return result;
        }
        /// <summary>
        /// 转int
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static int AsInt(this string instance)
        {
            var result = (int)0;
            int.TryParse(instance, out result);
            return result;
        }
        /// <summary>
        /// 是否整型
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsInt(this string instance)
        {
            return int.TryParse(instance, out int result);
        }
        /// <summary>
        /// 是否时间类型
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string instance)
        {
            return DateTime.TryParse(instance, out DateTime result);
        }
        /// <summary>
        /// 是否浮点类型
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsFloat(this string instance)
        {
            return float.TryParse(instance, out float result);
        }
        /// <summary>
        /// 是否null或空值
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string instance)
        {
            return string.IsNullOrWhiteSpace(instance);
        }
        /// <summary>
        /// 是否有值
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNotNullAndWhiteSpace(this string instance)
        {
            return !string.IsNullOrWhiteSpace(instance);
        }
        /// <summary>
        /// 是否null或空值
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string theString)
        {
            return string.IsNullOrEmpty(theString);
        }
        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string FirstCharToLowerCase(this string instance)
        {
            if (instance.IsNotNullAndWhiteSpace() && instance.Length > 1 && char.IsUpper(instance[0]))
            {
                return char.ToLower(instance[0]) + instance.Substring(1);
            }
            return instance;
        }
    }
}
