using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MegaMenu.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public bool Product { get; set; }         
        public int productIndex { get; set; }
        public string product_html { get; set; }

        public bool Category { get; set; }         
        public int categoryIndex { get; set; }
        public string category_html { get; set; }

        public bool Manufacturer { get; set; }         
        public int manufacturerIndex { get; set; }
        public string manufacturer_html { get; set; }

        public TopMenuModel topMenuModel { get; set; }

        public IList<CategorySimpleModel> categorySimpleModelList { get; set; }

        public IList<CategoryModel> categorylist { get; set; }
         
        public IList<Product> productList { get; set; }      
        
        public IEnumerable<ProductOverviewModel> productOverviewModel { get; set; }

        public IList<ManufacturerModel> ManufacturerModelIList { get; set; }

        public string SelectedProductsList { get; set; }
    }
}
