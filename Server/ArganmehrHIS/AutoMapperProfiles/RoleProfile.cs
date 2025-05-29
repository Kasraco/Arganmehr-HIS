using System.Web.Mvc;
using AutoMapper;
using AutoMapperProfiles.Extentions;
using DomainClasses.Entities;
using ViewModel.AdminArea.Role;
using System.Collections.ObjectModel;
using DomainClasses.Entities.Users;

namespace AutoMapperProfiles
{
    public class RoleProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Role, RoleViewModel>().IgnoreAllNonExisting();
            CreateMap<AddRoleViewModel, Role>()
                .IgnoreAllNonExisting();

            CreateMap<RoleViewModel, Role>().IgnoreAllNonExisting();

            CreateMap<EditRoleViewModel, Role>()
                .ForMember(d => d.IsSystemRole, m => m.Ignore())
                .ForMember(d => d.IsDefaultForRegister, m => m.Ignore())
               .IgnoreAllNonExisting();

            CreateMap<Role, EditRoleViewModel>().IgnoreAllNonExisting();
            CreateMap<Role, SelectListItem>()
                .ForMember(d => d.Text, m => m.MapFrom(s => s.Name))
                .ForMember(d => d.Value, m => m.MapFrom(s => s.Id));
        }

        public override string ProfileName
        {
            get { return this.GetType().Name; }
        }
    }
}
