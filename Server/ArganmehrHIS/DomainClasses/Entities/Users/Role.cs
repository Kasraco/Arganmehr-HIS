using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DomainClasses.Entities.Users
{

    public class Role : IdentityRole<long, UserRole>
    {
        #region Ctor
        
        #endregion
        #region Properties
        
        #endregion

        #region NavigationProperties
        
        #endregion
        public Role()
        {
        }
        public string Description { get; set; }
        public virtual bool IsSystemRole { get; set; }
        public virtual bool IsDefaultForRegister { get; set; }
        public virtual byte[] RowVersion { get; set; }
        public virtual long UsersCount { get; set; }
        public virtual bool IsSuperAdministrator { get; set; }
        public virtual int QuotaorumTopicsInDay { get; set; }
        public virtual int QuotaAttachment { get; set; }
        public virtual bool IsBanned { get; set; }
        public virtual int QuotaConversations { get; set; }
        public virtual int MaxMembersCountCanSendMessageEachSend { get; set; }
        public virtual int MaxForumPostsForEnableSignature { get; set; }
        public virtual ICollection<RoleAccess> RoleAccesses { get; set; }
      
    }
}
