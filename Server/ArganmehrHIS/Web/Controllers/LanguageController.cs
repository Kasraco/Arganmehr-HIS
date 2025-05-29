using IocConfig.ViewModel.LanguageVModel;
using IocConfig.Extensions;
using IocConfig.Localization;
using Common.Helpers.Extentions;
using Common.Controller;
using ServiceLayer;
using ServiceLayer.Contracts.AM.Directory;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using IocConfig.Controllers;
using DomainClasses.Entities.Localization;
using ViewModel.AdminArea.Arganmehr;
using Web.Extentions;
using AutoMapper;
using System.Globalization;
using System.IO;
using IocConfig.Filters;

namespace Web.Controllers
{
    [RoutePrefix("Language")]
    [Route("{action}")]
    public class LanguageController : PublicControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly ICountryService _countryService;
        private readonly ICommonServices _services;

        private readonly IMappingEngine _mappingEngine;
       

        public LanguageController(ILanguageService languageService,IMappingEngine mappingEngine,
            ICountryService countryService,
            ICommonServices services)
        {
            this._languageService = languageService;
            this._countryService = countryService;
            this._services = services;
            _mappingEngine = mappingEngine;
        }

        private void PrepareLanguageModel(LanguageModel model, Language language, bool excludeProperties)
        {
            var languageId = _services.WorkContext.WorkingLanguage.Id;

            var allCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .OrderBy(x => x.DisplayName)
                .ToList();

            var allCountryNames = _countryService.GetAllCountries(true)
                .ToDictionary(x => x.TwoLetterIsoCode.EmptyNull().ToLower(), x => x.GetLocalized(y => y.Name, languageId, true, false));

            model.AvailableCultures = allCultures
                .Select(x => new SelectListItem { Text = "{0} [{1}]".FormatInvariant(x.DisplayName, x.IetfLanguageTag), Value = x.IetfLanguageTag })
                .ToList();

            model.AvailableTwoLetterLanguageCodes = new List<SelectListItem>();
            model.AvailableFlags = new List<SelectListItem>();

            foreach (var item in allCultures)
            {
                if (!model.AvailableTwoLetterLanguageCodes.Any(x => x.Value.IsCaseInsensitiveEqual(item.TwoLetterISOLanguageName)))
                {
                    // display language name is not provided by net framework
                    var index = item.DisplayName.EmptyNull().IndexOf(" (");

                    if (index == -1)
                        index = item.DisplayName.EmptyNull().IndexOf(" [");

                    var displayName = "{0} [{1}]".FormatInvariant(
                        index == -1 ? item.DisplayName : item.DisplayName.Substring(0, index),
                        item.TwoLetterISOLanguageName);

                    model.AvailableTwoLetterLanguageCodes.Add(new SelectListItem { Text = displayName, Value = item.TwoLetterISOLanguageName });
                }
            }

            foreach (var path in Directory.EnumerateFiles(_services.WebHelper.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(path).EmptyNull().ToLower();
                string countryDescription = null;

                if (allCountryNames.ContainsKey(name))
                    countryDescription = "{0} [{1}]".FormatInvariant(allCountryNames[name], name);

                if (countryDescription.IsEmpty())
                    countryDescription = name;

                model.AvailableFlags.Add(new SelectListItem { Text = countryDescription, Value = Path.GetFileName(path) });
            }

            model.AvailableFlags = model.AvailableFlags.OrderBy(x => x.Text).ToList();

     

            //if (!excludeProperties)
            //{
            //    model.SelectedInstitutionIds = _storeMappingService.GetStoresIdsWithAccess(language);
            //}
        }


        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
          
            var languages = _languageService.GetAllLanguages(true);

            var lng = _mappingEngine.Map < IEnumerable<LanguageModel>>(languages);
            var gridModel = new GridModel<LanguageModel>
            {
                Data = lng,
                Total = languages.Count()
            };

            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel<LanguageModel>();

            var languages = _languageService.GetAllLanguages(true);
            var lng = _mappingEngine.Map<IEnumerable<LanguageModel>>(languages);
            model.Data = lng;
            model.Total = languages.Count();


            return new JsonResult
            {
                Data = model
            };
        }



