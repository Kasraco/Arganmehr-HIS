
using DomainClasses;
using DomainClasses.Entities.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Localization
{
    public interface ILocalizedEntityService
    {

        void DeleteLocalizedProperty(LocalizedProperty localizedProperty);


        LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId);


        string GetLocalizedValue(int languageId, int entityId, string localeKeyGroup, string localeKey);


        IList<LocalizedProperty> GetLocalizedProperties(int entityId, string localeKeyGroup);


        void InsertLocalizedProperty(LocalizedProperty localizedProperty);


        void UpdateLocalizedProperty(LocalizedProperty localizedProperty);


        void SaveLocalizedValue<T>(T entity,
            Expression<Func<T, string>> keySelector,
            string localeValue,
            int languageId) where T : BaseEntity, ILocalizedEntity;

        void SaveLocalizedValue<T, TPropType>(T entity,
           Expression<Func<T, TPropType>> keySelector,
           TPropType localeValue,
           int languageId) where T : BaseEntity, ILocalizedEntity;
    }
}
