using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;

namespace SYTrading.Controllers
{
    public class UploadController : Controller
    {
        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(StorageRoot, filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        [HttpPost]
        public ActionResult Upload(int id)
        {
            System.Threading.Thread.Sleep(3000);

            var statuses = new List<ViewDataUploadFilesResult>();

            // Loop through each file in the request
            for (int i = 0; i < HttpContext.Request.Files.Count; i++)
            {
                // Pointer to file
                var file = HttpContext.Request.Files[i];

                // Get new file name. If id == -1, it is Create view, no change; else, add the Glove_ID to rename the file
                var newFileName = id == -1 ? Path.GetFileName(file.FileName) : id + "_" + Path.GetFileName(file.FileName);

                // Save file to server
                var fullPath = StorageRoot + newFileName;
                file.SaveAs(fullPath);

                // Add to statuses
                string fullName = Path.GetFileName(file.FileName);
                statuses.Add(new ViewDataUploadFilesResult()
                {
                    name = newFileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    url = "/Upload/Download/" + newFileName,
                    title = Path.GetFileNameWithoutExtension(newFileName).Substring(Path.GetFileNameWithoutExtension(newFileName).LastIndexOf("_") + 1),
                    delete_url = "/Home/Delete/" + newFileName,
                    thumbnail_url = @"data:image/png;base64," + SYTrading.Upload.UploadHelper.GenerateThumbnailString(file.InputStream),
                    delete_type = "GET",
                });
            }

            // Return JSON
            return Json(statuses, JsonRequestBehavior.AllowGet);
        }

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Upload/UploadedFiles/")); }
        }

        //private string EncodeFile(string fileName)
        //{
        //    return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        //}

        //private string EncodeFile(byte[] inArray)
        //{
        //    return Convert.ToBase64String(inArray);
        //}

        private string GenerateHash(byte[] byteArray)
        {
            string hash;
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(byteArray));
            }
            return hash;
        }

        //private byte[] GenerateThumbnail(Stream inputStream)
        //{
        //    // Make thumbnail
        //    int thumbWidth = 50;
        //    System.Drawing.Image image = System.Drawing.Image.FromStream(inputStream);
        //    float srcWidth = image.Width;
        //    float srcHeight = image.Height;
        //    int thumbHeight = (int)((srcHeight / srcWidth) * thumbWidth);
        //    Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);

        //    System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
        //    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        //    System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
        //    gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);

        //    MemoryStream ms = new MemoryStream();
        //    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

        //    bmp.Dispose();
        //    image.Dispose();

        //    return ms.ToArray();
        //}

        public class ViewDataUploadFilesResult
        {
            public string name { get; set; }
            public int size { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string title { get; set; }
            public string delete_url { get; set; }
            public string thumbnail_url { get; set; }
            public string delete_type { get; set; }
        }
    }
}
