using System.Collections.Generic;

namespace DomainClasses.Entities.Users
{
    public class ActivityLogType
    {
        //public ActivityLogType()
        //{
        //    ActivityLogs=new HashSet<UserActivityLog>();
        //}
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; }
    }
}
