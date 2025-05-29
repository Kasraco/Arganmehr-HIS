using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainClasses.Entities.Common;

namespace DomainClasses.Configurations.Common
{
    public class UserRatingConfig : EntityTypeConfiguration<UserRating>
    {
        public UserRatingConfig()
        {
            Property(ur => ur.SectionId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Single_UserRating") { IsUnique = true, Order = 1 }));
            Property(ur => ur.SectionType).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Single_UserRating") { IsUnique = true, Order = 2 }));
            Property(ur => ur.RaterId).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Single_UserRating") { IsUnique = true, Order = 3 }));
        }
    }
}
