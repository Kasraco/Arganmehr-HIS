using Common.Caching;
using Common.Helpers.Extentions;
using DomainClasses.Entities.Localization;
using ServiceLayer.Congfiguration;
using ServiceLayer.Context;
using ServiceLayer.Contracts.AM.CA;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IocConfig.Extensions;
using IocConfig.Localization;
using ServiceLayer.EFServiecs.Users;
using DomainClasses.Entities.Users;
using ServiceLayer.Helper;

namespace IocConfig
{
    public class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "arganmehr.user";
        private readonly HttpContextBase _httpContext;
        private readonly ILanguageService _languageService;
        private readonly ICAContext _caContext;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly ICentralAdminService _CMServices;
        private readonly ApplicationUserManager _user;
        private readonly IGenericAttributeService _attrService;
        private readonly ICentralAdminService _caService;
        
        private Language _cachedLanguage;
          private User _cachedUser;
        private IList<Language> _cachedLanguages;
        private bool? _isAdmin;

        public WebWorkContext(Func<string, ICacheManagerKRB> cacheManager,
            HttpContextBase httpContext,
            ICAContext CAContext,
            ILanguageService languageService,
            LocalizationSettings localizationSettings,
            ICentralAdminService cmServices,
            ApplicationUserManager user,
            IGenericAttributeService attrService,
             ICentralAdminService caService)
        {
            this._cacheManager = cacheManager("static");
            this._httpContext = httpContext;
            this._caContext = CAContext;
            this._languageService = languageService;
            this._localizationSettings = localizationSettings;
            this._CMServices = cmServices;
            _user = user;
            this._attrService = attrService;
            this._caService = caService;
        }


        protected HttpCookie GetuserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected void SetUserCookie(long customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid ==0)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                try
                {
                    if (_httpContext.Response.Cookies[UserCookieName] != null)
                        _httpContext.Response.Cookies.Remove(UserCookieName);
                }
                catch (Exception) { }

                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        public Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                int CMId = _caContext.CurrentCA.Id;
                int userLangId = 0;

                if (this.CurrentUser != null)
                {
                    userLangId = this.CurrentUser.GetAttribute<int>(
                        "LanguageId",
                        _attrService,
                        _caContext.CurrentCA.Id);
                }

