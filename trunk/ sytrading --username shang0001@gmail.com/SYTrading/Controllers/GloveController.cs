using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTrading.Models;
using SYTrading.ViewModels;

namespace SYTrading.Controllers
{
    [Authorize]
    public class GloveController : Controller
    {
        private BusinessDbContext db = new BusinessDbContext();

        //
        // GET: /Glove/

        public ActionResult Index()
        {
            var gloves = db.Gloves.Include(g => g.Material);
            return View(gloves.ToList());
        }

        //
        // GET: /Glove/Details/5

        public ActionResult Details(int id = 0)
        {
            Glove glove = db.Gloves.Find(id);
            if (glove == null)
            {
                return HttpNotFound();
            }
            return View(glove);
        }

        //
        // GET: /Glove/Create

        public ActionResult Create()
        {
            var viewModel = new GloveViewData();

            viewModel.MaterialID = new SelectList(db.Materials, "MaterialID", "Name");
            viewModel.Applications = db.Applications;
            return View(viewModel);
        }

        //
        // POST: /Glove/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GloveViewData gloveViewData)
        {
            if (ModelState.IsValid)
            {
                db.Gloves.Add(gloveViewData.Glove);
                db.SaveChanges();
                db.Database.Connection.Close();

                var createdGlove = db.Gloves
                    .Where(i => i.GloveID == gloveViewData.Glove.GloveID)
                    .Single();

                if (gloveViewData.selectedApplications == null)
                {
                    gloveViewData.Glove.GloveApplications = new List<GloveApplication>();
                }
                else
                {
                    var selectedApplicationsHS = new HashSet<string>(gloveViewData.selectedApplications);

                    foreach (var application in db.Applications)
                    {
                        if (selectedApplicationsHS.Contains(application.ApplicationID.ToString()))
                        {
                            db.GloveApplications.Add(new GloveApplication { GloveID = createdGlove.GloveID, ApplicationID = application.ApplicationID });
                        }
                    }
                }

                if (gloveViewData.imageFileNames != null)
                {
                    string[] fileNames = gloveViewData.imageFileNames.Split(new char[] { ';' });

                    foreach (string filename in fileNames)
                    {
                        if (!string.IsNullOrEmpty(filename))
                        {
                            // Save file to server
                            var sourceFullPath = StorageRoot + Path.GetFileName(filename);

                            var newFileName = createdGlove.GloveID + "_" + filename;
                            var destFullPath = DestinationRoot + Path.GetFileName(newFileName);

                            System.IO.File.Copy(sourceFullPath, destFullPath, true);

                            db.ImagePaths.Add(new ImagePath { GloveID = createdGlove.GloveID, Path = "/Glove/Download/" + newFileName, ThumbnailString = Upload.UploadHelper.GenerateThumbnailString(destFullPath) });
                        }
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", gloveViewData.Glove.MaterialID);
            return View(gloveViewData);
        }

        //
        // GET: /Glove/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var viewModel = new GloveViewData();

            Glove glove = db.Gloves
                            .Include(i => i.GloveApplications)
                            .Where(i => i.GloveID == id)
                            .Single();

            if (glove == null)
            {
                return HttpNotFound();
            }

            viewModel.Glove = glove;
            viewModel.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", glove.MaterialID);
            viewModel.Applications = db.Applications;
            return View(viewModel);
        }

        //
        // POST: /Glove/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GloveViewData gloveViewData)
        {
            if (ModelState.IsValid)
            {
                var currentGloveApplications = db.GloveApplications.Where(c => c.GloveID == gloveViewData.Glove.GloveID).ToArray();
                var selectedApplicationsHS = gloveViewData.selectedApplications==null ? new HashSet<string>() : new HashSet<string>(gloveViewData.selectedApplications);
                var currentApplicationIDs = new HashSet<int>(currentGloveApplications.Select(c => c.ApplicationID));

                foreach (var application in db.Applications)
                {
                    if (selectedApplicationsHS.Contains(application.ApplicationID.ToString()))
                    {
                        if (!currentApplicationIDs.Contains(application.ApplicationID))
                        {
                            db.GloveApplications.Add(new GloveApplication { GloveID = gloveViewData.Glove.GloveID, ApplicationID = application.ApplicationID });
                        }
                    }
                    else
                    {
                        if (currentApplicationIDs.Contains(application.ApplicationID))
                        {
                            GloveApplication gloveApplication = currentGloveApplications
                                                                    .Where(c => c.ApplicationID == application.ApplicationID)
                                                                    .Single();
                            db.GloveApplications.Remove(gloveApplication);
                        }
                    }
                }

                var currentImages = db.ImagePaths.Where(c => c.GloveID == gloveViewData.Glove.GloveID).ToArray();

                if (gloveViewData.imageFileNames != null)
                {
                    string[] fileNames = gloveViewData.imageFileNames.Split(new char[] { ';' });

                    foreach (string filename in fileNames)
                    {
                        if (!string.IsNullOrEmpty(filename) && currentImages.Where(c => Path.GetFileName(c.Path) == filename).Count() == 0)
                        {
                            // New file, save file to server
                            var sourceFullPath = StorageRoot + Path.GetFileName(filename);
                            var destFullPath = DestinationRoot + Path.GetFileName(filename);

                            // Save original file
                            System.IO.File.Copy(sourceFullPath, destFullPath, true);
                            
                            // Save 200px thumbnail image
                            byte[] thumbnailBuffer = Upload.UploadHelper.GenerateThumbnailBytes(destFullPath, 200);
                            System.IO.File.WriteAllBytes(Path.Combine(DestinationRoot, "thumbnails/" + filename), thumbnailBuffer);

                            db.ImagePaths.Add(new ImagePath { GloveID = gloveViewData.Glove.GloveID, Path = "/Glove/Download/" + filename, ThumbnailString = Upload.UploadHelper.GenerateThumbnailString(destFullPath) });
                        }
                    }

                    foreach (ImagePath img in currentImages)
                    {
                        if (fileNames.Where(c => c == Path.GetFileName(img.Path)).Count() == 0)
                        {
                            // Old file, remove file from server
                            var deleteFullPath = DestinationRoot + Path.GetFileName(img.Path);
                            var deletethumbnailFullPath = DestinationRoot + Path.Combine("thumbnails", Path.GetFileName(img.Path));
                            if (System.IO.File.Exists(deleteFullPath))
                            {
                                System.IO.File.Delete(deleteFullPath);
                                System.IO.File.Delete(deletethumbnailFullPath);
                            }
                            db.ImagePaths.Remove(img);
                        }
                    }
                }
                else
                {
                    foreach (ImagePath img in currentImages)
                    {
                        // No file, remove all files from server
                        var deleteFullPath = DestinationRoot + Path.GetFileName(img.Path);
                        var deletethumbnailFullPath = DestinationRoot + Path.Combine("thumbnails", Path.GetFileName(img.Path));
                        if (System.IO.File.Exists(deleteFullPath))
                        {
                            System.IO.File.Delete(deleteFullPath);
                            System.IO.File.Delete(deletethumbnailFullPath);
                        }

                        db.ImagePaths.Remove(img);
                    }
                }

                db.Set<Glove>().Attach(gloveViewData.Glove);
                db.Entry(gloveViewData.Glove).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", gloveViewData.Glove.MaterialID);
            return View(gloveViewData.Glove);
        }

        //
        // GET: /Glove/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Glove glove = db.Gloves.Find(id);
            if (glove == null)
            {
                return HttpNotFound();
            }
            return View(glove);
        }

        //
        // POST: /Glove/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Glove glove = db.Gloves.Include(i => i.Images)
                            .Where(i => i.GloveID == id)
                            .Single();
            foreach (ImagePath img in glove.Images)
            {
                // Remove all files from server
                var deleteFullPath = DestinationRoot + Path.GetFileName(img.Path);
                if (System.IO.File.Exists(deleteFullPath))
                {
                    System.IO.File.Delete(deleteFullPath);
                }
            }
            db.Gloves.Remove(glove);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(DestinationRoot, filename);

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

        //
        // POST: /Glove/Release/

        [HttpPost, ActionName("Release")]
        public ActionResult ReleaseConfirmed(int[] ids)
        {
            DbSet<Glove> gloves = db.Gloves;
            foreach (Glove glove in gloves)
            {
                if (ids.Contains(glove.GloveID))
                {
                    glove.Released = true;
                }
                else
                {
                    glove.Released = false;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GenerateThumbnails()
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(DestinationRoot);
            foreach (System.IO.FileInfo file in dir.GetFiles())
            {
                // Save 200px thumbnail image
                using(FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read)){
                    byte[] thumbnailBuffer = Upload.UploadHelper.GenerateThumbnailBytes(fileStream, 200);
                    System.IO.File.WriteAllBytes(Path.Combine(DestinationRoot, "thumbnails/" + file.Name), thumbnailBuffer);
                }
            }

            return RedirectToAction("Index");
        }

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Upload/UploadedFiles/")); }
        }

        private string DestinationRoot
        {
            get { return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath("~"))), "Gloves/"); }
        }
    }
}