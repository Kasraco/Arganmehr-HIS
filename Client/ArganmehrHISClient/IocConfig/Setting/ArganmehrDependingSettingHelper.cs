using Common.ComponentModel;
using Common.Helpers.Extentions;
using DomainClasses.Configurations;
using IocConfig.Localization;
using ServiceLayer.Contracts;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig
{


    public class ArganmehrDependingSettingHelper
    {
        private ViewDataDictionary _viewData;

        public ArganmehrDependingSettingHelper(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }

        public static string ViewDataKey { get { return "ArganmehrDependingSettingData"; } }
        public ArganmehrDependingSettingData Data
        {
            get
            {
                return _viewData[ViewDataKey] as ArganmehrDependingSettingData;
            }
        }

        private bool IsOverrideChecked(string settingKey, FormCollection form)
        {
            var rawOverrideKey = form.AllKeys.FirstOrDefault(k => k.IsCaseInsensitiveEqual(settingKey + "_OverrideForArganmehr"));

            if (rawOverrideKey.HasValue())
            {
                var checkboxValue = form[rawOverrideKey].EmptyNull().ToLower();
                return checkboxValue.Contains("on") || checkboxValue.Contains("true");
            }
            return false;
        }
        public bool IsOverrideChecked(object settings, string name, FormCollection form)
        {
            var key = settings.GetType().Name + "." + name;
            return IsOverrideChecked(key, form);
        }
        public void AddOverrideKey(object settings, string name)
        {
            var key = settings.GetType().Name + "." + name;
            Data.OverrideSettingKeys.Add(key);
        }
        public void CreateViewDataObject(int activeArganmehrScopeConfiguration, string rootSettingClass = null)
        {
            _viewData[ViewDataKey] = new ArganmehrDependingSettingData()
            {
                ActiveArganmehrScopeConfiguration = activeArganmehrScopeConfiguration,
                RootSettingClass = rootSettingClass
            };
        }

        public void GetOverrideKeys(object settings, object model, int ArganmehrId, ISettingServices settingService, bool isRootModel = true)
        {
            if (ArganmehrId <= 0)
                return;		// single Arganmehr mode -> there are no overrides

            var data = Data;
            if (data == null)
                data = new ArganmehrDependingSettingData();

            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();

            var modelType = model.GetType();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var modelProperty = modelType.GetProperty(name);

                if (modelProperty == null)
                    continue;	// setting is not configurable or missing or whatever... however we don't need the override info

                var key = settingName + "." + name;
                var setting = settingService.GetSettingByKey<string>(key,  ArganmehrId.ToString());

                if (setting != null)
                    data.OverrideSettingKeys.Add(key);
            }

            if (isRootModel)
            {
                data.ActiveArganmehrScopeConfiguration = ArganmehrId;
                data.RootSettingClass = settingName;

                _viewData[ViewDataKey] = data;
            }
        }

        // DRY?
        public void GetOverrideKeysLocalized(object settings, object model, int ArganmehrId, ISettingServices settingService, bool isRootModel = true, ILocalizedModelLocal localized = null, int? index = null)
        {
            if (ArganmehrId <= 0)
                return;		// single Arganmehr mode -> there are no overrides

            var data = Data;
            if (data == null)
                data = new ArganmehrDependingSettingData();

            var settingName = settings.GetType().Name;
            var properties = localized.GetType().GetProperties();
            var localizedEntityService = ProjectObjectFactory.Container.GetInstance<ILocalizedEntityService>();

            var modelType = model.GetType();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var modelProperty = modelType.GetProperty(name);

                if (modelProperty == null)
                    continue;	// setting is not configurable or missing or whatever... however we don't need the override info

                var key = "Locales[" + index.ToString() + "]." + name;

                var resultStr = localizedEntityService.GetLocalizedValue(localized.LanguageId, 0, settingName, name);

                if (!String.IsNullOrEmpty(resultStr))
                    data.OverrideSettingKeys.Add(key);
            }

            if (isRootModel)
            {
                data.ActiveArganmehrScopeConfiguration = ArganmehrId;
                data.RootSettingClass = settingName;

                _viewData[ViewDataKey] = data;
            }
        }

        public void UpdateSettings(object settings, FormCollection form, int ArganmehrId, ISettingServices settingService)
        {
            var settingName = settings.GetType().Name;
            var properties = FastProperty.GetProperties(settings.GetType()).Values;

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var key = settingName + "." + name;

                if (ArganmehrId == 0 || IsOverrideChecked(key, form))
                {
                    dynamic value = prop.GetValue(settings);
                    settingService.SetSetting(key, value == null ? "" : value, ArganmehrId, false);
                }
                else if (ArganmehrId > 0)
                {
                    settingService.DeleteSetting(key, ArganmehrId);
                }
            }
        }

        // DRY?
        public void UpdateLocalizedSettings(object settings, FormCollection form, int ArganmehrId, ISettingServices settingService, ILocalizedModelLocal localized)
        {
            var settingName = settings.GetType().Name;
            var properties = FastProperty.GetProperties(localized.GetType()).Values;

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var key = settingName + "." + name;

                if (ArganmehrId == 0 || IsOverrideChecked(key, form))
                {
                    dynamic value = prop.GetValue(settings);
                    settingService.SetSetting(key, value == null ? "" : value, ArganmehrId, false);
                }
                else if (ArganmehrId > 0)
                {
                    settingService.DeleteSetting(key, ArganmehrId);
                }
            }
        }
    }
}
