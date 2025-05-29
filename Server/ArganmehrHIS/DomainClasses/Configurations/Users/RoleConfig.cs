using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using DomainClasses.EF.Filters;
using DomainClasses.Entities.Users;
using EntityFramework.Filters;

namespace DomainClasses.Configurations.Users
{
    public class RoleConfig : EntityTypeConfiguration<Role>
    {
        public RoleConfig()
        {
            ToTable("Roles");
            Property(r => r.Name)
                 .IsRequired()
                 .HasMaxLength(256)
                 .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_RoleName") { IsUnique = true }));
            Property(r => r.RowVersion).IsRowVersion();
            this.Filter(RoleFilters.ActiveList, a => a.Condition(u => !u.IsBanned));            
            HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
        }
    }
}
