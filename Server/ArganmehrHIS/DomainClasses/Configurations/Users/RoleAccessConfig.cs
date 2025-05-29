using DomainClasses.Entities.Users;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Configurations.Users
{
    public class RoleAccessConfig : EntityTypeConfiguration<RoleAccess>
    {
        public RoleAccessConfig()
        {
            Property(ra => ra.Action).IsUnicode(false).HasMaxLength(70).IsRequired();
            Property(ra => ra.Controller).IsUnicode(false).HasMaxLength(70).IsRequired();
            HasRequired(ra => ra.Role).WithMany(r => r.RoleAccesses).HasForeignKey(ra => ra.RoleId);

        }
    }
}
