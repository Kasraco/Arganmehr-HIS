
using DomainClasses.Entities.DataExchange;
using DomainClasses.Entities.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace ServiceLayer.Localization
{
    public interface ILocalizationService
    {
        void ClearCache();

        void DeleteLocaleStringResource(LocaleStringResource localeStringResource);

        int DeleteLocaleStringResources(string key, bool keyIsRootKey = true);

        LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId);

        LocaleStringResource GetLocaleStringResourceByName(string resourceName);

        LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true);

        IList<LocaleStringResource> GetAllResources(int languageId);

        void InsertLocaleStringResource(LocaleStringResource localeStringResource);

        void UpdateLocaleStringResource(LocaleStringResource localeStringResource);

        IDictionary<string, Tuple<int, string>> GetResourceValues(int languageId, bool forceAll = false);


        string GetResource(string resourceKey, int languageId = 0, bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

        string ExportResourcesToXml(Language language);

        void ImportResourcesFromXml(Language language,
            XmlDocument xmlDocument,
            string rootKey = null,
            bool sourceIsPlugin = false,
            ImportModeFlags mode = ImportModeFlags.Insert | ImportModeFlags.Update,
            bool updateTouchedResources = false);


        XmlDocument FlattenResourceFile(XmlDocument source);
    }
}
