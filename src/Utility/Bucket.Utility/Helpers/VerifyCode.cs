using System;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.IO;
using System.Security.Cryptography;

namespace Bucket.Utility.Helpers
{
    public class VerifyCode
    {
        #region 单例模式  
        //创建私有化静态obj锁    
        private static readonly object _ObjLock = new object();
        //创建私有静态字段，接收类的实例化对象    
        private static VerifyCode _VerifyCodeHelper = null;
        //构造函数私有化    
        private VerifyCode() { }
        //创建单利对象资源并返回    
        public static VerifyCode GetSingleObj()
        {
            if (_VerifyCodeHelper == null)
            {
                lock (_ObjLock)
                {
                    if (_VerifyCodeHelper == null)
                        _VerifyCodeHelper = new VerifyCode();
                }
            }
            return _VerifyCodeHelper;
        }
        #endregion

        #region 生产验证码  
        public enum VerifyCodeType { NumberVerifyCode, AbcVerifyCode, MixVerifyCode };

        /// <summary>  
        /// 1.数字验证码  
        /// </summary>  
        /// <param name="length"></param>  
        /// <returns></returns>  
        private string CreateNumberVerifyCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值    
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字    
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字    
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码    
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>  
        /// 2.字母验证码  
        /// </summary>  
        /// <param name="length">字符长度</param>  
        /// <returns>验证码字符</returns>  
        private string CreateAbcVerifyCode(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>  
        /// 3.混合验证码  
        /// </summary>  
        /// <param name="length">字符长度</param>  
        /// <returns>验证码字符</returns>  
        private string CreateMixVerifyCode(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>  
        /// 产生验证码（随机产生4-6位）  
        /// </summary>  
        /// <param name="type">验证码类型：数字，字符，符合</param>  
        /// <returns></returns>  
        public string CreateVerifyCode(VerifyCodeType type)
        {
            string verifyCode = string.Empty;
            Random random = new Random();
            int length = random.Next(4, 6);
            switch (type)
            {
                case VerifyCodeType.NumberVerifyCode:
                    verifyCode = GetSingleObj().CreateNumberVerifyCode(length);
                    break;
                case VerifyCodeType.AbcVerifyCode:
                    verifyCode = GetSingleObj().CreateAbcVerifyCode(length);
                    break;
                case VerifyCodeType.MixVerifyCode:
                    verifyCode = GetSingleObj().CreateMixVerifyCode(length);
                    break;
            }
            return verifyCode;
        }
        #endregion

        #region 验证码图片
        private static readonly byte[] randb = new byte[4];
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
        private FontFamily[] fontFamily ={
            new FontFamily("Times New Roman"),
            new FontFamily("Georgia"),
            new FontFamily("Arial"),
            new FontFamily("Comic Sans MS")
        };
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        private static int Next(int max)
        {
            rand.GetBytes(randb);
            int value = BitConverter.ToInt32(randb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }
        /// <summary>
        /// 获取 验证码图形 的 byte 数组对象
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes(Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            ms.Close();
            return bytes;
        }
        /// <summary>
        /// 字体随机颜色
        /// </summary>
        public Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(180);
            int int_Green = RandomNum_Sencond.Next(180);
            int int_Blue = (int_Red + int_Green > 300) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return Color.FromArgb(int_Red, int_Green, int_Blue);
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            double PI = 6.283185307179586476925286766559;
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            srcBmp.Dispose();
            return destBmp;
        }
        /// <summary>  
        /// 验证码图片 => Bitmap  
        /// </summary>  
        /// <param name="verifyCode">验证码</param>  
        /// <param name="width">宽</param>  
        /// <param name="height">高</param>  
        /// <returns>Bitmap</returns>  
        public Bitmap CreateBitmapByImgVerifyCode(string verifyCode, int width, int height)
        {
            var textLength = verifyCode.Length;
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            int fontSize = height / 2;
            g.Clear(Color.White);
            var color = GetRandomColor();
            // 干扰曲线
            var bezierLength = Next(2, 6);
            for (int i = 0; i < bezierLength; i++)
            {
                Point p1 = new Point(0, Next(bitmap.Height));
                Point p2 = new Point(Next(bitmap.Width), Next(bitmap.Height));
                Point p3 = new Point(Next(bitmap.Width), Next(bitmap.Height));
                Point p4 = new Point(bitmap.Width, Next(bitmap.Height));
                Point[] p = { p1, p2, p3, p4 };
                Brush newBrush = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), color, color);
                Pen pen = new Pen(newBrush);
                g.DrawBeziers(pen, p);
            }
            // 验证码
            int _x = -12, _y = 0;
            for (int int_index = 0; int_index < textLength; int_index++)
            {
                _x += fontSize + Next(-5, 5);
                _y = Next(-3, 3);
                string str_char = verifyCode[int_index].ToString();
                str_char = Next(1) == 1 ? str_char.ToLower() : str_char.ToUpper();
                Font font = new Font(fontFamily[Next(fontFamily.Length - 1)], fontSize + Next(-2, 2), (FontStyle.Bold | FontStyle.Italic));
                Brush newBrush = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), color, color);
                Point thePos = new Point(_x, _y);
                g.DrawString(str_char, font, newBrush, thePos);
                font.Dispose();
            }
            // 噪点
            for (int i = 0; i < 100; i++)
            {
                int x = Next(bitmap.Width - 1);
                int y = Next(bitmap.Height - 1);
                bitmap.SetPixel(x, y, color);
            }
            bitmap = TwistImage(bitmap, true, Next(2, 3), Next(4, 6));
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, bitmap.Width - 1, (bitmap.Height - 1));
            return bitmap;
        }

        /// <summary>  
        /// 验证码图片 => byte[]  
        /// </summary>  
        /// <param name="verifyCode">验证码</param>  
        /// <param name="width">宽</param>  
        /// <param name="height">高</param>  
        /// <returns>byte[]</returns>  
        public byte[] CreateByteByImgVerifyCode(string verifyCode, int width, int height)
        {
            var bitmap = CreateBitmapByImgVerifyCode(verifyCode, width, height);
            //输出图片流    
            return GetBytes(bitmap);
        }
        #endregion
    }
}
