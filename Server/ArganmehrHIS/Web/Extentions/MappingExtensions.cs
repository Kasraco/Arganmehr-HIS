using AutoMapper;
using DomainClasses.Entities.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewModel.AdminArea.Arganmehr;

namespace Web.Extentions
{
    public static class MappingExtensions
    {
        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }

        #endregion
    }
}