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
    public class ApplicationController : Controller
    {
        private BusinessDbContext db = new BusinessDbContext();

        //
        // GET: /Application/

        public ActionResult Index()
        {
            return View(db.Applications.ToList());
        }

        //
        // GET: /Application/Details/5

        public ActionResult Details(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // GET: /Application/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Application/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Application application)
        {
            if (ModelState.IsValid)
            {
                db.Applications.Add(application);
                db.SaveChanges();

                TempData.Remove("ClearCache");
                TempData.Add("ClearCache", ClearCacheController.ClearCache("Glove", null, null));

                return RedirectToAction("Index");
            }

            return View(application);
        }

        //
        // GET: /Application/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // POST: /Application/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();

                TempData.Remove("ClearCache");
                TempData.Add("ClearCache", ClearCacheController.ClearCache("Glove", null, null));

                return RedirectToAction("Index");
            }
            return View(application);
        }

        //
        // GET: /Application/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // POST: /Application/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();

            TempData.Remove("ClearCache");
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