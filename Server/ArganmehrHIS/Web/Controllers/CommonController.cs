using Common.Caching;
using Common.Helpers.Extentions;
using DomainClasses.Entities.Theme;
using DomainClasses.Localization;
using DomainClasses.Theme;
using IocConfig.Localization;
using IocConfig.Theming;
using ServiceLayer;
using ServiceLayer.Congfiguration;
using ServiceLayer.Contracts.Common;
using ServiceLayer.Helper;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModel.Common;

namespace Web.Controllers
{
    [RoutePrefix("Common")]
    [Route("{action}")]
    public partial class CommonController : Controller
    {


        #region Fields

        private readonly Lazy<ILanguageService> _languageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICommonServices _services;
        private readonly IThemeContext _themeContext;
        private readonly ThemeSettings _themeSettings;
        private readonly Lazy<IThemeRegistry> _themeRegistry;
        private readonly Lazy<IGenericAttributeService> _genericAttributeService;
        private readonly Lazy<IMobileDeviceHelper> _mobileDeviceHelper;
        #endregion

        #region Constructors

        public CommonController(
            Lazy<ILanguageService> languageService,
            LocalizationSettings localizationSettings,
            IThemeContext themeContext,
            Lazy<IThemeRegistry> themeRegistry,
            Lazy<IGenericAttributeService> genericAttributeService,
            Lazy<IMobileDeviceHelper> mobileDeviceHelper,
            ThemeSettings themeSettings,
            ICommonServices services)
        {
            this._languageService = languageService;
            this._localizationSettings = localizationSettings;
            this._services = services;
            this._themeContext = themeContext;
            this._themeRegistry = themeRegistry;
            this._mobileDeviceHelper = mobileDeviceHelper;
            this._themeSettings = themeSettings;




        }

        #endregion


        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _services.Cache.Get(ModelCacheKRB.AVAILABLE_LANGUAGES_MODEL_KEY, () =>
            {
                var result = _languageService.Value
                    .GetAllLanguages(true)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NativeName = LocalizationHelper.GetLanguageNativeName(x.LanguageCulture) ?? x.Name,
                        ISOCode = x.LanguageCulture,
                        SeoCode = x.UniqueSeoCode,
                        FlagImageFileName = x.FlagImageFileName
                    })
                    .ToList();
                return result;
            });

            var workingLanguage = _services.WorkContext.WorkingLanguage;

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = workingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            string defaultSeoCode = _languageService.Value.GetDefaultLanguageSeoCode();

            foreach (var lang in model.AvailableLanguages)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request, true);

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (lang.SeoCode == defaultSeoCode && (int)(_localizationSettings.DefaultLanguageRedirectBehaviour) > 0)
                    {
                        helper.StripSeoCode();
                    }
                    else
                    {
                        helper.PrependSeoCode(lang.SeoCode, true);
                    }
                }

                model.ReturnUrls[lang.SeoCode] = helper.GetAbsolutePath();
            }

            return model;
        }


        [ChildActionOnly]
        public virtual ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();

            if (model.AvailableLanguages.Count < 2)
                return Content("");


            return PartialView(model);
        }

        public virtual ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.Value.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _services.WorkContext.WorkingLanguage = language;
            }

            //  url referrer
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _services.WebHelper.GetUrlReferrer();
            }

            // home page
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.RouteUrl("HomePage");
            }

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request.ApplicationPath, returnUrl, true);
                helper.PrependSeoCode(_services.WorkContext.WorkingLanguage.UniqueSeoCode, true);
                returnUrl = helper.GetAbsolutePath();
            }

            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public virtual ActionResult ArganmehrThemeSelector()
        {
            if (!_themeSettings.AllowCustomerToSelectTheme)
                return Content("");

            var model = new ArganmehrThemeSelectorModel();
            var currentTheme = _themeRegistry.Value.GetThemeManifest(_themeContext.WorkingDesktopTheme);
            model.CurrentTheme = new ArganmehrThemeModel()
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableThemes = _themeRegistry.Value.GetThemeManifests()
                //do not display themes for mobile devices
                .Where(x => !x.MobileTheme)
                .Select(x =>
                {
                    return new ArganmehrThemeModel()
                    {
                        Name = x.ThemeName,
                        Title = x.ThemeTitle
                    };
                })
                .ToList();
            return PartialView(model);
        }

        public virtual ActionResult ChangeTheme(string themeName, string returnUrl = null)
        {
            if (!_themeSettings.AllowCustomerToSelectTheme || (themeName.HasValue() && !_themeRegistry.Value.ThemeManifestExists(themeName)))
            {
                return HttpNotFound();
            }

            _themeContext.WorkingDesktopTheme = themeName;

            if (HttpContext.Request.IsAjaxRequest())
            {
                return Json(new { Success = true });
            }

            if (returnUrl.IsEmpty())
            {
                return RedirectToAction("Index","Home");
            }

            return Redirect(returnUrl);
        }

    }
}
