using System;
using System.Collections.Generic;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Bucket.ImgVerifyCode
{
    public static class VerifyCode
    {
        #region 生产验证码  
        public enum VerifyCodeType { NumberVerifyCode, AbcVerifyCode, MixVerifyCode, ChineseVerifyCode };

        /// <summary>  
        /// 1.数字验证码  
        /// </summary>  
        /// <param name="length"></param>  
        /// <returns></returns>  
        private static string CreateNumberVerifyCode(int length)
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
        private static string CreateAbcVerifyCode(int length)
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
        private static string CreateMixVerifyCode(int length)
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
        /// 4.混合验证码  
        /// </summary>  
        /// <param name="length">字符长度</param>  
        /// <returns>验证码字符</returns>  
        private static string CreateChineseVerifyCode(int length)
        {
            string verification = string.Empty;
            string[] dictionary = @"的,一,了,是,我,不,在,人,们,有,来,他,这,上,着,个,地,到,大,里,说,去,子,得,也,和,那,要,下,看,天,时,过,出,小,么,起,你,都,把,好,还,多,没,为,又,可,家,学,只,以,主,会,样,年,想,能,生,同,老,中,从,自,面,前,头,到,它,后,然,走,很,像,见,两,用,她,国,动,进,成,回,什,边,作,对,开,而,已,些,现,山,民,候,经,发,工,向,事,命,给,长,水,几,义,三,声,于,高,正,妈,手,知,理,眼,志,点,心,战,二,问,但,身,方,实,吃,做,叫,当,住,听,革,打,呢,真,党,全,才,四,已,所,敌,之,最,光,产,情,路,分,总,条,白,话,东,席,次,亲,如,被,花,口,放,儿,常,西,气,五,第,使,写,军,吧,文,运,在,果,怎,定,许,快,明,行,因,别,飞,外,树,物,活,部,门,无,往,船,望,新,带,队,先,力,完,间,却,站,代,员,机,更,九,您,每,风,级,跟,笑,啊,孩,万,少,直,意,夜,比,阶,连,车,重,便,斗,马,哪,化,太,指,变,社,似,士,者,干,石,满,决,百,原,拿,群,究,各,六,本,思,解,立,河,爸,村,八,难,早,论,吗,根,共,让,相,研,今,其,书,坐,接,应,关,信,觉,死,步,反,处,记,将,千,找,争,领,或,师,结,块,跑,谁,草,越,字,加,脚,紧,爱,等,习,阵,怕,月,青,半,火,法,题,建,赶,位,唱,海,七,女,任,件,感,准,张,团,屋,爷,离,色,脸,片,科,倒,睛,利,世,病,刚,且,由,送,切,星,晚,表,够,整,认,响,雪,流,未,场,该,并,底,深,刻,平,伟,忙,提,确,近,亮,轻,讲,农,古,黑,告,界,拉,名,呀,土,清,阳,照,办,史,改,历,转,画,造,嘴,此,治,北,必,服,雨,穿,父,内,识,验,传,业,菜,爬,睡,兴,客".Split(new Char[] { ',' });
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification = verification + dictionary[random.Next(dictionary.Length - 1)];
            }
            return verification;
        }
        /// <summary>  
        /// 产生验证码（随机产生4-6位）  
        /// </summary>  
        /// <param name="type">验证码类型：数字，字符，符合</param>  
        /// <returns></returns>  
        public static string CreateVerifyCode(VerifyCodeType type, int length = 4)
        {
            string verifyCode = string.Empty;
            switch (type)
            {
                case VerifyCodeType.NumberVerifyCode:
                    verifyCode = CreateNumberVerifyCode(length);
                    break;
                case VerifyCodeType.AbcVerifyCode:
                    verifyCode = CreateAbcVerifyCode(length);
                    break;
                case VerifyCodeType.MixVerifyCode:
                    verifyCode = CreateMixVerifyCode(length);
                    break;
                case VerifyCodeType.ChineseVerifyCode:
                    verifyCode = CreateChineseVerifyCode(length);
                    break;
            }
            return verifyCode;
        }
        #endregion

        //#region 验证码图片
        private static readonly byte[] randb = new byte[4];
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
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

        private static List<SKColor> colors = new List<SKColor>()
        {
            new SKColor(205,104,0),
            new SKColor(151,155,22),
            new SKColor(108,163,44),
            new SKColor(121,85,72),
            new SKColor(96,125,139),
            new SKColor(76,175,80),
            new SKColor(0,150,136),
            new SKColor(4,167,187),
            new SKColor(33,150,243),
            new SKColor(63,81,181)
        };
        public static byte[] CreateByteByImgVerifyCode(string verifyCode, int width, int height)
        {
            byte[] bytes;
            var text = verifyCode.ToUpper().ToList();
            SKBitmap bmp = new SKBitmap(width, height);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                // 背景色
                canvas.DrawColor(SKColors.White);

                using (SKPaint sKPaint = new SKPaint())
                {
                    sKPaint.TextSize = 18; // 字体大小
                    sKPaint.FakeBoldText = true;
                    sKPaint.IsAntialias = true; // 开启抗锯齿 
                    sKPaint.Typeface = SKTypeface.FromFamilyName("WenQuanYi Micro Hei", SKTypefaceStyle.Normal);//字体

                    SKRect size = new SKRect();
                    sKPaint.MeasureText(text[0].ToString(), ref size); // 计算文字宽度以及高度

                    float _x = (width - size.Width * text.Count) / 2 - size.Width;
                    float _y = size.Height;
                    int num = Next(0, 9);
                    sKPaint.Color = colors[num];
                    // 干扰线
                    for (int i = 0; i < 3; i++)
                    {
                        canvas.DrawLine(Next(0, 40), Next(1, 29), Next(41, 80), Next(1, 29), sKPaint);
                    }
                    // 文字
                    for (int i = 0; i < text.Count; i++)
                    {
                        _x += size.Width + Next(0, 3);
                        _y = size.Height + Next(5, 15);
                        canvas.DrawText(text[i].ToString(), _x , _y, sKPaint); // 画文字
                    }
                }
                // 页面展示图片
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (SKData p = img.Encode())
                    {
                        bytes = p.ToArray();
                    }
                }
            }
            return bytes;
        }
    }
}