using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.MegaMenu
{
    public class MegaMenuSettings : ISettings
    {
        #region product
        public bool Product { get; set; }         
        public int productIndex { get; set; }
        #endregion product


        #region category
        public bool Category { get; set; }         
        public int categoryIndex { get; set; }
        #endregion category

        #region manufacturer

        public bool Manufacturer { get; set; }         
        public int manufacturerIndex { get; set; }
        #endregion manufacturer

        public string SelectedProductsList { get; set; }

        public string Selected_categories { get; set; }
    }
}
