using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using DomainClasses.Entities.Users;

namespace DomainClasses.Configurations.Users
{
    public class ActivityLogConfig : EntityTypeConfiguration<UserActivityLog>
    {
        public ActivityLogConfig()
        {
            ToTable("ActivityLogs");
            HasRequired(a => a.User)
                .WithMany(a => a.ActivityLogs)
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(true);

            Property(a => a.Message).HasMaxLength(1024).IsRequired();
            Property(a => a.CreateDate).IsRequired();
        }
    }
}
