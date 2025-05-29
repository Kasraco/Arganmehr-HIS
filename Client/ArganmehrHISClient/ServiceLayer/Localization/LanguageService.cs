using Common.Caching;
using Common.Collections;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.Configurations;
using DomainClasses.Entities.Localization;
using ServiceLayer.Congfiguration;
using ServiceLayer.Contracts.AM.CA;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Localization
{
    public class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "ArganmehrPortal.language.all-{0}";
        private const string LANGUAGES_COUNT = "ArganmehrPortal.language.count-{0}";
        private const string LANGUAGES_BY_CULTURE_KEY = "ArganmehrPortal.language.culture-{0}";
        private const string LANGUAGES_BY_SEOCODE_KEY = "ArganmehrPortal.language.seocode-{0}";
        private const string LANGUAGES_PATTERN_KEY = "ArganmehrPortal.language.";
        private const string LANGUAGES_BY_ID_KEY = "ArganmehrPortal.language.id-{0}";
        #endregion

        private readonly IUnitOfWork _uow;
        private readonly IDbSet<Language> _lanquage;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly ISettingServices _settingService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ICentralAdminService _CentralAdminServce;
        public LanguageService(IUnitOfWork uow, ICacheManagerKRB cacheManager, ISettingServices settingService, LocalizationSettings localizationSettings, ICentralAdminService CentralAdminServce)
        {
            _uow = uow;
            _lanquage = _uow.Set<Language>();
            _cacheManager = cacheManager;
            _settingService = settingService;
            _localizationSettings = localizationSettings;
            _CentralAdminServce = CentralAdminServce;
        }
        public void DeleteLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            //update default admin area language (if required)
            if (_localizationSettings.DefaultAdminLanguageId == language.Id)
            {
                foreach (var activeLanguage in GetAllLanguages())
                {
                    if (activeLanguage.Id != language.Id)
                    {
                        _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                        _settingService.SaveSetting(_localizationSettings);
                        break;
                    }
                }
            }

            _lanquage.Remove(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        public IList<Language> GetAllLanguages(bool showHidden = false, int portalId = 0)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            var languages = _cacheManager.Get(key, () =>
            {
                var query = _lanquage.AsQueryable();
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder);
                return query.ToList();
            });

            ////portal mapping
            //if (portalId > 0)
            //{
            //    languages = languages
            //        .Where(l => _portalMappingService.Authorize(l, portalId))
            //        .ToList();
            //}
            return languages;
        }

        public int GetLanguagesCount(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_COUNT, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _lanquage.AsQueryable();
                if (!showHidden)
                    query = query.Where(l => l.Published);
                return query.Select(x => x.Id).Count();
            });
        }

        public Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
            {
                return _lanquage.Find(languageId);
            });
        }

        public Language GetLanguageByCulture(string culture)
        {
            if (!culture.HasValue())
                return null;

            string key = string.Format(LANGUAGES_BY_CULTURE_KEY, culture);
            return _cacheManager.Get(key, () =>
            {
                return _lanquage.AsQueryable().Where(x => culture.Equals(x.LanguageCulture, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });
        }

        public Language GetLanguageBySeoCode(string seoCode)
        {
            if (!seoCode.HasValue())
                return null;

            string key = string.Format(LANGUAGES_BY_SEOCODE_KEY, seoCode);
            return _cacheManager.Get(key, () =>
            {
                return _lanquage.AsQueryable().Where(x => seoCode.Equals(x.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            });
        }

        public void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _lanquage.Add(language);
            _uow.SaveChanges();
            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

        }

        public void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            var l = GetLanguageById(language.Id);
            l.DisplayOrder = language.DisplayOrder;
            l.FlagImageFileName = language.FlagImageFileName;
            l.LanguageCulture = language.LanguageCulture;
            l.Name = language.Name;
            l.Published = language.Published;
            l.Rtl = language.Rtl;
            l.UniqueSeoCode = language.UniqueSeoCode;


            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

        }

        public bool IsPublishedLanguage(string seoCode, int portalId = 0)
        {
            //if (portalId <= 0)
            //    portalId = _portalContext.CurrentPortal.Id;

            var map = this.GetStoreLanguageMap();
            if (map.ContainsKey(portalId))
            {
                return map[portalId].Any(x => x.Item2 == seoCode);
            }

            return false;
        }

        public bool IsPublishedLanguage(int languageId, int portalId = 0)
        {
            if (languageId <= 0)
                return false;

            //if (portalId <= 0)
            //    portalId = _portalContext.CurrentPortal.Id;

            var map = this.GetStoreLanguageMap();
            if (map.ContainsKey(portalId))
            {
                return map[portalId].Any(x => x.Item1 == languageId);
            }

            return false;
        }

        public string GetDefaultLanguageSeoCode(int portalId = 0)
        {
            //if (portalId <= 0)
            //    portalId = _portalContext.CurrentPortal.Id;

            var map = this.GetStoreLanguageMap();           
            if (map.ContainsKey(portalId))
            {
                return map[portalId].FirstOrDefault().Item2;
            }

            return null;
        }

        public int GetDefaultLanguageId(int portalId = 1)
        {
            if (portalId <= 0)
                portalId = _CentralAdminServce.GetFirstCentralAdmin().Id;

            var map = this.GetStoreLanguageMap();
            if (map.ContainsKey(portalId))
            {
                return map[portalId].FirstOrDefault().Item1;
            }

            return 0;
        }

        protected virtual Multimap<int, Tuple<int, string>> GetStoreLanguageMap()
        {
            var result = _cacheManager.Get("amclient.svc.institutionlangmap", () =>
            {
                var map = new Multimap<int, Tuple<int, string>>();

                var allCentralAdmins = _CentralAdminServce.GetAllCentralAdmins();
                foreach (var CentralAdmin in allCentralAdmins)
                {
                    var languages = GetAllLanguages(false, CentralAdmin.Id);
                    if (!languages.Any())
                    {
                        var firstStoreLang = GetAllLanguages(true, CentralAdmin.Id).FirstOrDefault();
                        if (firstStoreLang == null)
                        {
                            // absolute fallback
                            firstStoreLang = GetAllLanguages(true).FirstOrDefault();
                        }
                        map.Add(CentralAdmin.Id, new Tuple<int, string>(firstStoreLang.Id, firstStoreLang.UniqueSeoCode));
                    }
                    else
                    {
                        foreach (var lang in languages)
                        {
                            map.Add(CentralAdmin.Id, new Tuple<int, string>(lang.Id, lang.UniqueSeoCode));
                        }
                    }
                }

                return map;
            }, 1440 /* 24 hrs */);

            return result;
        }


        public void SeedDatabase()
        {
            Language L = this.GetLanguageByCulture("fa-IR");
            if (L == null)
            {
                L = new Language
                {
                    DisplayOrder = 0,
                    LanguageCulture = "fa-IR",
                    Name = "Persian",
                    Published = true,
                    Rtl = true,
                    FlagImageFileName = "fa.png",
                    UniqueSeoCode = "fa"
                };
                this.InsertLanguage(L);
            }
            var CMS = this._CentralAdminServce.GetAllCentralAdmins();
            foreach (var cm in CMS)
            {
               //cm.Language = L;
                this._CentralAdminServce.UpdateCentralAdmin(cm);
            }
            _uow.SaveAllChanges();


        }
    }
}
