using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using DomainClasses.Entities.Users;

namespace DomainClasses.Configurations.Users
{
    public class ActivityLogTypeConfig : EntityTypeConfiguration<ActivityLogType>
    {
        public ActivityLogTypeConfig()
        {
            ToTable("ActivityLogTypes");
            Property(a => a.Name).HasMaxLength(50).IsRequired().HasColumnAnnotation("Index",
                new IndexAnnotation(new IndexAttribute("IX_ActivityLogName") { IsUnique = true }));
            HasMany(a => a.ActivityLogs)
                .WithRequired(a => a.LogType)
                .HasForeignKey(a => a.LogTypeId)
                .WillCascadeOnDelete(true);
        }
    }
}
