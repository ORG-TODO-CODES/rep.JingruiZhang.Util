using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JingruiZhang.Util
{
    /// <summary>
    /// Image 帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 获取 Image 对象的 MD5
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string GetImageMD5(Image img)
        {
            MD5 md5 = MD5.Create();
            byte[] fileHashBytes;
            using (MemoryStream msw = new MemoryStream())
            {
                img.Save(msw, ImageFormat.Png);
                fileHashBytes = md5.ComputeHash(msw.ToArray());
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fileHashBytes.Length; i++)
            {
                sb.Append(fileHashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Image 转为 base64编码的文本
        /// </summary>
        /// <param name="bmp">待转的Bitmap</param>
        /// <returns>转换后的base64字符串</returns>
        public static string ImageToBase64String(Image imageData, ImageFormat format)
        {
            string base64;
            MemoryStream memory = new MemoryStream();
            imageData.Save(memory, format);
            base64 = System.Convert.ToBase64String(memory.ToArray());
            memory.Close();
            memory = null;
            return base64;
        }

        /// <summary>
        /// 按照指定的高和宽生成缩略图
        /// </summary>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="imageFrom">原图片</param>
        /// <returns>返回新生成的图</returns>
        public static Image Idispose_BigToSmallImage(int width, int height, Image imageFrom, InterpolationMode mode = InterpolationMode.Low
            , SmoothingMode smode = SmoothingMode.HighSpeed)
        {
            // 源图宽度及高度 
            int imageFromWidth = imageFrom.Width;
            int imageFromHeight = imageFrom.Height;

            // 生成的缩略图实际宽度及高度.如果指定的高和宽比原图大，则返回原图；否则按照指定高宽生成图片
            if (width >= imageFromWidth && height >= imageFromHeight)
            {
                return imageFrom;
            }
            else
            {
                // 生成的缩略图在上述"画布"上的位置
                int X = 0;
                int Y = 0;

                // 创建画布
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                bmp.SetResolution(imageFrom.HorizontalResolution, imageFrom.VerticalResolution);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // 用白色清空 
                    g.Clear(Color.White);

                    // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
                    g.InterpolationMode = mode;//InterpolationMode.HighQualityBicubic;

                    // 指定高质量、低速度呈现。 
                    g.SmoothingMode = smode;//SmoothingMode.HighQuality;

                    // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
                    g.DrawImage(imageFrom, new Rectangle(X, Y, width, height),
                        new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);

                    return bmp;
                }
            }
        }
    }
}
