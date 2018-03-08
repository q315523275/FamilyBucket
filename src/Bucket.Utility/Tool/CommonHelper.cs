using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Bucket.Tool
{
    /// <summary>
    /// 普通帮助类
    /// </summary>
    public class CommonHelper
    {
        //星期数组
        private static string[] _weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
        //空格、回车、换行符、制表符正则表达式
        private static Regex _tbbrRegex = new Regex(@"\s*|\t|\r|\n", RegexOptions.IgnoreCase);

        #region 时间操作

        /// <summary>
        /// 获得当前时间的""yyyy-MM-dd HH:mm:ss:fffffff""格式字符串
        /// </summary>
        public static string GetDateTimeMS()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 获得当前时间的""yyyy年MM月dd日 HH:mm:ss""格式字符串
        /// </summary>
        public static string GetDateTimeU()
        {
            return string.Format("{0:U}", DateTime.Now);
        }

        /// <summary>
        /// 获得当前时间的""yyyy-MM-dd HH:mm:ss""格式字符串
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获得当前日期
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获得中文当前日期
        /// </summary>
        public static string GetChineseDate()
        {
            return DateTime.Now.ToString("yyyy月MM日dd");
        }

        /// <summary>
        /// 获得当前时间(不含日期部分)
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 获得当前小时
        /// </summary>
        public static string GetHour()
        {
            return DateTime.Now.Hour.ToString("00");
        }

        /// <summary>
        /// 获得当前天
        /// </summary>
        public static string GetDay()
        {
            return DateTime.Now.Day.ToString("00");
        }

        /// <summary>
        /// 获得当前月
        /// </summary>
        public static string GetMonth()
        {
            return DateTime.Now.Month.ToString("00");
        }

        /// <summary>
        /// 获得当前年
        /// </summary>
        public static string GetYear()
        {
            return DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// 获得当前星期(数字)
        /// </summary>
        public static string GetDayOfWeek()
        {
            return ((int)DateTime.Now.DayOfWeek).ToString();
        }

        /// <summary>
        /// 获得当前星期(汉字)
        /// </summary>
        public static string GetWeek()
        {
            return _weekdays[(int)DateTime.Now.DayOfWeek];
        }

        #endregion

        #region 数组操作

        /// <summary>
        /// 获得字符串在字符串数组中的位置
        /// </summary>
        public static int GetIndexInArray(string s, string[] array, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(s) || array == null || array.Length == 0)
                return -1;

            int index = 0;
            string temp = null;

            if (ignoreCase)
                s = s.ToLower();

            foreach (string item in array)
            {
                if (ignoreCase)
                    temp = item.ToLower();
                else
                    temp = item;

                if (s == temp)
                    return index;
                else
                    index++;
            }

            return -1;
        }

        /// <summary>
        /// 获得字符串在字符串数组中的位置
        /// </summary>
        public static int GetIndexInArray(string s, string[] array)
        {
            return GetIndexInArray(s, array, false);
        }

        /// <summary>
        /// 判断字符串是否在字符串数组中
        /// </summary>
        public static bool IsInArray(string s, string[] array, bool ignoreCase)
        {
            return GetIndexInArray(s, array, ignoreCase) > -1;
        }

        /// <summary>
        /// 判断字符串是否在字符串数组中
        /// </summary>
        public static bool IsInArray(string s, string[] array)
        {
            return IsInArray(s, array, false);
        }

        /// <summary>
        /// 判断字符串是否在字符串中
        /// </summary>
        public static bool IsInArray(string s, string array, string splitStr, bool ignoreCase)
        {
            return IsInArray(s, StringHelper.SplitString(array, splitStr), ignoreCase);
        }

        /// <summary>
        /// 判断字符串是否在字符串中
        /// </summary>
        public static bool IsInArray(string s, string array, string splitStr)
        {
            return IsInArray(s, StringHelper.SplitString(array, splitStr), false);
        }

        /// <summary>
        /// 判断字符串是否在字符串中
        /// </summary>
        public static bool IsInArray(string s, string array)
        {
            return IsInArray(s, StringHelper.SplitString(array, ","), false);
        }



        /// <summary>
        /// 将对象数组拼接成字符串
        /// </summary>
        public static string ObjectArrayToString(object[] array, string splitStr)
        {
            if (array == null || array.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
                result.AppendFormat("{0}{1}", array[i], splitStr);

            return result.Remove(result.Length - splitStr.Length, splitStr.Length).ToString();
        }

        /// <summary>
        /// 将对象数组拼接成字符串
        /// </summary>
        public static string ObjectArrayToString(object[] array)
        {
            return ObjectArrayToString(array, ",");
        }

        /// <summary>
        /// 将字符串数组拼接成字符串
        /// </summary>
        public static string StringArrayToString(string[] array, string splitStr)
        {
            return ObjectArrayToString(array, splitStr);
        }

        /// <summary>
        /// 将字符串数组拼接成字符串
        /// </summary>
        public static string StringArrayToString(string[] array)
        {
            return StringArrayToString(array, ",");
        }

        /// <summary>
        /// 将整数数组拼接成字符串
        /// </summary>
        public static string IntArrayToString(int[] array, string splitStr)
        {
            if (array == null || array.Length == 0)
                return "";

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
                result.AppendFormat("{0}{1}", array[i], splitStr);

            return result.Remove(result.Length - splitStr.Length, splitStr.Length).ToString();
        }

        /// <summary>
        /// 将整数数组拼接成字符串
        /// </summary>
        public static string IntArrayToString(int[] array)
        {
            return IntArrayToString(array, ",");
        }



        /// <summary>
        /// 移除数组中的指定项
        /// </summary>
        /// <param name="array">源数组</param>
        /// <param name="removeItem">要移除的项</param>
        /// <param name="removeBackspace">是否移除空格</param>
        /// <param name="ignoreCase">是否忽略大小写</param>
        /// <returns></returns>
        public static string[] RemoveArrayItem(string[] array, string removeItem, bool removeBackspace, bool ignoreCase)
        {
            if (array != null && array.Length > 0)
            {
                StringBuilder arrayStr = new StringBuilder();
                if (ignoreCase)
                    removeItem = removeItem.ToLower();
                string temp = "";
                foreach (string item in array)
                {
                    if (ignoreCase)
                        temp = item.ToLower();
                    else
                        temp = item;

                    if (temp != removeItem)
                        arrayStr.AppendFormat("{0}_", removeBackspace ? item.Trim() : item);
                }

                return StringHelper.SplitString(arrayStr.Remove(arrayStr.Length - 1, 1).ToString(), "_");
            }

            return array;
        }

        /// <summary>
        /// 移除数组中的指定项
        /// </summary>
        /// <param name="array">源数组</param>
        /// <returns></returns>
        public static string[] RemoveArrayItem(string[] array)
        {
            return RemoveArrayItem(array, "", true, false);
        }

        /// <summary>
        /// 移除字符串中的指定项
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="splitStr">分割字符串</param>
        /// <returns></returns>
        public static string[] RemoveStringItem(string s, string splitStr)
        {
            return RemoveArrayItem(StringHelper.SplitString(s, splitStr), "", true, false);
        }

        /// <summary>
        /// 移除字符串中的指定项
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <returns></returns>
        public static string[] RemoveStringItem(string s)
        {
            return RemoveArrayItem(StringHelper.SplitString(s), "", true, false);
        }



        /// <summary>
        /// 移除数组中的重复项
        /// </summary>
        /// <returns></returns>
        public static int[] RemoveRepeaterItem(int[] array)
        {
            if (array == null || array.Length < 2)
                return array;

            Array.Sort(array);

            int length = 1;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[i - 1])
                    length++;
            }

            int[] uniqueArray = new int[length];
            uniqueArray[0] = array[0];
            int j = 1;
            for (int i = 1; i < array.Length; i++)
                if (array[i] != array[i - 1])
                    uniqueArray[j++] = array[i];

            return uniqueArray;
        }

        /// <summary>
        /// 移除数组中的重复项
        /// </summary>
        /// <returns></returns>
        public static string[] RemoveRepeaterItem(string[] array)
        {
            if (array == null || array.Length < 2)
                return array;

            Array.Sort(array);

            int length = 1;
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[i - 1])
                    length++;
            }

            string[] uniqueArray = new string[length];
            uniqueArray[0] = array[0];
            int j = 1;
            for (int i = 1; i < array.Length; i++)
                if (array[i] != array[i - 1])
                    uniqueArray[j++] = array[i];

            return uniqueArray;
        }

        /// <summary>
        /// 去除字符串中的重复元素
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString(string s)
        {
            return GetUniqueString(s, ",");
        }

        /// <summary>
        /// 去除字符串中的重复元素
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString(string s, string splitStr)
        {
            return ObjectArrayToString(RemoveRepeaterItem(StringHelper.SplitString(s, splitStr)), splitStr);
        }

        #endregion

        /// <summary>
        /// 去除字符串首尾处的空格、回车、换行符、制表符
        /// </summary>
        public static string TBBRTrim(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.Trim().Trim('\r').Trim('\n').Trim('\t');
            return string.Empty;
        }

        /// <summary>
        /// 去除字符串中的空格、回车、换行符、制表符
        /// </summary>
        public static string ClearTBBR(string str)
        {
            if (!string.IsNullOrEmpty(str))
                return _tbbrRegex.Replace(str, "");

            return string.Empty;
        }

        /// <summary>
        /// 删除字符串中的空行
        /// </summary>
        /// <returns></returns>
        public static string DeleteNullOrSpaceRow(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            string[] tempArray = StringHelper.SplitString(s, "\r\n");
            StringBuilder result = new StringBuilder();
            foreach (string item in tempArray)
            {
                if (!string.IsNullOrWhiteSpace(item))
                    result.AppendFormat("{0}\r\n", item);
            }
            if (result.Length > 0)
                result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        /// <summary>
        /// 获得指定数量的html空格
        /// </summary>
        /// <returns></returns>
        public static string GetHtmlBS(int count)
        {
            if (count == 1)
                return "&nbsp;&nbsp;&nbsp;&nbsp;";
            else if (count == 2)
                return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            else if (count == 3)
                return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            else
            {
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < count; i++)
                    result.Append("&nbsp;&nbsp;&nbsp;&nbsp;");

                return result.ToString();
            }
        }

        /// <summary>
        /// 获得指定数量的htmlSpan元素
        /// </summary>
        /// <returns></returns>
        public static string GetHtmlSpan(int count)
        {
            if (count <= 0)
                return "";

            if (count == 1)
                return "<span></span>";
            else if (count == 2)
                return "<span></span><span></span>";
            else if (count == 3)
                return "<span></span><span></span><span></span>";
            else
            {
                StringBuilder result = new StringBuilder();

                for (int i = 0; i < count; i++)
                    result.Append("<span></span>");

                return result.ToString();
            }
        }

        /// <summary>
        ///获得邮箱提供者
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static string GetEmailProvider(string email)
        {
            int index = email.LastIndexOf('@');
            if (index > 0)
                return email.Substring(index + 1);
            return string.Empty;
        }

        /// <summary>
        /// 转义正则表达式
        /// </summary>
        public static string EscapeRegex(string s)
        {
            string[] oList = { "\\", ".", "+", "*", "?", "{", "}", "[", "^", "]", "$", "(", ")", "=", "!", "<", ">", "|", ":" };
            string[] eList = { "\\\\", "\\.", "\\+", "\\*", "\\?", "\\{", "\\}", "\\[", "\\^", "\\]", "\\$", "\\(", "\\)", "\\=", "\\!", "\\<", "\\>", "\\|", "\\:" };
            for (int i = 0; i < oList.Length; i++)
                s = s.Replace(oList[i], eList[i]);
            return s;
        }

        /// <summary>
        /// 将ip地址转换成long类型
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static long ConvertIPToLong(string ip)
        {
            string[] ips = ip.Split('.');
            long number = 16777216L * long.Parse(ips[0]) + 65536L * long.Parse(ips[1]) + 256 * long.Parse(ips[2]) + long.Parse(ips[3]);
            return number;
        }

        /// <summary>
        /// 隐藏邮箱
        /// </summary>
        public static string HideEmail(string email)
        {
            int index = email.LastIndexOf('@');

            if (index == 1)
                return "*" + email.Substring(index);
            if (index == 2)
                return email[0] + "*" + email.Substring(index);

            StringBuilder sb = new StringBuilder();
            sb.Append(email.Substring(0, 2));
            int count = index - 2;
            while (count > 0)
            {
                sb.Append("*");
                count--;
            }
            sb.Append(email.Substring(index));
            return sb.ToString();
        }

        /// <summary>
        /// 隐藏手机
        /// </summary>
        public static string HideMobile(string mobile)
        {
            if (mobile != null && mobile.Length > 10)
                return mobile.Substring(0, 3) + "*****" + mobile.Substring(8);
            return string.Empty;
        }

        /// <summary>
        /// 数据转换为列表
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns></returns>
        public static List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>(array.Length);
            foreach (T item in array)
                list.Add(item);
            return list;
        }

        /// <summary> 
        /// DataTable转化为List
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns></returns> 
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            int columnCount = dt.Columns.Count;
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>(dt.Rows.Count);
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>(columnCount);
                for (int i = 0; i < columnCount; i++)
                {
                    item.Add(dt.Columns[i].ColumnName, dr[i]);
                }
                list.Add(item);
            }
            return list;
        }
    }
}
