using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.MegaMenu.Models;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Configuration;
using Nop.Web.Models.Catalog;
using System.Web.Mvc;
using System.IO;
using Nop.Web.Factories;
using Nop.Core;
using Nop.Services.Media;

namespace Nop.Plugin.Widgets.MegaMenu.Services
{
    public class MegaMenuServices
    {
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService ;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;

        public MegaMenuServices(ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            ISettingService settingService,
            IPictureService pictureService) {
            this._catalogModelFactory = catalogModelFactory;
            this._categoryService = categoryService;
            this._settingService = settingService;
            this._pictureService = pictureService;
        }
        public string AlterCategoryDisplayOrder(string displayorderJson , int storeScope = 0)
        {
                                                               // c# object from json
            List<DisplayorderModel> objDeserDisplayOrderList = function_convert_json_to_displayOrdermodel(displayorderJson);

            var topMenuModel = _catalogModelFactory.PrepareTopMenuModel();
            var listTopMenuModel = (List<CategorySimpleModel>)topMenuModel.Categories;

            // means thrown to trashbin
            var DeletedTopMenuCategoryList = listTopMenuModel.Where(x => !objDeserDisplayOrderList.Any(z=>z.Id == Convert.ToString(x.Id)));

            foreach (DisplayorderModel item in objDeserDisplayOrderList)
            {
                int categoryId;
                int DisplayOrder = objDeserDisplayOrderList.IndexOf(item);
                // if numerice mean real category  else megamenu
                if (int.TryParse(item.Id, out categoryId))
                {
                    Category category = _categoryService.GetCategoryById(categoryId);
                    
                    // because index starts with zero
                    category.DisplayOrder = DisplayOrder + 1;
                    _categoryService.UpdateCategory(category);                     
                }                 
            }

            foreach (CategorySimpleModel deletedItem in DeletedTopMenuCategoryList)
            {
                Category category = _categoryService.GetCategoryById(deletedItem.Id);

                // because index starts with zero
                category.IncludeInTopMenu = false;
                _categoryService.UpdateCategory(category);
            }

            return displayorderJson;
        }

        public List<DisplayorderModel> function_convert_json_to_displayOrdermodel(string displayorderJson)
        {
            List<DisplayorderModel> objDeserDisplayOrderList = JsonConvert.DeserializeObject<List<DisplayorderModel>>(displayorderJson);
            return objDeserDisplayOrderList;
        }

        // [Id =1 , label = "Car"] , [Id= 2 , label = "computer"]   =>  [{"id" : 1 , "label" : car} , {"id" :2 , "label" : "electronics" }]
        public string function_convert_displayOrdermodel_to_json(List<DisplayorderModel> displayorderModelList)
        {
            string displayorderJson = JsonConvert.SerializeObject(displayorderModelList); ; 

            return  displayorderJson;
        }

        internal string ConvertcategorylisttoJson(List<CategorySimpleModel> categorylist, MegaMenuSettings megaMenuSettings)
        {
            List<DisplayorderModel> displayorderModelList = new List<DisplayorderModel>();
            foreach (CategorySimpleModel item in categorylist)
            {
                 
                    DisplayorderModel displayorderModel = new DisplayorderModel();

                    displayorderModel.Id = item.Id.ToString();
                    displayorderModel.label = item.Name;

                    displayorderModelList.Add(displayorderModel);
                
            }

           // displayorderModelList = addMegaMenuDropDownItems(displayorderModelList , megaMenuSettings);
            string topMenuCategoriesJson =  function_convert_displayOrdermodel_to_json(displayorderModelList);

            return topMenuCategoriesJson;
        }
         

        public List<DisplayorderModel> addMegaMenuDropDownItems(List<DisplayorderModel> displayorderModelList , MegaMenuSettings megaMenuSettings)
        {
            DisplayorderModel product = new DisplayorderModel();
            product.Id = "product";
            product.label = "product megamenu";
            product.index = megaMenuSettings.productIndex;

            DisplayorderModel category = new DisplayorderModel();
            category.Id = "category";
            category.label = "category megamenu";
            category.index = megaMenuSettings.categoryIndex;

            DisplayorderModel manufacturer = new DisplayorderModel();
            manufacturer.Id = "manufacturer";
            manufacturer.label = "manufacturer megamenu";
            manufacturer.index = megaMenuSettings.manufacturerIndex;

            // insert smallest index first
            List<DisplayorderModel> tempdisplayorderModelList = new List<DisplayorderModel>();
            tempdisplayorderModelList.Add(product);
            tempdisplayorderModelList.Add(category);
            tempdisplayorderModelList.Add(manufacturer);

            var SortdisplayorderModelList = tempdisplayorderModelList.OrderBy(x => x.index).ToList();

            foreach (DisplayorderModel item in SortdisplayorderModelList)
            {
                displayorderModelList.Insert(item.index.Value ,item);
            }                     

            return displayorderModelList;
        }


