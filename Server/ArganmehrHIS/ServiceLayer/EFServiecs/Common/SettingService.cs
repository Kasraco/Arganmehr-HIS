
using DataLayer.Context;
using DomainClasses.Configurations;
using DomainClasses.Entities.Cms;
using ServiceLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arganmehr.Service
{
    public class SettingService : ISettingServices
    {
        #region Fields
        #endregion

        #region Ctor
        public SettingService(IUnitOfWork unitOfWork)
        {

        }
        #endregion

        public Setting GetSettingById(int settingId)
        {
            throw new NotImplementedException();
        }

        public T GetSettingByKey<T>(string key, T defaultValue = default(T), int portalId = 0, bool loadSharedValueIfNotFound = false)
        {
            throw new NotImplementedException();
        }

        public IList<Setting> GetAllSettings()
        {
            throw new NotImplementedException();
        }

        public bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public T LoadSetting<T>(int portalId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SetSetting<T>(string key, T value, int portalId = 0, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T>(T settings, int portalId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0, bool clearCache = true) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, int portalId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void InsertSetting(Setting setting, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting(Setting setting, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting<T>() where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int portalId = 0) where T : ISettings, new()
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting(string key, int portalId = 0)
        {
            throw new NotImplementedException();
        }

        public int DeleteSettings(string rootKey)
        {
            throw new NotImplementedException();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }
    }
}
