using AutoMapper;
using AutoMapperProfiles.Extentions;
using DomainClasses.Entities.AM;
using DomainClasses.Entities.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Arganmehr;

namespace AutoMapperProfiles
{
    public class ArganmehrProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<LanguageModel, Language>()
                .ForMember(d=>d.Name ,mo=>mo.MapFrom(s=>s.LanguageName))
                .IgnoreAllNonExisting();
            CreateMap<Language, LanguageModel>()
                .ForMember(d => d.LanguageName, mo => mo.MapFrom(s => s.Name))
                .IgnoreAllNonExisting();


            CreateMap<InstitutionModel, Institution>().IgnoreAllNonExisting();
            CreateMap<Institution, InstitutionModel>().IgnoreAllNonExisting();

        }

        public override string ProfileName
        {
            get { return this.GetType().Name; }
        }
    }
}
