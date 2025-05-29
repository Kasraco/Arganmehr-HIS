using System;
using System.Collections.ObjectModel;
using AutoMapper;
using AutoMapperProfiles.Extentions;
using DomainClasses;
using DomainClasses.Entities;
using DomainClasses.Entities.Common;
using DomainClasses.Entities.Users;
using Utility;
using ViewModel.Account;
using ViewModel.AdminArea.User;
using EditUserViewModel = ViewModel.AdminArea.User.EditUserViewModel;


namespace AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<DateTime, string>().ConvertUsing(new ToPersianDateTimeConverter());

            CreateMap<User, UserViewModel>()
                .ForMember(d => d.Roles, m => m.Ignore()).IgnoreAllNonExisting();

            CreateMap<AddUserViewModel, User>()
                .ForMember(d => d.RegisterDate, m => m.MapFrom(s => DateTime.Now))
                .ForMember(d => d.LastActivityDate, m => m.MapFrom(s => DateTime.Now))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.Email.FixGmailDots()))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.UserName.ToLower()))
                .IgnoreAllNonExisting();

            CreateMap<EditUserViewModel, User>()
                .ForMember(d => d.Roles, m => m.MapFrom(s=>new Collection<UserRole>()))
                .ForMember(d => d.RegisterDate, m => m.Ignore())
                .ForMember(d => d.LastActivityDate, m => m.Ignore())
                .ForMember(d => d.BirthDay, m => m.Ignore())
                .ForMember(d => d.EmailConfirmed, m => m.Ignore())
                .ForMember(d => d.Email, m => m.Ignore())
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.UserName.ToLower()))
                 .IgnoreAllNonExisting();

            CreateMap<User, EditUserViewModel>().IgnoreAllNonExisting();

            CreateMap<RegisterViewModel, User>()
                .ForMember(d => d.RegisterDate, a => a.MapFrom(s => DateTime.Now))
                .ForMember(d => d.LastActivityDate, m => m.MapFrom(s => DateTime.Now))
                .ForMember(d => d.AvatarFileName, a => a.MapFrom(s => "avatar.jpg"))
                .ForMember(d => d.Email, m => m.MapFrom(s => s.Email.FixGmailDots()))
                .ForMember(d => d.UserName, m => m.MapFrom(s => s.UserName.ToLower()))
                .IgnoreAllNonExisting();

        }

        public override string ProfileName
        {
            get { return GetType().Name; }
        }
    }

}
