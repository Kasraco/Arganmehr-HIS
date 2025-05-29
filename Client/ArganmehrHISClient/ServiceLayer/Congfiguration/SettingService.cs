
using Common.Caching;
using Common.ComponentModel;
using Common.Filters;
using Common.Helpers.Extentions;
using Common.TypeConversion.ComponentModel;
using DataLayer.Context;
using DomainClasses.Configurations;
using DomainClasses.Entities.Cms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Congfiguration
{
    public class SettingService : ISettingServices
    {
        #region Nested classes

        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public int PortalId { get; set; }
        }

        #endregion

        private const string SETTINGS_ALL_KEY = "Institution.setting.all";

        private readonly IDbSet<Setting> _settingServices;
        private readonly IUnitOfWork _uow;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly string _name;
        private readonly PropertyInfo[] _properties;

        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            string key = string.Format(SETTINGS_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from s in _settingServices.AsQueryable()
                            orderby s.Name, s.PortalId
                            select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>(StringComparer.OrdinalIgnoreCase);
                foreach (var s in settings)
                {
                    var settingName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        PortalId = s.PortalId
                    };

                    if (!dictionary.ContainsKey(settingName))
                    {
                        dictionary.Add(settingName, new List<SettingForCaching>(){
                                settingForCaching
                            });
                    }
                    else
                    {
                        dictionary[settingName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }




        private T LoadSettingsJson<T>(int portalId = 0)
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;

            T settings = Activator.CreateInstance<T>();

            var rawSetting = GetSettingByKey<string>(key, portalId: portalId, loadSharedValueIfNotFound: true);
            if (rawSetting.HasValue())
            {
                JsonConvert.PopulateObject(rawSetting, settings);
            }

            return settings;
        }

        private void SaveSettingsJson<T>(T settings)
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;
            var storeId = 0;

            var rawSettings = JsonConvert.SerializeObject(settings);
            SetSetting(key, rawSettings, storeId, false);

            // and now clear cache
            ClearCache();
        }

        private void DeleteSettingsJson<T>()
        {
            Type t = typeof(T);
            string key = t.Namespace + "." + t.Name;

            var setting = GetAllSettings()
                .FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (setting != null)
            {
                DeleteSetting(setting);
            }
        }
        public SettingService(IUnitOfWork uow, ICacheManagerKRB CacheManager)
        {
            _uow = uow;
            _cacheManager = CacheManager;
            _settingServices = _uow.Set<Setting>();

            var type = GetType();
            _name = type.Name;
            _properties = type.GetProperties();
        }



        public Setting GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            var setting = _settingServices.Find(settingId);
            return setting;
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), int portalId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault(x => x.PortalId == portalId);

                if (setting == null && portalId > 0 && loadSharedValueIfNotFound)
                    setting = settingsByKey.FirstOrDefault(x => x.PortalId == 0);


                if (setting != null)
                    return setting.Value.Convert<T>();
            }
            return defaultValue;
        }

        public IList<Setting> GetAllSettings()
        {
            var query = from s in _settingServices
                        orderby s.Name, s.PortalId
                        select s;
            var settings = query.ToList();
            return settings;
        }

        public bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;

            string setting = GetSettingByKey<string>(key, portalId: portalId);
            return setting != null;
        }

        public T LoadSetting<T>(int portalId = 0) where T : ISettings, new()
        {
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                return LoadSettingsJson<T>(portalId);
            }

            var settings = Activator.CreateInstance<T>();

            foreach (var fastProp in FastProperty.GetProperties(typeof(T)).Values)
            {
                var prop = fastProp.Property;

                // get properties we can read and write to
                if (!prop.CanWrite)
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                //load by store
                string setting = GetSettingByKey<string>(key, portalId: portalId, loadSharedValueIfNotFound: true);

                if (setting == null && !fastProp.IsSequenceType)
                {
                    #region Obsolete ('EnumerableConverter' can handle this case now)
                    //if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    //{
                    //	// convenience: don't return null for simple list types
                    //	var listArg = prop.PropertyType.GetGenericArguments()[0];
                    //	object list = null;

                    //	if (listArg == typeof(int))
                    //		list = new List<int>();
                    //	else if (listArg == typeof(decimal))
                    //		list = new List<decimal>();
                    //	else if (listArg == typeof(string))
                    //		list = new List<string>();

                    //	if (list != null)
                    //	{
                    //		fastProp.SetValue(settings, list);
                    //	}
                    //}
                    #endregion

                    continue;
                }

                var converter = TypeConverterFactory.GetConverter(prop.PropertyType);

                if (converter == null || !converter.CanConvertFrom(typeof(string)))
                    continue;

                object value = converter.ConvertFrom(setting);

                //set property
                fastProp.SetValue(settings, value);
            }

            return settings;
        }

        public void SetSetting<T>(string key, T value, int portalId = 0, bool clearCache = true)
        {
            key = key.Trim().ToLowerInvariant();
            var str = value.Convert<string>();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.PortalId == portalId) : null;

            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = str;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = str,
                    PortalId = portalId
                };
                InsertSetting(setting, clearCache);
            }
        }

        public void SaveSetting<T>(T settings, int portalId = 0) where T : ISettings, new()
        {
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                SaveSettingsJson<T>(settings);
                return;
            }

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in FastProperty.GetProperties(typeof(T)).Values)
            {
                // get properties we can read and write to
                if (!prop.IsPublicSettable)
                    continue;

                var converter = TypeConverterFactory.GetConverter(prop.Property.PropertyType);
                if (converter == null || !converter.CanConvertFrom(typeof(string)))
                    continue;

                string key = typeof(T).Name + "." + prop.Name;
                // Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings);

                SetSetting(key, value ?? "", portalId, false);
            }

            //and now clear cache
            ClearCache();
        }

        public void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0, bool clearCache = true) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;
            // Duck typing is not supported in C#. That's why we're using dynamic type
            var fastProp = FastProperty.GetProperty(propInfo, PropertyCachingStrategy.EagerCached);
            dynamic value = fastProp.GetValue(settings);

            SetSetting(key, value ?? "", portalId, false);
        }

        public void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, int portalId = 0) where T : ISettings, new()
        {
            if (overrideForStore || portalId == 0)
                SaveSetting(settings, keySelector, portalId, false);
            else if (portalId > 0)
                DeleteSetting(settings, keySelector, portalId);
        }

        public void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingServices.Add(setting);
            _uow.SaveChanges();

            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        public void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _uow.SaveChanges();


            //cache
            if (clearCache)
                _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

        }

        public void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingServices.Remove(setting);
            _uow.SaveChanges();

            //cache
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }

        public void DeleteSetting<T>() where T : ISettings, new()
        {
            if (typeof(T).HasAttribute<JsonPersistAttribute>(true))
            {
                DeleteSettingsJson<T>();
                return;
            }

            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings();
            foreach (var prop in typeof(T).GetProperties())
            {
                string key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            foreach (var setting in settingsToDelete)
                DeleteSetting(setting);
        }

        public void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0) where T : ISettings, new()
        {
            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            string key = typeof(T).Name + "." + propInfo.Name;

            DeleteSetting(key, portalId);
        }

        public void DeleteSetting(string key, int portalId = 0)
        {
            if (key.HasValue())
            {
                key = key.Trim().ToLowerInvariant();

                var setting = (
                    from s in _settingServices
                    where s.PortalId == portalId && s.Name == key
                    select s).FirstOrDefault();

                if (setting != null)
                    DeleteSetting(setting);
            }
        }


        public int DeleteSettings(string rootKey)
        {
            int result = 0;

            if (rootKey.HasValue())
            {
                try
                {
                    string sqlDelete = "DELETE FROM [Setting] WHERE [Name] LIKE '{0}%'".FormatWith(rootKey.EndsWith(".") ? rootKey : rootKey + ".");
                    result = _uow.ExecuteSqlCommand(sqlDelete);


                    // cache
                    _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
                }
                catch (Exception exc)
                {
                    exc.Dump();
                }
            }

            return result;
        }

        public void ClearCache()
        {
            _cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }
    }
}
