using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.MegaMenu.Models;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.MegaMenu.Services;
using Nop.Services.Vendors;
using static Nop.Admin.Models.Catalog.CategoryModel;
using System;

namespace Nop.Plugin.Widgets.MegaMenu.Controllers
{
    public class MegaMenuController : BasePublicController  
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IVendorService _vendorService;
        public MegaMenuServices _megaMenuServices;
        public MegaMenuController(ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            IProductModelFactory  productModelFactory,
            IProductService  productService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService ,
            IManufacturerService manufacturerService,
            IVendorService vendorService)
        {
            this._catalogModelFactory = catalogModelFactory;
            this._categoryService = categoryService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._vendorService = vendorService;
            _megaMenuServices = new MegaMenuServices(catalogModelFactory, categoryService ,settingService , pictureService);
        }

        protected string GetPictureUrl(int pictureId)
        {
            string cacheKey = string.Format(Infrastructure.Cache.ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
                //little hack here. nulls aren't cacheable so set it to ""
                if (url == null)
                    url = "";

                return url;
            });
        }

        #region Configure
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(storeScope);

            var model = new ConfigurationModel();            
              
           //never Use presviously saved value
           var topMenuModel  = _catalogModelFactory.PrepareTopMenuModel();

            var AllCategoriesList_Paged = _categoryService.GetAllCategories();             

            // getting all the available top menu lists
            model.topMenuCategoriesJson = _megaMenuServices.ConvertcategorylisttoJson((List<CategorySimpleModel>)topMenuModel.Categories , megaMenuSettings);
            model.AllCategoriesJson = _megaMenuServices.Convert_PagedList_to_categorywith_id_name_pictureUrl_Array_for_multiselect(AllCategoriesList_Paged).ToString();

            model.Product       = megaMenuSettings.Product;             
            model.Category      = megaMenuSettings.Category;             
            model.Manufacturer  = megaMenuSettings.Manufacturer;             

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                //model.Picture1Id_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x, storeScope);
                //model.Text1_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.Text1, storeScope);
                //model.Link1_OverrideForStore = _settingService.SettingExists(megaMenuSettings, x => x.Link1, storeScope);               
            }
             
            return View("~/Plugins/Widgets.MegaMenu/Views/Configure.cshtml", model);
        }

       
        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(storeScope);             

            megaMenuSettings.Product = model.Product;                        
            megaMenuSettings.Category = model.Category;               
            megaMenuSettings.Manufacturer = model.Manufacturer;             

            //get all categories to be displayed i.e. outputAllCategories
            string Selected_categories = _megaMenuServices.get_Selected_categories_fromJson(model.outputAllCategories);
            megaMenuSettings.Selected_categories = Selected_categories;
            
            //update category table display order
            _megaMenuServices.AlterCategoryDisplayOrder(model.DisplayorderJson);

            // incase of blank response...happends when user clicks the submit button too fast !!!

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSetting(megaMenuSettings, x => x.Product, storeScope, false);             
            _settingService.SaveSetting(megaMenuSettings, x => x.Category, storeScope, false);             
            _settingService.SaveSetting(megaMenuSettings, x => x.Manufacturer, storeScope, false);             

            _settingService.SaveSetting(megaMenuSettings, x => x.Selected_categories, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
        

        #region popup Product

        public PartialViewResult addProductsPopup()
        {
            PublicInfoModel publicInfoModel = new PublicInfoModel();
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(0);

            publicInfoModel.SelectedProductsList = megaMenuSettings.SelectedProductsList;
            return PartialView("~/Plugins/Widgets.MegaMenu/Views/Partial_addProductsPopUp.cshtml" , publicInfoModel); ;
        }

        [HttpGet]
        public JsonResult get_config_product_count()
        {
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(0);
            int[] SelectedProductsList = _megaMenuServices.Int_array_from_Strin_array(megaMenuSettings.SelectedProductsList);
            string jsondata = Convert.ToString(SelectedProductsList.Length);

            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        [FormValueRequired("popup_submit_Product")]
        public ActionResult Configure(AddCategoryProductModel model)
        {
            MegaMenuSettings megaMenuSettings = new MegaMenuSettings();
            megaMenuSettings.SelectedProductsList = _megaMenuServices.create_CSV_From_selectedIdsArray(model.SelectedProductIds);
            _settingService.SaveSetting(megaMenuSettings, x => x.SelectedProductsList, 0, false);

            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return View("~/Plugins/Widgets.MegaMenu/Views/close.cshtml");
        }

        #endregion popup Product
         
        #endregion Configure

        #region publicInfo
        [ChildActionOnly]         
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(_storeContext.CurrentStore.Id);

            PublicInfoModel publicInfoModel = new PublicInfoModel();
            publicInfoModel = _generateNewPublicInfoModel(publicInfoModel);

            publicInfoModel.product_html = _megaMenuServices.RenderPartialToString("~/Plugins/Widgets.MegaMenu/Views/partial/PublicInfo/_ProductPartial.cshtml", publicInfoModel, this.ControllerContext);
            publicInfoModel.category_html = _megaMenuServices.RenderPartialToString("~/Plugins/Widgets.MegaMenu/Views/partial/PublicInfo/_CategoryPartial.cshtml", publicInfoModel, this.ControllerContext);
            publicInfoModel.manufacturer_html = _megaMenuServices.RenderPartialToString("~/Plugins/Widgets.MegaMenu/Views/partial/PublicInfo/_ManufacturerPartial.cshtml", publicInfoModel, this.ControllerContext);


            return View("~/Plugins/Widgets.MegaMenu/Views/PublicInfo.cshtml", publicInfoModel);
        }

        private PublicInfoModel _generateNewPublicInfoModel(PublicInfoModel model)
        {
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(_storeContext.CurrentStore.Id);
            #region products
            var productList = _productService.GetAllProductsDisplayedOnHomePage();
            var SelectedProductsList = megaMenuSettings.SelectedProductsList;
            var ProductsId_Displayed_On_megaMenu = _megaMenuServices.Int_array_from_Strin_array(SelectedProductsList);
            var products = _productService.GetProductsByIds(ProductsId_Displayed_On_megaMenu);
            // var products = _productService.SearchProducts(categoryIds: null, pageSize: 100, showHidden: true);
            model.Product = megaMenuSettings.Product;
            model.productIndex = megaMenuSettings.productIndex;
            model.productOverviewModel = prepareProductOverViewModel((List<Product>)products);
            #endregion products

            // assigning categories to PublicInfoModel
            #region categories
            var topMenuModel = _catalogModelFactory.PrepareTopMenuModel();
            model.Category = megaMenuSettings.Category;
            model.categorylist = getCategoryListFromTopMenuModel((List<CategorySimpleModel>)topMenuModel.Categories);
            model.categoryIndex = megaMenuSettings.categoryIndex;
            #endregion categories


            // assigning Manufacturer to PublicInfoModel
            #region Manufacturer
            var ManufacturerModelIList = _catalogModelFactory.PrepareManufacturerAllModels();
            model.Manufacturer = megaMenuSettings.Manufacturer;
            model.ManufacturerModelIList = ManufacturerModelIList;
            model.manufacturerIndex = megaMenuSettings.productIndex;
            #endregion Manufacturer

            return model;
        }

        private IList<CategoryModel> getCategoryListFromTopMenuModel(List<CategorySimpleModel> categorySimpleModel)
        {
            var tempcategorylist = new List<CategoryModel>();

            foreach (CategorySimpleModel item in categorySimpleModel)
            {
                CategoryModel tempCategoryModel = new CategoryModel();

                // check If exists in setting _table
                if (_megaMenuServices._check_if_category_tickedin_settingsTable(item.Id))
                {
                    var category = _categoryService.GetCategoryById(item.Id);

                    CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
                    var currentcategoryModel = _catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
                    tempCategoryModel = currentcategoryModel;
                    tempCategoryModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(category.PictureId);

                    tempCategoryModel = prepareCategoryModel(category, tempCategoryModel);

                    tempcategorylist.Add(tempCategoryModel);
                }
            }

            //sorting by subcategories count
            tempcategorylist.Sort((x, y) => x.SubCategories.Count.CompareTo(y.SubCategories.Count));
            tempcategorylist.Reverse();

            return tempcategorylist;
        }

        public IEnumerable<ProductOverviewModel> prepareProductOverViewModel(List<Product> productList)
        {
            var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(productList);
             
            return productOverviewModel;

        }


        // prepareCategoryModel converts CategorySimpleModel --> categoryModel (comsplex model with images)
        private CategoryModel prepareCategoryModel(Category category, CategoryModel tempCategoryModel)
        {
            CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
            var currentcategoryModel = _catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);
            tempCategoryModel = currentcategoryModel;
            tempCategoryModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(category.PictureId);

            return tempCategoryModel;
        }

        #endregion publicInfo
    }
}
