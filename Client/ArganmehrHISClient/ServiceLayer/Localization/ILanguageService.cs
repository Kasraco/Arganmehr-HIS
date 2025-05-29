
using DomainClasses.Entities.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Localization
{
    public partial interface ILanguageService
    {

        void DeleteLanguage(Language language);

        IList<Language> GetAllLanguages(bool showHidden = false, int portalId = 0);


        int GetLanguagesCount(bool showHidden = false);


        Language GetLanguageById(int languageId);

        Language GetLanguageByCulture(string culture);


        Language GetLanguageBySeoCode(string seoCode);


        void InsertLanguage(Language language);

        /// <summary>

        void UpdateLanguage(Language language);

        bool IsPublishedLanguage(string seoCode, int portalId = 0);


        bool IsPublishedLanguage(int languageId, int portalId = 0);


        string GetDefaultLanguageSeoCode(int portalId = 0);


        int GetDefaultLanguageId(int portalId = 0);

        void SeedDatabase();
    }
}
