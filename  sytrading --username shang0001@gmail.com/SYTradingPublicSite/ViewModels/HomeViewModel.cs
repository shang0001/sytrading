using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SYTradingPublicSite.Models;

namespace SYTradingPublicSite.ViewModels
{
    public class HomeViewModel
    {
        public Customer Customer { get; set; }
        public Message Message { get; set; }
        public SelectList Titles { get; set; }
        public SelectList Countries { get; set; }
    }
}