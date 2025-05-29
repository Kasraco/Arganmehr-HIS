using DomainClasses.Entities.Theme;
using DomainClasses.Entities.Users;
using DomainClasses.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainClasses.Entities.Cms;
using Common.Caching;
using Common.Helpers.Extentions;
using DomainClasses.Theme;
namespace IocConfig
{
    public class FrameworkCacheConsumer :
      IConsumer<EntityInserted<ThemeVariable>>,
      IConsumer<EntityUpdated<ThemeVariable>>,
      IConsumer<EntityDeleted<ThemeVariable>>,
      IConsumer<EntityUpdated<Setting>>,
      IConsumer<ThemeTouchedEvent>
    {

        /// <summary>
        /// Key for ThemeVariables caching
        /// </summary>
        /// <remarks>
        /// {0} : theme name
        /// {1} : store identifier
        /// </remarks>
        public const string THEMEVARS_LESSCSS_KEY = "am.pres.themevars-lesscss-{0}-{1}";
        public const string THEMEVARS_LESSCSS_THEME_KEY = "am.pres.themevars-lesscss-{0}";


        /// <summary>
        /// Key for tax display type caching
        /// </summary>
        /// <remarks>
        /// {0} : customer role ids
        /// {1} : store identifier
        /// </remarks>
        public const string CUSTOMERROLES_TAX_DISPLAY_TYPES_KEY = "am.fw.customerroles.taxdisplaytypes-{0}-{1}";
        public const string CUSTOMERROLES_TAX_DISPLAY_TYPES_PATTERN_KEY = "am.fw.customerroles.taxdisplaytypes";

        private readonly ICacheManagerKRB _cacheManager;
        private readonly ICacheManagerKRB _aspCache;

        public FrameworkCacheConsumer(Func<string, ICacheManagerKRB> cache)
        {
            this._cacheManager = cache("static");
            this._aspCache = cache("aspnet");
        }

        public void HandleEvent(EntityInserted<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(EntityUpdated<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(EntityDeleted<ThemeVariable> eventMessage)
        {
            _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        }

        public void HandleEvent(ThemeTouchedEvent eventMessage)
        {
            var cacheKey = BuildThemeVarsCacheKey(eventMessage.ThemeName, 0);
            _aspCache.RemoveByPattern(cacheKey);
        }


     
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            // clear models which depend on settings
            _cacheManager.RemoveByPattern(CUSTOMERROLES_TAX_DISPLAY_TYPES_PATTERN_KEY); // depends on TaxSettings.TaxDisplayType
        }

        #region Helpers

        private static string BuildThemeVarsCacheKey(ThemeVariable entity)
        {
            return BuildThemeVarsCacheKey(entity.Theme, entity.StoreId);
        }

        internal static string BuildThemeVarsCacheKey(string themeName, int storeId)
        {
            if (storeId > 0)
            {
                return THEMEVARS_LESSCSS_KEY.FormatInvariant(themeName, storeId);
            }

            return THEMEVARS_LESSCSS_THEME_KEY.FormatInvariant(themeName);
        }

        #endregion

    }
}
