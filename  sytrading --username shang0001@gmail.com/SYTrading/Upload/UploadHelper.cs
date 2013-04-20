using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace SYTrading.Upload
{
    public class UploadHelper
    {
        public static string GenerateThumbnailString(string filename)
        {
            Stream fileStream = File.Open(filename, FileMode.Open);
            string result = GenerateThumbnailString(fileStream);
            fileStream.Close();
            return result;
        }

        public static string GenerateThumbnailString(Stream fileStream)
        {
            return EncodeFile(GenerateThumbnail(fileStream));
        }

        private static string EncodeFile(byte[] inArray)
        {
            return Convert.ToBase64String(inArray);
        }

        private static byte[] GenerateThumbnail(Stream fileStream)
        {
            // Make thumbnail
            int thumbWidth = 50;
            System.Drawing.Image image = System.Drawing.Image.FromStream(fileStream);
            float srcWidth = image.Width;
            float srcHeight = image.Height;
            int thumbHeight = (int)((srcHeight / srcWidth) * thumbWidth);
            Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);

            System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
            gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            bmp.Dispose();
            image.Dispose();

            return ms.ToArray();
        }
    }
}