using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SYTrading.Models;

namespace SYTrading.Controllers
{
    public class ClearCacheController : Controller
    {
        private static BusinessDbContext db = new BusinessDbContext();

        public static string ClearCache(string target, int? id, bool? clearIndexOnly)
        {
            WebClient client = new WebClient();

            // Add a user agent header in case the 
            // requested URI contains a query.

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            string baseUrl = string.Empty;
            if (System.Web.HttpContext.Current.IsDebuggingEnabled && db.Configs.Where(c => c.Name == "BaseWebUrl" && c.Level == "Debug").Count() > 0)
            {
                baseUrl = db.Configs.Where(c => c.Name == "BaseWebUrl" && c.Level == "Debug").Single().Value;
            }
            else if ((!System.Web.HttpContext.Current.IsDebuggingEnabled) && (db.Configs.Where(c => c.Name == "BaseWebUrl" && c.Level == "Release").Count() > 0))
            {
                baseUrl = db.Configs.Where(c => c.Name == "BaseWebUrl" && c.Level == "Release").Single().Value;
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrEmpty(baseUrl))
            {
                List<string> urls = new List<string>();
                switch (target)
                {
                    case "Glove":
                        if (id.HasValue)
                        {
                            // Clear for index and one glove detail page
                            urls.Add(baseUrl + "/Glove/clearCache/" + id);
                        }
                        else if (clearIndexOnly.HasValue && clearIndexOnly.Value == true)
                        {
                            // Clear for index ONLY
                            urls.Add(baseUrl + "/Glove/clearCache?clearIndexOnly=true");
                        }
                        else
                        {
                            // Clear for index and all gloves detail pages
                            urls.Add(baseUrl + "/Glove/clearCache");
                        }
                        break;
                    case "Home":
                        urls.Add(baseUrl + "/Home/clearCache");
                        break;
                    default:
                        urls.Add(baseUrl + "/Glove/clearCache");
                        urls.Add(baseUrl + "/Home/clearCache");
                        break;
                }

                foreach (string url in urls)
                {
                    Stream data = client.OpenRead(url);
                    StreamReader reader = new StreamReader(data);
                    result.AppendLine(string.Format("{0} : {1}", url, reader.ReadToEnd()));
                    data.Close();
                    reader.Close();
                }
            }
            else
            {
                result.AppendLine("No Base URL");
            }

            return result.ToString();
        }
    }
}