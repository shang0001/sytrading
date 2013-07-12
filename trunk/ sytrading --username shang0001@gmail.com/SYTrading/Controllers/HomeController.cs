using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SYTrading.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the website administration console.";

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

            return View();
        }

        public ActionResult ClearCache()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(ClearCacheController.ClearCache("Home", null, null));
            result.AppendLine(ClearCacheController.ClearCache("Glove", null, null));

            TempData.Remove("ClearCache");
            TempData.Add("ClearCache", result);

            return RedirectToAction("Index");
        }
    }
}
