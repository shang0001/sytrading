using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SYTradingPublicSite.Models;

namespace SYTradingPublicSite.ViewModels
{
    public class GloveViewModel
    {
        public List<Glove> Gloves { get; set; }
        public List<Glove> RelatedGloves { get; set; }
        public Dictionary<int, string> ImageThumbnailStrings { get; set; }
        public Material[] Materials { get; set; }
        public string[] ApplicationCategories { get; set; }
        public string selectedApplicationCategory { get; set; }
        public int selectedMaterialID { get; set; }
    }
}