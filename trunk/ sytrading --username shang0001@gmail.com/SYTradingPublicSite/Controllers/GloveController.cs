using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTrading.Upload;
using SYTradingPublicSite.Models;
using SYTradingPublicSite.ViewModels;

namespace SYTradingPublicSite.Controllers
{
    public class GloveController : Controller
    {
        private businessDbEntities db = new businessDbEntities();

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Gloves/")); }
        }

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

        //
        // GET: /Glove/

        public ActionResult Index(string selectedApplicationCategory, string materialID)
        {
            GloveViewModel model = this.InitializeNavData();

            if (string.IsNullOrEmpty(selectedApplicationCategory) && string.IsNullOrEmpty(materialID))
            {
                selectedApplicationCategory = "dasdsa";
            }

            if (!string.IsNullOrEmpty(selectedApplicationCategory))
            {
                int[] selectedGloveIDs = db.Applications.Include(a => a.GloveApplications)
                    .Where(a => a.Category == selectedApplicationCategory)
                    .SelectMany(a => a.GloveApplications)
                    .GroupBy(ga => ga.GloveID)
                    .Select(ga => ga.Key).ToArray();

                foreach (int id in selectedGloveIDs)
                {
                    model.Gloves.Add(db.Gloves.Find(id));
                }
                model.selectedApplicationCategory = selectedApplicationCategory;
            }
            else
            {
                int mid = int.Parse(materialID);
                model.Gloves.AddRange(db.Gloves.Where(g => g.MaterialID == mid));
                model.selectedMaterialID = mid;
            }

            foreach (Glove glove in model.Gloves)
            {
                if (glove.ImagePaths.Count > 0)
                {
                    string filename = Path.GetFileName(glove.ImagePaths.First().Path);
                    model.ImageThumbnailStrings.Add(glove.GloveID, @"data:image/png;base64," + UploadHelper.GenerateThumbnailString(Path.Combine(StorageRoot, filename)));
                }
            }
            
            return View(model);
        }

        //
        // GET: /Glove/Details/5

        public ActionResult Details(int id = 0)
        {
            GloveViewModel model = this.InitializeNavData();

            Glove glove = db.Gloves
                            .Include(g => g.ImagePaths)
                            .Include(g => g.GloveApplications)
                            .Where(g => g.GloveID == id)
                            .Single();

            if (glove == null)
            {
                return HttpNotFound();
            }

            model.Gloves.Add(glove);

            foreach (GloveApplication gloveApplication in glove.GloveApplications)
            {
                var anotherGAs = gloveApplication.Application.GloveApplications.Where(ga => ga.GloveApplicationID != gloveApplication.GloveApplicationID);

                if (anotherGAs.Count() == 0)
                {
                    break;
                }

                Glove relatedGlove = db.Gloves.Find(anotherGAs.First().GloveID);

                if (!model.RelatedGloves.Contains(relatedGlove) && relatedGlove.ImagePaths.Count > 0)
                {
                    model.RelatedGloves.Add(relatedGlove);

                    string filename = Path.GetFileName(relatedGlove.ImagePaths.First().Path);
                    model.ImageThumbnailStrings.Add(relatedGlove.GloveID, @"data:image/png;base64," + UploadHelper.GenerateThumbnailString(Path.Combine(StorageRoot, filename)));

                    if (model.RelatedGloves.Count == 5)
                    {
                        break;
                    }
                }
            }

            return View(model);
        }

        //
        // GET: /Glove/Create

        public ActionResult Create()
        {
            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name");
            return View();
        }

        //
        // POST: /Glove/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Glove glove)
        {
            if (ModelState.IsValid)
            {
                db.Gloves.Add(glove);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", glove.MaterialID);
            return View(glove);
        }

        //
        // GET: /Glove/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Glove glove = db.Gloves.Find(id);
            if (glove == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", glove.MaterialID);
            return View(glove);
        }

        //
        // POST: /Glove/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Glove glove)
        {
            if (ModelState.IsValid)
            {
                db.Entry(glove).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaterialID = new SelectList(db.Materials, "MaterialID", "Name", glove.MaterialID);
            return View(glove);
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
            Glove glove = db.Gloves.Find(id);
            db.Gloves.Remove(glove);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private GloveViewModel InitializeNavData()
        {
            GloveViewModel model = new GloveViewModel();
            model.Gloves = new List<Glove>();
            model.RelatedGloves = new List<Glove>();
            model.ImageThumbnailStrings = new Dictionary<int, string>();

            model.ApplicationCategories = db.Applications.GroupBy(a => a.Category).Select(lst => lst.Key).ToArray();
            model.Materials = db.Materials.ToArray();

            return model;
        }
    }
}