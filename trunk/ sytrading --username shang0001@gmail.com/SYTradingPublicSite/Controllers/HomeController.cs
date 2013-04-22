using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

                if (!string.IsNullOrEmpty(model.Message.Subject) || !string.IsNullOrEmpty(model.Message.Body))
                {
                    SendEmail(model.Customer.Email, (model.Customer.FirstName + " "  + model.Customer.LastName), model.Message.Subject, model.Message.Body);
                }
            }

            return View();
        }

        private void SendEmail(string sender, string displayName, string subject, string body)
        {
            try
            {
                Dictionary<string, string> dictionary =
                    db.Configs.Where(c => c.Enabled == true).ToDictionary(p => p.Name, c => c.Value);

                MailMessage mailMsg = new MailMessage();
                if (dictionary.ContainsKey("admin_email"))
                {
                    mailMsg.To.Add(dictionary["admin_email"]);
                }
                else
                {
                    throw new KeyNotFoundException("Email settings key \"admin_email\" is not found, please check immediately!");
                }

                // From
                MailAddress mailAddress = new MailAddress(dictionary["smtp_server_username"]);
                mailMsg.From = mailAddress;

                // Subject and Body
                mailMsg.Subject = "New inquery";
                mailMsg.Body = string.Format(
                    "There is a new inquery to Shang Yue Trading Ltd:\n\n Customer: {0}\n Email: {1}\n Subject:{2}\n Body:{3}",
                    displayName,
                    sender,
                    subject,
                    body);
                //mailMsg.IsBodyHtml = true;

                SmtpClient smtpClient = null;
                // Init SmtpClient and send on port 587 in my case. (Usual=port25)
                if (dictionary.ContainsKey("smtp_server") && dictionary.ContainsKey("smtp_server_port"))
                {
                    smtpClient = new SmtpClient(
                        dictionary["smtp_server"],
                        int.Parse(dictionary["smtp_server_port"]));
                }
                else
                {
                    throw new KeyNotFoundException("Email settings key \"smtp_server\" or \"smtp_server_port\" is not found, please check immediately!");
                }

                if (dictionary.ContainsKey("smtp_server_defaultCredential"))
                {
                    if (bool.Parse(dictionary["smtp_server_defaultCredential"]) == true)
                    {
                        smtpClient.UseDefaultCredentials = true;
                    }
                    else
                    {
                        if (dictionary.ContainsKey("smtp_server_username") && dictionary.ContainsKey("smtp_server_password"))
                        {
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(
                                dictionary["smtp_server_username"],
                                dictionary["smtp_server_password"]);
                            smtpClient.Credentials = credentials;
                        }
                        else
                        {
                            throw new KeyNotFoundException("Email settings key \"smtp_server_username\" or \"smtp_server_password\" is not found, please check immediately!");
                        }
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Email settings key \"smtp_server_defaultCredential\" is not found, please check immediately!");
                }

                if (dictionary.ContainsKey("smtp_server_ssl"))
                {
                    if (bool.Parse(dictionary["smtp_server_ssl"]) == true)
                    {
                        smtpClient.EnableSsl = true;
                    }
                    else
                    {
                        smtpClient.EnableSsl = false;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Email settings key \"smtp_server_ssl\" is not found, please check immediately!");
                }

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