        // IPagedList<Category>   -->  [{"id" : "1" , "name": Computer" , "imgurl" : "<img src..." , "ticked" : "true"} , {"id" :2 .........]}]
        public string Convert_PagedList_to_categorywith_id_name_pictureUrl_Array_for_multiselect(IPagedList<Category> allCategoriesList_Paged)
        {
            List<ConfigurationModel.multiselecteModel> AllCategoriesList = new List<ConfigurationModel.multiselecteModel>();

            foreach(Category category in allCategoriesList_Paged.Where(x=>x.ParentCategoryId == 0).ToList())
            {                 
                ConfigurationModel.multiselecteModel multiselecteModel = new ConfigurationModel.multiselecteModel();
                multiselecteModel.id= Convert.ToString(category.Id);
                multiselecteModel.name = category.Name;
                multiselecteModel.imgurl = "<img src='"+_pictureService.GetPictureUrl(category.PictureId) + "'>";
                multiselecteModel.ticked = _check_if_category_tickedin_settingsTable(category.Id);
                AllCategoriesList.Add(multiselecteModel);
            }
             
            string jsondata = JsonConvert.SerializeObject(AllCategoriesList);

            return jsondata;
        }

        public string get_Selected_categories_fromJson(string outputAllCategories)
        {
            string Selected_categories = string.Empty;  
            if (!(string.IsNullOrWhiteSpace(outputAllCategories)))
            {
                        List<int> selectedIdsArray = new List<int>();
                        // List<ConfigurationModel.multiselecteModel> multiselecteModelList = JsonConvert.DeserializeObject<List<ConfigurationModel.multiselecteModel>>(outputAllCategories);

                        //converting "1","2","5"  -> [1,2,5]                 
                        selectedIdsArray = outputAllCategories.Split(',').Select(n => Convert.ToInt32(n)).ToList();
                        Selected_categories = create_CSV_From_selectedIdsArray(selectedIdsArray.ToArray());
            }
            return Selected_categories;
        }

        // category ✓ means return true & show  as checked in dropdown
        public bool _check_if_category_tickedin_settingsTable(int categoryid)
        {
            var megaMenuSettings = _settingService.LoadSetting<MegaMenuSettings>(0);
            int[] selected_category_array = Int_array_from_Strin_array(megaMenuSettings.Selected_categories);

            var result = selected_category_array.ToList().Contains(categoryid);
            return result;
        }

        public string _get_Category_imgurl_from_categoryid(Category category)
        {
            CatalogPagingFilteringModel catalogPagingFilteringModel = new CatalogPagingFilteringModel();
            CategoryModel categoryModel = _catalogModelFactory.PrepareCategoryModel(category, catalogPagingFilteringModel);

            return categoryModel.PictureModel.ImageUrl;
        }


        // [23,65,85,45 ,67]   ->  ["23","65","85" , "45" , "67"]
        public string create_CSV_From_selectedIdsArray(int[] selectedIdsArray)
        {
            string csv_selectedProductIds = "";
            string arrayJsonObj;

            if (selectedIdsArray != null)
            {
                csv_selectedProductIds = string.Join(",", selectedIdsArray);
                arrayJsonObj = "[" + csv_selectedProductIds + "]";
            }
            else
            {
                arrayJsonObj = string.Empty;
            }
            
            return arrayJsonObj;
        }


        // ["2" ,"4" ,"6"]   -> [2,4,5]    
        public int[] Int_array_from_Strin_array(string strStringArray_of_numbers)
        {
            List<Int32> resulIntArray = new List<int>();

            // null checking

            if (!(string.IsNullOrEmpty(strStringArray_of_numbers)))
            {
                //cleaning
                strStringArray_of_numbers = strStringArray_of_numbers.Replace("[", string.Empty);
                strStringArray_of_numbers = strStringArray_of_numbers.Replace("]", string.Empty);

                if (!(string.IsNullOrWhiteSpace(strStringArray_of_numbers)))
                {
                    resulIntArray = strStringArray_of_numbers.Split(',').Select(n => Convert.ToInt32(n)).ToList();
                }
            }
            return resulIntArray.ToArray();
        }

        public string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }
    }
}
