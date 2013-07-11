using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTrading.Models;

namespace SYTrading.Controllers
{
    [Authorize]
    public class MaterialController : Controller
    {
        private BusinessDbContext db = new BusinessDbContext();

        //
        // GET: /Material/

        public ActionResult Index()
        {
            return View(db.Materials.ToList());
        }

        //
        // GET: /Material/Details/5

        public ActionResult Details(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // GET: /Material/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Material/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Materials.Add(material);
                db.SaveChanges();

                TempData.Add("ClearCache", ClearCacheController.ClearCache("Glove", null, null));

                return RedirectToAction("Index");
            }

            return View(material);
        }

        //
        // GET: /Material/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // POST: /Material/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();

                TempData.Add("ClearCache", ClearCacheController.ClearCache("Glove", null, null));

                return RedirectToAction("Index");
            }
            return View(material);
        }

        //
        // GET: /Material/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // POST: /Material/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
            db.SaveChanges();

            TempData.Add("ClearCache", ClearCacheController.ClearCache("Glove", null, null));

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}