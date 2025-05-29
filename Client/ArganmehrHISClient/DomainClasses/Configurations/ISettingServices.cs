
using DomainClasses.Entities.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Configurations
{
    public interface ISettingServices
    {
        Setting GetSettingById(int settingId);
        T GetSettingByKey<T>(string key, T defaultValue = default(T), int portalId = 0, bool loadSharedValueIfNotFound = false);
        IList<Setting> GetAllSettings();

        bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int portalId = 0)
            where T : ISettings, new();
        T LoadSetting<T>(int portalId = 0) where T : ISettings, new();

        void SetSetting<T>(string key, T value, int portalId = 0, bool clearCache = true);

        void SaveSetting<T>(T settings, int portalId = 0) where T : ISettings, new();
        void SaveSetting<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector,
        int portalId = 0, bool clearCache = true) where T : ISettings, new();

        void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, int portalId = 0) where T : ISettings, new();

        void InsertSetting(Setting setting, bool clearCache = true);

        void UpdateSetting(Setting setting, bool clearCache = true);
        void DeleteSetting(Setting setting);

        void DeleteSetting<T>() where T : ISettings, new();
        void DeleteSetting<T, TPropType>(T settings,
        Expression<Func<T, TPropType>> keySelector, int portalId = 0) where T : ISettings, new();
        void DeleteSetting(string key, int portalId = 0);
        int DeleteSettings(string rootKey);
        void ClearCache();
    }
}
