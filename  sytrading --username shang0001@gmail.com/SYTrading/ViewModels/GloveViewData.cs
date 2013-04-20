using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTrading.Models;

namespace SYTrading.ViewModels
{
    public class GloveViewData
    {
        public Glove Glove { get; set; }
        public SelectList MaterialID { get; set; }
        public IEnumerable<Application> Applications { get; set; }
        public string[] selectedApplications { get; set; }
        public string imageFileNames { get; set; }
    }
}