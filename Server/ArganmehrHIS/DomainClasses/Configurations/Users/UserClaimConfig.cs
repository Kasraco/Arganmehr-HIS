using System.Data.Entity.ModelConfiguration;
using DomainClasses.Entities.Users;

namespace DomainClasses.Configurations.Users
{
    public class UserClaimConfig : EntityTypeConfiguration<UserClaim>
    {
        public UserClaimConfig()
        {
            ToTable("UserClaims");
        }
    }
}
