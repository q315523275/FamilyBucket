using System;
using System.Collections.Generic;
using System.Linq;

namespace Bucket.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 过滤开头字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="trim"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static string TrimStart(this string source, string trim, StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (source == null)
            {
                return null;
            }
            string s = source;
            while (s.StartsWith(trim, stringComparison))
            {
                s = s.Substring(trim.Length);
            }
            return s;
        }
        /// <summary>
        /// 分割逗号的字符串为List<string>
        /// </summary>
        /// <param name="csvList"></param>
        /// <param name="nullOrWhitespaceInputReturnsNull">nullorwhitespace字符串是否返回空对象</param>
        /// <returns></returns>
        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }
        /// <summary>
        /// 判断字符串是否不为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhitespace(this string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }
    }
}
