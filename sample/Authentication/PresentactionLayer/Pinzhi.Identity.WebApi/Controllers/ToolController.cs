using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Linq;

namespace Pinzhi.Identity.WebApi.Controllers
{
    /// <summary>
    /// 工具控制器
    /// </summary>
    public class ToolController : Controller
    {
        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Tool/ValidateCode")]
        public IActionResult ValidateCode()
        {
            #region 反射SK支持的全部颜色
            //List<SKColor> colors = new List<SKColor>();           
            //var skcolors = new SKColors();
            //var type = skcolors.GetType();
            //foreach (FieldInfo field in type.GetFields())
            //{
            //    colors.Add( (SKColor)field.GetValue(skcolors));
            //}
            #endregion

            //int maxcolorindex = colors.Count-1;
            string text = "1A3V";
            var zu = text.ToList();
            SKBitmap bmp = new SKBitmap(80, 30);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                //背景色
                canvas.DrawColor(SKColors.White);

                using (SKPaint sKPaint = new SKPaint())
                {
                    sKPaint.TextSize = 16;//字体大小
                    sKPaint.IsAntialias = true;//开启抗锯齿                   
                    sKPaint.Typeface = SKTypeface.FromFamilyName("微软雅黑", SKTypefaceStyle.Bold);//字体
                    SKRect size = new SKRect();
                    sKPaint.MeasureText(zu[0].ToString(), ref size);//计算文字宽度以及高度

                    float temp = (bmp.Width / 4 - size.Size.Width) / 2;
                    float temp1 = bmp.Height - (bmp.Height - size.Size.Height) / 2;
                    Random random = new Random();

                    for (int i = 0; i < 4; i++)
                    {
                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawText(zu[i].ToString(), temp + 20 * i, temp1, sKPaint);//画文字
                    }
                    //干扰线
                    for (int i = 0; i < 5; i++)
                    {
                        sKPaint.Color = new SKColor((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
                        canvas.DrawLine(random.Next(0, 40), random.Next(1, 29), random.Next(41, 80), random.Next(1, 29), sKPaint);
                    }
                }
                //页面展示图片
                using (SKImage img = SKImage.FromBitmap(bmp))
                {
                    using (SKData p = img.Encode())
                    {
                        return File(p.ToArray(), "image/Png");
                    }
                }
            }
        }
    }
}