        public ActionResult Create()
        {
          

            var model = new LanguageModel();

            PrepareLanguageModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(LanguageModel model, bool continueEditing)
        {
          

            if (ModelState.IsValid)
            {
                var language = model.ToEntity();
                _languageService.InsertLanguage(language);


                var filterLanguages = new List<Language>() { language };

              

               this.NotySuccess(T("Admin.Configuration.Languages.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            PrepareLanguageModel(model, null, true);

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
           

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = language.ToModel();

            PrepareLanguageModel(model, language, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(LanguageModel model, bool continueEditing)
        {
           ;

            var language = _languageService.GetLanguageById(model.Id);
            if (language == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = _languageService.GetAllLanguages();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id &&
                    !model.Published)
                {
                   this.NotyError("At least one published language is required.");
                    return RedirectToAction("Edit", new { id = language.Id });
                }

                //update
                language = model.ToEntity(language);
                _languageService.UpdateLanguage(language);


                //notification
                this.NotySuccess(T("Admin.Configuration.Languages.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = language.Id }) : RedirectToAction("List");
            }

            PrepareLanguageModel(model, language, true);

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
           

            var language = _languageService.GetLanguageById(id);
            if (language == null)
                return RedirectToAction("List");

            //ensure we have at least one published language
            var allLanguages = _languageService.GetAllLanguages();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
            {
             this.NotyError("At least one published language is required.");
                return RedirectToAction("Edit", new { id = language.Id });
            }

            //delete
            _languageService.DeleteLanguage(language);

            //notification
            this.NotySuccess(T("Admin.Configuration.Languages.Deleted"));
            return RedirectToAction("List");
        }





        [HttpGet]
        public ActionResult Resources(int languageId)
        {

            ViewBag.AllLanguages = _languageService.GetAllLanguages(true)
                .Select(x => new SelectListItem
                {
                    Selected = (x.Id.Equals(languageId)),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            var language = _languageService.GetLanguageById(languageId);
            ViewBag.LanguageId = languageId;
            ViewBag.LanguageName = language.Name;

            var resources = _services.Localization
                .GetResourceValues(languageId, true)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
                .OrderBy(x => x.Key)
                .ToList();


            var gridModel = new GridModel<LanguageResourceModel>
            {
                Data = resources
                .Take(25)              
                .Select(x => new LanguageResourceModel()
                    {
                        LanguageId = languageId,
                        LanguageName = language.Name,
                        Id = x.Value.Item1,
                        Name = x.Key,
                        Value = x.Value.Item2.EmptyNull(),
                    }),
                Total = resources.Count
            };
            return View(gridModel);

        }


        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Resources(int languageId, GridCommand command)
        {
            var model = new GridModel<LanguageResourceModel>();


            var language = _languageService.GetLanguageById(languageId);

            var resources = _services.Localization
                .GetResourceValues(languageId, true)
                .OrderBy(x => x.Key)
                .Where(x => x.Key != "!!___EOF___!!" && x.Value != null)
                .Select(x => new LanguageResourceModel
                {
                    LanguageId = languageId,
                    LanguageName = language.Name,
                    Id = x.Value.Item1,
                    Name = x.Key,
                    Value = x.Value.Item2.EmptyNull(),
                })
                .ForCommand(command)
                .ToList();

            model.Data = resources.PagedForCommand(command);
            model.Total = resources.Count;



            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ResourceUpdate(LanguageResourceModel model, GridCommand command)
        {

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var resource = _services.Localization.GetLocaleStringResourceById(model.Id);
            // if the resourceName changed, ensure it isn't being used by another resource
            if (!resource.ResourceName.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var res = _services.Localization.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
                if (res != null && res.Id != resource.Id)
                {
                    return Content(T("Admin.Configuration.Languages.Resources.NameAlreadyExists", res.ResourceName));
                }
            }

            resource.ResourceName = model.Name;
            resource.ResourceValue = model.Value;
            resource.IsTouched = true;

            _services.Localization.UpdateLocaleStringResource(resource);


            return Resources(model.LanguageId, command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ResourceAdd(int id, LanguageResourceModel model, GridCommand command)
        {

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var res = _services.Localization.GetLocaleStringResourceByName(model.Name, model.LanguageId, false);
            if (res == null)
            {
                var resource = new LocaleStringResource { LanguageId = id };
                resource.ResourceName = model.Name;
                resource.ResourceValue = model.Value;
                resource.IsTouched = true;

                _services.Localization.InsertLocaleStringResource(resource);
            }
            else
            {
                return Content(T("Admin.Configuration.Languages.Resources.NameAlreadyExists", model.Name));
            }


            return Resources(id, command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ResourceDelete(int id, int languageId, GridCommand command)
        {

            var resource = _services.Localization.GetLocaleStringResourceById(id);

            _services.Localization.DeleteLocaleStringResource(resource);


            return Resources(languageId, command);
        }


    }
}
