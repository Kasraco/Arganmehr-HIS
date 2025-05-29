using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using DomainClasses.EF.Filters;
using DomainClasses.Entities.Users;
using EntityFramework.Filters;

namespace DomainClasses.Configurations.Users
{
    public class UserConfig : EntityTypeConfiguration<User>
    {
        public UserConfig()
        {
            ToTable("Users");
            HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
           
            HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            Property(u => u.AdminComment).IsOptional().HasMaxLength(1024);
            Property(u => u.LastIp).IsOptional().HasMaxLength(20);
            Property(u => u.RowVersion).IsRowVersion();
            Property(u => u.NameForShow).IsRequired().HasMaxLength(256);
            Property(u => u.AvatarFileName).IsRequired().HasMaxLength(256);
            Property(u => u.FaceBookId).IsOptional().HasMaxLength(50);
            Property(u => u.GooglePlusId).IsOptional().HasMaxLength(50);
            Property(u => u.PhoneNumber).IsOptional().HasMaxLength(20);
            Property(u => u.DirectPermissions).HasColumnType("xml");
            Ignore(u => u.XmlDirectPermissions);
            Ignore(u => u.ConnectionIds);
            Property(u => u.UserName)
                 .IsRequired()
                 .HasMaxLength(256)
                 .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UserName") { IsUnique = true }));

            this.Filter(UserFilters.DeletedList, a => a.Condition(u => u.IsDeleted))
                .Filter(UserFilters.BannedList, a => a.Condition(u => u.IsBanned))
                .Filter(UserFilters.SystemAccountList, a => a.Condition(u => u.IsSystemAccount))
                .Filter(UserFilters.EmailConfirmedList, a => a.Condition(u => u.EmailConfirmed))
                .Filter(UserFilters.ActiveList, a => a.Condition(u => !u.IsBanned))
                .Filter(UserFilters.NotSystemAccountList, a => a.Condition(u => !u.IsSystemAccount));

            Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_UserEmail") {IsUnique = true}));


        }
    }
}
