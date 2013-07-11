using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTradingPublicSite.Models;
using SYTradingPublicSite.ViewModels;

namespace SYTradingPublicSite.Controllers
{
    public class GloveController : Controller
    {
        private businessDbEntities db = new businessDbEntities();

        private string StorageRoot
        {
            get { return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Server.MapPath("~"))), "Gloves/"); }
        }

        [HttpGet]
        public void Download(string id, bool thumbnail = false)
        {
            var filename = id;
            var filePath = thumbnail ? Path.Combine(StorageRoot, "thumbnails/" + filename) : Path.Combine(StorageRoot, filename);

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
        [OutputCache(CacheProfile = "CacheGlove", VaryByParam = "selectedApplicationCategory;materialID", NoStore = true)]
        public ActionResult Index(string selectedApplicationCategory, string materialID)
        {
            GloveViewModel model = this.InitializeNavData();

            if (string.IsNullOrEmpty(selectedApplicationCategory) && string.IsNullOrEmpty(materialID))
            {
                selectedApplicationCategory = db.Applications.First().Category;
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
                    if (db.Gloves.Where(g => g.GloveID == id && g.Released == true).Count() > 0)
                    {
                        model.Gloves.Add(db.Gloves.Where(g => g.GloveID == id && g.Released == true).Single());
                    }
                }
                model.selectedApplicationCategory = selectedApplicationCategory;
            }
            else
            {
                int mid = int.Parse(materialID);
                if (db.Gloves.Where(g => g.MaterialID == mid && g.Released == true).Count() > 0)
                {
                    model.Gloves.AddRange(db.Gloves.Where(g => g.MaterialID == mid && g.Released == true));
                }
                model.selectedMaterialID = mid;
            }
            
            return View(model);
        }

        //
        // GET: /Glove/Details/5
        [OutputCache(CacheProfile = "CacheGlove", VaryByParam = "id", NoStore = true)]
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

            string selectedApplicationCategory = Request.Params["selectedApplicationCategory"];
            string materialID = Request.Params["selectedMaterialID"];

            if (!string.IsNullOrEmpty(selectedApplicationCategory))
            {
                int[] selectedGloveIDs = db.Applications.Include(a => a.GloveApplications)
                    .Where(a => a.Category == selectedApplicationCategory)
                    .SelectMany(a => a.GloveApplications)
                    .GroupBy(ga => ga.GloveID)
                    .Select(ga => ga.Key).ToArray();

                foreach (int gid in selectedGloveIDs)
                {
                    if (db.Gloves.Where(g => g.GloveID != id && g.GloveID == gid && g.Released == true).Count() > 0)
                    {
                        model.RelatedGloves.Add(db.Gloves.Where(g => g.GloveID != id && g.GloveID == gid && g.Released == true).Single());
                    }
                }
                model.selectedApplicationCategory = selectedApplicationCategory;
            }
            else
            {
                int mid = int.Parse(materialID);
                if (db.Gloves.Where(g => g.GloveID != id && g.MaterialID == mid && g.Released == true).Count() > 0)
                {
                    model.RelatedGloves.AddRange(db.Gloves.Where(g => g.GloveID != id && g.MaterialID == mid && g.Released == true));
                }
                model.selectedMaterialID = mid;
            }





            //foreach (GloveApplication gloveApplication in glove.GloveApplications)
            //{
            //    var anotherGAs = gloveApplication.Application.GloveApplications.Where(ga => ga.GloveApplicationID != gloveApplication.GloveApplicationID);

            //    if (anotherGAs.Count() == 0)
            //    {
            //        break;
            //    }

            //    Glove relatedGlove = db.Gloves.Find(anotherGAs.Where(g => g.Glove.Released == true).First().GloveID);

            //    if (!model.RelatedGloves.Contains(relatedGlove) && relatedGlove.ImagePaths.Count > 0)
            //    {
            //        model.RelatedGloves.Add(relatedGlove);
            //        model.ImageThumbnailPath.Add(relatedGlove.GloveID, relatedGlove.ImagePaths.First().Path + "?thumbnail=true");

            //        if (model.RelatedGloves.Count == 5)
            //        {
            //            break;
            //        }
            //    }
            //}

            return View(model);
        }

        public string ClearCache(int? id, bool? clearIndexOnly)
        {
            HttpResponse.RemoveOutputCacheItem("/Glove");

            if (id.HasValue)
            {
                HttpResponse.RemoveOutputCacheItem(string.Format("/Glove/Details/{0}", id));
            }
            else if (!(clearIndexOnly.HasValue && clearIndexOnly.Value == true))
            {
                foreach (var gid in db.Gloves.Select(g => g.GloveID))
                {
                    HttpResponse.RemoveOutputCacheItem(string.Format("/Glove/Details/{0}", gid));
                }
            }

            return "Clear!";
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

            model.ApplicationCategories = db.Applications.GroupBy(a => a.Category).Select(lst => lst.Key).ToArray();
            model.Materials = db.Materials.ToArray();

            return model;
        }
    }
}