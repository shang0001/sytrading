using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTradingPublicSite.Models;
using SYTradingPublicSite.ViewModels;

namespace SYTradingPublicSite.Controllers
{
    public class HomeController : Controller
    {
        private businessDbEntities db = new businessDbEntities();

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/Images/banner/Home/")); }
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

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            ViewBag.BannerImages = Directory.EnumerateFiles(this.StorageRoot)
                                        .Select(p => Path.GetFileName(p)).ToArray();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.Create = true;

            var viewModel = new HomeViewModel();
            viewModel.Titles = new SelectList(new string[] {"Mr.", "Ms.", "Mrs.", "Miss"});

            return View(viewModel);
        }

        //
        // POST: /Customer/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(model.Customer);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(model.Message.Subject) || !string.IsNullOrEmpty(model.Message.Body))
                {
                    model.Message.CustomerID = model.Customer.CustomerID;
                    db.Messages.Add(model.Message);
                    db.SaveChanges();
                }

                ViewBag.Create = false;
            }

            return View();
        }
    }
}
