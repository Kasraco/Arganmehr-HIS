﻿
using DomainClasses.Configurations;
namespace ServiceLayer.Congfiguration
{
    public class LocalizationSettings: ISettings
    {

        public LocalizationSettings()
        {
            UseImagesForLanguageSelection = true;
            DefaultLanguageRedirectBehaviour = DefaultLanguageRedirectBehaviour.StripSeoCode;
            InvalidLanguageRedirectBehaviour = InvalidLanguageRedirectBehaviour.ReturnHttp404;
            LoadAllLocalizedPropertiesOnStartup = true;
        }

        /// <summary>
        /// Default admin area language identifier
        /// </summary>
        public int DefaultAdminLanguageId { get; set; }

        /// <summary>
        /// Use images for language selection
        /// </summary>
        public bool UseImagesForLanguageSelection { get; set; }

        /// <summary>
        /// A value indicating whether to load all records on application startup
        /// </summary>
        public bool LoadAllLocaleRecordsOnStartup { get; set; }

        /// <summary>
        /// A value indicating whether to load all localized entity properties on application startup
        /// </summary>
        public bool LoadAllLocalizedPropertiesOnStartup { get; set; }

        /// <summary>
        /// A value indicating whether the browser user lannguage should be detected
        /// </summary>
        public bool DetectBrowserUserLanguage { get; set; }

        /// <summary>
        /// A value indicating whether SEO friendly URLs with multiple languages are enabled
        /// </summary>
        public bool SeoFriendlyUrlsForLanguagesEnabled { get; set; }

        /// <summary>
        /// A value specifying if and how default language redirection should be handled.
        /// </summary>
        /// <remarks>This setting is ignored when <c>SeoFriendlyUrlsForLanguagesEnabled</c> is <c>false</c></remarks>
        public DefaultLanguageRedirectBehaviour DefaultLanguageRedirectBehaviour { get; set; }

        /// <summary>
        /// A value specifying how requests for invalid or unpublished languages should be handled
        /// </summary>
        /// <remarks>This setting is ignored when <c>SeoFriendlyUrlsForLanguagesEnabled</c> is <c>false</c></remarks>
        public InvalidLanguageRedirectBehaviour InvalidLanguageRedirectBehaviour { get; set; }
    }

    public enum DefaultLanguageRedirectBehaviour
    {
        PrependSeoCodeAndRedirect = 0,
        DoNoRedirect = 1,
        StripSeoCode = 2
    }

    public enum InvalidLanguageRedirectBehaviour
    {
        Tolerate = 0,
        FallbackToWorkingLanguage = 1,
        ReturnHttp404 = 2
    }
}
