using System;
using System.Text.RegularExpressions;

namespace Bucket.Tool
{
    /// <summary>
    /// 验证帮助类
    /// </summary>
    public class ValidateHelper
    {
        //邮件正则表达式
        private static Regex _emailregex = new Regex(@"^[a-z0-9]([a-z0-9]*[-_]?[a-z0-9]+)*@([a-z0-9]*[-_]?[a-z0-9]+)+[\.][a-z]{2,3}([\.][a-z]{2})?$", RegexOptions.IgnoreCase);
        //手机号正则表达式
        private static Regex _mobileregex = new Regex("^(13|15|17|18)[0-9]{9}$");
        //固话号正则表达式
        private static Regex _phoneregex = new Regex(@"^(\d{3,4}-?)?\d{7,8}$");
        //IP正则表达式
        private static Regex _ipregex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        //日期正则表达式
        private static Regex _dateregex = new Regex(@"(\d{4})-(\d{1,2})-(\d{1,2})");
        //数值(包括整数和小数)正则表达式
        private static Regex _numericregex = new Regex(@"^[-]?[0-9]+(\.[0-9]+)?$");
        //邮政编码正则表达式
        private static Regex _zipcoderegex = new Regex(@"^\d{6}$");

        /// <summary>
        /// 是否为邮箱名
        /// </summary>
        public static bool IsEmail(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return _emailregex.IsMatch(s);
        }

        /// <summary>
        /// 是否为手机号
        /// </summary>
        public static bool IsMobile(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return _mobileregex.IsMatch(s);
        }

        /// <summary>
        /// 是否为固话号
        /// </summary>
        public static bool IsPhone(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return _phoneregex.IsMatch(s);
        }

        /// <summary>
        /// 是否为IP
        /// </summary>
        public static bool IsIP(string s)
        {
            return _ipregex.IsMatch(s);
        }

        /// <summary>
        /// 是否是身份证号
        /// </summary>
        public static bool IsIdCard(string id)
        {
            if (string.IsNullOrEmpty(id))
                return true;
            if (id.Length == 18)
                return CheckIDCard18(id);
            else if (id.Length == 15)
                return CheckIDCard15(id);
            else
                return false;
        }

        /// <summary>
        /// 是否为18位身份证号
        /// </summary>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
                return false;//数字验证

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
                return false;//省份验证

            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
                return false;//生日验证

            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());

            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
                return false;//校验码验证

            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 是否为15位身份证号
        /// </summary>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
                return false;//数字验证

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
                return false;//省份验证

            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
                return false;//生日验证

