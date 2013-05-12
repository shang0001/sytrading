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
        public static string GenerateThumbnailString(string filename, int thumbWidth = 50)
        {
            Stream fileStream = File.Open(filename, FileMode.Open);
            string result = GenerateThumbnailString(fileStream, thumbWidth);
            fileStream.Close();
            return result;
        }

        public static string GenerateThumbnailString(Stream fileStream, int thumbWidth = 50)
        {
            return EncodeFile(GenerateThumbnail(fileStream, thumbWidth));
        }

        public static byte[] GenerateThumbnailBytes(string filename, int thumbWidth = 50)
        {
            Stream fileStream = File.Open(filename, FileMode.Open);
            byte[] result = GenerateThumbnailBytes(fileStream, thumbWidth);
            fileStream.Close();
            return result;
        }

        public static byte[] GenerateThumbnailBytes(Stream fileStream, int thumbWidth = 50)
        {
            return GenerateThumbnail(fileStream, thumbWidth);
        }

        private static string EncodeFile(byte[] inArray)
        {
            return Convert.ToBase64String(inArray);
        }

        private static byte[] GenerateThumbnail(Stream fileStream, int thumbWidth)
        {
            // Make thumbnail
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
        }
    }
}