                #region Get language from URL (if possible)

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && _httpContext != null && _httpContext.Request != null)
                {
                    var helper = new LocalizedUrlHelper(_httpContext.Request, true);
                    string seoCode;
                    if (helper.IsLocalizedUrl(out seoCode))
                    {
                        if (_languageService.IsPublishedLanguage(seoCode, CMId))
                        {
                            // the language is found. now we need to save it
                            var langBySeoCode = _languageService.GetLanguageBySeoCode(seoCode);

                            if (this.CurrentUser != null && userLangId != langBySeoCode.Id)
                            {
                                userLangId = langBySeoCode.Id;
                                this.SetUserLanguage(langBySeoCode.Id, CMId);
                            }
                            _cachedLanguage = langBySeoCode;
                            return langBySeoCode;
                        }
                    }
                }

                #endregion

                if (_localizationSettings.DetectBrowserUserLanguage && (userLangId == 0 || !_languageService.IsPublishedLanguage(userLangId, CMId)))
                {
                    #region Get Browser UserLanguage

                    // Fallback to browser detected language
                    Language browserLanguage = null;

                    if (_httpContext != null && _httpContext.Request != null && _httpContext.Request.UserLanguages != null)
                    {
                        var userLangs = _httpContext.Request.UserLanguages.Select(x => x.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (userLangs.Any())
                        {
                            foreach (var culture in userLangs)
                            {
                                browserLanguage = _languageService.GetLanguageByCulture(culture);
                                if (browserLanguage != null && _languageService.IsPublishedLanguage(browserLanguage.Id, CMId))
                                {
                                    // the language is found. now we need to save it
                                    if (this.CurrentUser != null && userLangId != browserLanguage.Id)
                                    {
                                        userLangId = browserLanguage.Id;
                                        SetUserLanguage(userLangId, CMId);
                                    }
                                    _cachedLanguage = browserLanguage;
                                    return browserLanguage;
                                }
                            }
                        }
                    }

                    #endregion
                }

                if (userLangId > 0 && _languageService.IsPublishedLanguage(userLangId, CMId))
                {
                    _cachedLanguage = _languageService.GetLanguageById(userLangId);
                    return _cachedLanguage;
                }

                // Fallback
                userLangId = _languageService.GetDefaultLanguageId(CMId);
                // SetCustomerLanguage(customerLangId, storeId);

                _cachedLanguage = _languageService.GetLanguageById(userLangId);
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                this.SetUserLanguage(languageId, _caContext.CurrentCA.Id);
                _cachedLanguage = null;
            }
        }

        private void SetUserLanguage(int languageId, int storeId)
        {
            _attrService.SaveAttribute(
                this.CurrentUser,
                "LanguageId",
                languageId,
                storeId);
        }

        public IList<Language> WorkingLanguages
        {
            get {  
                if (_cachedLanguages != null)
                    return _cachedLanguages;

                _cachedLanguages = _languageService.GetAllLanguages(true);
                return _cachedLanguages;
            }
        }

        //public IList<Language> WorkingLanguages
        //{
        //    get
        //    {
        //        if (_cachedLanguages != null)
        //            return _cachedLanguages;

        //        _cachedLanguages = _languageService.GetAllLanguages();
        //        return _cachedLanguages;
        //    }
        //}

        [Obsolete("Use ILanguageService.IsPublishedLanguage() instead")]
        public bool IsPublishedLanguage(string seoCode, int PortalId = 0)
        {

            return _languageService.IsPublishedLanguage(seoCode, PortalId);
        }

        [Obsolete("Use ILanguageService.GetDefaultLanguageSeoCode() instead")]
        public string GetDefaultLanguageSeoCode(int PortalId = 0)
        {
            return _languageService.GetDefaultLanguageSeoCode(PortalId);
        }

        public bool IsAdmin
        {
            get
            {
                if (!_isAdmin.HasValue)
                {
                    _isAdmin = _httpContext.Request.IsAdminArea();
                }

                return _isAdmin.Value;
            }
            set
            {
                _isAdmin = value;
            }
        }







        public DomainClasses.Entities.Users.User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User customer = null;
                // check whether request is made by a background task
                // in this case return built-in customer record for background task
                if (_httpContext == null || _httpContext.IsFakeContext())
                {
                    customer = _user.GetCurrentUser();
                    _cachedUser = customer;
                }


                // load guest customer
                if (customer == null || customer.IsDeleted || customer.IsBanned)
                {
                    var userCookie = GetuserCookie();
                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        long customerid;
                        if (long.TryParse(userCookie.Value, out customerid))
                        {
                            var userByCookie = _user.FindUserById(customerid);
                            if (userByCookie != null )
                                customer = userByCookie;
                        }
                    }
                }

                // create guest if not exists
               // if (customer == null || customer.IsDeleted || customer.IsBanned)
              //  {
               //     customer = _user.InsertGuestCustomer();
               // }

                 if (customer == null || customer.IsDeleted || customer.IsBanned)
                  {
                     try
                     {
                         customer = _user.GetCurrentUser();
                     }
                     catch(Exception ex)
                     {
                         customer = null;
                     }
                 }

                 if (customer == null || customer.IsDeleted || customer.IsBanned)
                  {
                      customer = _user.InsertGeustUser();
                 }
                

                 if (!customer.IsDeleted)
                 {
                     SetUserCookie(customer.Id);
                     _cachedUser = customer;
                 }


                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.Id);
                _cachedUser = _user.GetCurrentUser();
            }
        }
    }
}
