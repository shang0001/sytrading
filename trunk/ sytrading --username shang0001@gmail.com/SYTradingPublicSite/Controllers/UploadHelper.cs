using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace SYTrading.Upload
{
    public class UploadHelper
    {
        public static string GenerateThumbnailString(string filename, int thumbWidth = 200)
        {
            Stream fileStream = File.Open(filename, FileMode.Open);
            string result = GenerateThumbnailString(fileStream);
            fileStream.Close();
            return result;
        }

        public static string GenerateThumbnailString(Stream fileStream, int thumbWidth = 200)
        {
            return EncodeFile(GenerateThumbnail(fileStream, thumbWidth));
        }

        private static string EncodeFile(byte[] inArray)
        {
            return Convert.ToBase64String(inArray);
        }

        private static byte[] GenerateThumbnail(Stream fileStream, int thumbWidth)
        {
            Graphics oEffect;

            System.Drawing.Image canvas = System.Drawing.Image.FromStream(fileStream);
            //int thumbWidth = 200;
            float srcWidth = canvas.Width;
            float srcHeight = canvas.Height;
            int thumbHeight = (int)((srcHeight / srcWidth) * thumbWidth);

            System.Drawing.Image otemp = new Bitmap(thumbWidth, thumbHeight);

            oEffect = Graphics.FromImage(otemp);
            oEffect.Clear(Color.Transparent);

            // oEffect.Clear(Color.Red);  you can see the back color through the image if you use anything other than .Transparent


            ColorMatrix cm = new ColorMatrix();
            cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = cm.Matrix44 = 1;
            cm.Matrix33 = (float)10 / 100; // The first value effects the transparent value ##/100

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            oEffect.DrawImage(canvas, new Rectangle(0, 0, thumbWidth, thumbHeight), 0, 0, canvas.Width, canvas.Height, GraphicsUnit.Pixel, ia);

            MemoryStream ms = new MemoryStream();
            otemp.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();











            //// Make thumbnail
            //int thumbWidth = 200;
            //System.Drawing.Image image = System.Drawing.Image.FromStream(fileStream);
            //float srcWidth = image.Width;
            //float srcHeight = image.Height;
            //int thumbHeight = (int)((srcHeight / srcWidth) * thumbWidth);
            //Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);

            //System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            //gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
            //gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);

            //MemoryStream ms = new MemoryStream();
            //bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            //bmp.Dispose();
            //image.Dispose();

            //return ms.ToArray();
        }
    }
}