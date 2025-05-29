using System.Data.Entity.ModelConfiguration;
using DomainClasses.Entities.Users;

namespace DomainClasses.Configurations.Users
{
   public class UserLoginConfig:EntityTypeConfiguration<UserLogin>
   {
       public UserLoginConfig()
       {
           HasKey(l => new {l.LoginProvider, l.ProviderKey, l.UserId});
               ToTable("UserLogins");

       }
    }
}
