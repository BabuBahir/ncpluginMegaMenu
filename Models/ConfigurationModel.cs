using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MegaMenu.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Widgets.MegaMenu.Product")]
        public bool Product { get; set; }

        

        [NopResourceDisplayName("Plugins.Widgets.MegaMenu.Category")]
        public bool Category { get; set; }

      

        [NopResourceDisplayName("Plugins.Widgets.MegaMenu.Manufacturer")]
        public bool Manufacturer { get; set; }

      
        public string topMenuCategoriesJson { get; set; }

        public string AllCategoriesJson { get; set; }

        public string DisplayorderJson { get; set; }

        public string outputAllCategories { get; set; }
        public class multiselecteModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string imgurl { get; set; }
            public bool ticked { get; set; }

        }
    }
}
