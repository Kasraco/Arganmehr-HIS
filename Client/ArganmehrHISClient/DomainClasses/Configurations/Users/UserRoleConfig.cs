using System.Data.Entity.ModelConfiguration;
using DomainClasses.Entities.Users;

namespace DomainClasses.Configurations.Users
{
    public class UserRoleConfig : EntityTypeConfiguration<UserRole>
    {
        public UserRoleConfig()
        {
            HasKey(r => new { r.UserId, r.RoleId });
            ToTable("UserRole");
        }
    }
}
