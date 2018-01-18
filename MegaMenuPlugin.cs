using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.MegaMenu
{
    public class MegaMenuPlugin : BasePlugin, IWidgetPlugin  
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public MegaMenuPlugin(IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> {  "header_menu_before" };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MegaMenu";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.MegaMenu.Controllers" }, { "area", null } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "MegaMenu";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.MegaMenu.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public override void Install()
        {
            var settings = new MegaMenuSettings
            {
                Product = true,                 
                productIndex = 1,

                Category = true,                 
                categoryIndex = 2,

                Manufacturer = true,                 
                manufacturerIndex = 3
            };

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Product", "Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Product.Hint", "Check all the Products which you want to show in the MegaMenu DropDown in Products");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Category", "Category.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Category.Hint", "Select all the Category which you want to show in the MegaMenu DropDown in Products.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Manufacturer", "Manufacturer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.MegaMenu.Manufacturer.Hint", "Manufacturer.");                         

            _settingService.SaveSetting(settings);  

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<MegaMenuSettings>();

            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Product");
            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Product.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Category");
            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Category.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Manufacturer");
            this.DeletePluginLocaleResource("Plugins.Widgets.MegaMenu.Manufacturer.Hint");
            //locales                      

            base.Uninstall();
        }
       
    }
}
