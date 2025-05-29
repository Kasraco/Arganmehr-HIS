using AutoMapper;
using Common.Controller;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.Entities.Localization;
using DomainClasses.Localization;
using IocConfig.Controllers;
using IocConfig.Localization;
using ServiceLayer;
using ServiceLayer.Contracts.AM.Directory;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModel.AdminArea.Arganmehr;

namespace Web.Areas.Administrator.Controllers
{
    [RouteArea("Administrator")]
    [RoutePrefix("Language")]
    [Route("{action}")]
    public partial class LanguageController : AdminControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILanguageService _languageService;       
        private readonly ICommonServices _services;
        private readonly IMappingEngine _mappingEngine;
        private readonly ICountryService _countryService;

        public LanguageController(IUnitOfWork uow,IMappingEngine mappingEngine, ILanguageService languageService, ICommonServices services,ICountryService countryService)
        {
            this._languageService = languageService;         
            this._services = services;
            _mappingEngine = mappingEngine;
            _uow = uow;
            this._countryService = countryService;
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

            foreach (var path in Directory.EnumerateFiles(Server.MapPath("~/Content/Images/flags/"), "*.png", SearchOption.TopDirectoryOnly))
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

            //model.AvailableStores = _services.StoreService.GetAllStores()
            //    .Select(s => s.ToModel())
            //    .ToList();

            //if (!excludeProperties)
            //{
            //    model.SelectedInstitutionIds =  _storeMappingService.GetStoresIdsWithAccess(language);
            //}
        }

        public virtual ActionResult Index()
        {
            return PartialView(MVC.Administrator.Language.Views._Index);
        }

        public virtual ActionResult List()
        {

            var languages = _services.WorkContext.WorkingLanguages.ToList();
            var LM = _mappingEngine.Map<IList<LanguageModel>>(languages);

            return PartialView(MVC.Administrator.Language.Views._List, LM);
        }

        [HttpGet]
        public virtual ActionResult AddLanguage()
        {
            var model = new LanguageModel();

            PrepareLanguageModel(model, null, false);

           return PartialView(MVC.Administrator.Language.Views._AddLanguage,model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult AddLanguage(LanguageModel model)
        {
           // model.Name = "ttttt";
            var lang = _mappingEngine.Map<Language>(model);
            if(!ModelState.IsValid)
            {
                   this.NotyWarning("اطلاعات وارد شده صحیح نمی باشد");
                   return RedirectToAction(MVC.Administrator.Language.ActionNames.Index, MVC.Administrator.Language.Name);
            }
            _languageService.InsertLanguage(lang);
            _uow.SaveChanges();
        //  this.NotySuccessModal(T("Admin.Configuration.Languages.Added"));
            return RedirectToAction(MVC.Administrator.Language.ActionNames.Index, MVC.Administrator.Language.Name);
        }
    }
}