            return true;//符合15位身份证标准
        }

        /// <summary>
        /// 是否为日期
        /// </summary>
        public static bool IsDate(string s)
        {
            return _dateregex.IsMatch(s);
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsNumeric(string numericStr)
        {
            return _numericregex.IsMatch(numericStr);
        }

        /// <summary>
        /// 是否为邮政编码
        /// </summary>
        public static bool IsZipCode(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return _zipcoderegex.IsMatch(s);
        }

        /// <summary>
        /// 是否是图片文件名
        /// </summary>
        /// <returns> </returns>
        public static bool IsImgFileName(string fileName)
        {
            if (fileName.IndexOf(".") == -1)
                return false;

            string tempFileName = fileName.Trim().ToLower();
            string extension = tempFileName.Substring(tempFileName.LastIndexOf("."));
            return extension == ".png" || extension == ".bmp" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif";
        }

        /// <summary>
        /// 判断一个ip是否在另一个ip内
        /// </summary>
        /// <param name="sourceIP">检测ip</param>
        /// <param name="targetIP">匹配ip</param>
        /// <returns></returns>
        public static bool InIP(string sourceIP, string targetIP)
        {
            if (string.IsNullOrEmpty(sourceIP) || string.IsNullOrEmpty(targetIP))
                return false;

            string[] sourceIPBlockList = StringHelper.SplitString(sourceIP, @".");
            string[] targetIPBlockList = StringHelper.SplitString(targetIP, @".");

            int sourceIPBlockListLength = sourceIPBlockList.Length;

            for (int i = 0; i < sourceIPBlockListLength; i++)
            {
                if (targetIPBlockList[i] == "*")
                    return true;

                if (sourceIPBlockList[i] != targetIPBlockList[i])
                {
                    return false;
                }
                else
                {
                    if (i == 3)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断一个ip是否在另一个ip内
        /// </summary>
        /// <param name="sourceIP">检测ip</param>
        /// <param name="targetIPList">匹配ip列表</param>
        /// <returns></returns>
        public static bool InIPList(string sourceIP, string[] targetIPList)
        {
            if (targetIPList != null && targetIPList.Length > 0)
            {
                foreach (string targetIP in targetIPList)
                {
                    if (InIP(sourceIP, targetIP))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断一个ip是否在另一个ip内
        /// </summary>
        /// <param name="sourceIP">检测ip</param>
        /// <param name="targetIPStr">匹配ip</param>
        /// <returns></returns>
        public static bool InIPList(string sourceIP, string targetIPStr)
        {
            string[] targetIPList = StringHelper.SplitString(targetIPStr, "\n");
            return InIPList(sourceIP, targetIPList);
        }

        /// <summary>
        /// 判断当前时间是否在指定的时间段内
        /// </summary>
        /// <param name="periodList">指定时间段</param>
        /// <param name="liePeriod">所处时间段</param>
        /// <returns></returns>
        public static bool BetweenPeriod(string[] periodList, out string liePeriod)
        {
            if (periodList != null && periodList.Length > 0)
            {
                DateTime startTime;
                DateTime endTime;
                DateTime nowTime = DateTime.Now;
                DateTime nowDate = nowTime.Date;

                foreach (string period in periodList)
                {
                    int index = period.IndexOf("-");
                    startTime = TypeHelper.StringToDateTime(period.Substring(0, index));
                    endTime = TypeHelper.StringToDateTime(period.Substring(index + 1));

                    if (startTime < endTime)
                    {
                        if (nowTime > startTime && nowTime < endTime)
                        {
                            liePeriod = period;
                            return true;
                        }
                    }
                    else
                    {
                        if ((nowTime > startTime && nowTime < nowDate.AddDays(1)) || (nowTime < endTime))
                        {
                            liePeriod = period;
                            return true;
                        }
                    }
                }
            }
            liePeriod = string.Empty;
            return false;
        }

        /// <summary>
        /// 判断当前时间是否在指定的时间段内
        /// </summary>
        /// <param name="periodStr">指定时间段</param>
        /// <param name="liePeriod">所处时间段</param>
        /// <returns></returns>
        public static bool BetweenPeriod(string periodStr, out string liePeriod)
        {
            string[] periodList = StringHelper.SplitString(periodStr, "\n");
            return BetweenPeriod(periodList, out liePeriod);
        }

        /// <summary>
        /// 判断当前时间是否在指定的时间段内
        /// </summary>
        /// <param name="periodList">指定时间段</param>
        /// <returns></returns>
        public static bool BetweenPeriod(string periodList)
        {
            string liePeriod = string.Empty;
            return BetweenPeriod(periodList, out liePeriod);
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsNumericArray(string[] numericStrList)
        {
            if (numericStrList != null && numericStrList.Length > 0)
            {
                foreach (string numberStr in numericStrList)
                {
                    if (!IsNumeric(numberStr))
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsNumericRule(string numericRuleStr, string splitChar)
        {
            return IsNumericArray(StringHelper.SplitString(numericRuleStr, splitChar));
        }

        /// <summary>
        /// 是否是数值(包括整数和小数)
        /// </summary>
        public static bool IsNumericRule(string numericRuleStr)
        {
            return IsNumericRule(numericRuleStr, ",");
        }
    }
}
