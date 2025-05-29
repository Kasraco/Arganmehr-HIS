using System;

namespace DomainClasses.Entities.Users
{
    /// <summary>
    /// Represents user's Activity log
    /// </summary>
    public class UserActivityLog
    {
        #region Ctor
        public UserActivityLog()
        {
            Id = Guid.NewGuid().ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// gets or sets Id Of user's ActivityLog
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// gets or sets message of activity
        /// </summary>
        public virtual string Message { get; set; }

        public virtual string Name { get; set; }

        public virtual string IP { get; set; }

        public virtual string URL { get; set; }

        public virtual string AreaName { get; set; }
        public virtual string ControllerName { get; set; }

        public virtual string ActionMethod { get; set; }
        /// <summary>
        /// gets or sets date that  activity done
        /// </summary>
        public virtual DateTime CreateDate { get; set; }
        #endregion

        #region NavigationProperties
        /// <summary>
        /// gets or sets type of activitylog
        /// </summary>
        public virtual ActivityLogType LogType { get; set; }
        /// <summary>
        /// gets or sets typeId of activitylog
        /// </summary>
        public virtual int LogTypeId { get; set; }
        /// <summary>
        /// gets or sets userId that did this activity
        /// </summary>
        public virtual long UserId { get; set; }
        /// <summary>
        /// gets or sets user that did this activity
        /// </summary>
        public virtual User User { get; set; }
        #endregion
    }
}
