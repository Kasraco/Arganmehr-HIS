using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DomainClasses.Entities.Users
{

    public class User : IdentityUser<long, UserLogin, UserRole, UserClaim>
    {
        #region Ctor
        public User()
        {
            ActivityLogs = new HashSet<UserActivityLog>();

        }
        #endregion

        #region Properties

        public virtual bool AutoConvertDraftsToLive { get; set; }
        public virtual bool IsBanned { get; set; }
        public virtual DateTime? LastVisit { get; set; }
        public string NameForShow { get; set; }
        public virtual int BlogPostsCount { get; set; }
        public virtual int NewsItemsCount { get; set; }
        public virtual int PollItemsCount { get; set; }
        public virtual int AnnouncementsCount { get; set; }
        public virtual int ForumTopicsCount { get; set; }
        public virtual int LearningPathsCount { get; set; }
        public virtual int BacklogsCount { get; set; }
        public virtual int CoursesCount { get; set; }
        public virtual int CourseFeedbacksCount { get; set; }
        public virtual int BlogPostCommentsCount { get; set; }
        public virtual int AnnouncementCommentsCount { get; set; }
        public virtual int NewsCommentsCount { get; set; }
        public virtual int PollCommentsCount { get; set; }
        public virtual int ForumPostsCount { get; set; }
        public virtual int AttachmentsCount { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual bool IsSystemAccount { get; set; }
        public virtual string AdminComment { get; set; }
        public virtual bool SuspensionComments { get; set; }
        public virtual bool SuspensionForumPosts { get; set; }
        public virtual bool SuspensionForumTopics{ get; set; }
        public virtual bool ModerateCommentsOfUser { get; set; }
        public virtual bool ModerateForumTopicsOfUser { get; set; }
        public virtual bool ModerateForumPostsOfUser { get; set; }
        public virtual string AvatarFileName { get; set; }
        public virtual DateTime? ComeBackToForumDate { get; set; }
        public virtual bool InvisibleInForum { get; set; }
        public virtual string InvisibleReason { get; set; }
        public virtual bool DontShowInOnlineUsersList { get; set; }
        public virtual bool ReciveEmailFromAdmins { get; set; }
        public virtual bool ShowImagesInForumTopics { get; set; }
        public virtual string DisableSignatureByAdmin { get; set; }
        public virtual string Signature { get; set; }
        public virtual string ShowMySignature { get; set; }
        public virtual DateTime? BirthDay { get; set; }
        public virtual DateTime RegisterDate { get; set; }
        public virtual string GooglePlusId { get; set; }
        public virtual string FaceBookId { get; set; }
        public virtual DateTime? BannedDate { get; set; }
        public virtual string LastIp { get; set; }
        public virtual string RegistrationIp { get; set; }
        public virtual DateTime? LastLoginDate { get; set; }
        public virtual DateTime LastActivityDate { get; set; }
        public virtual string ShowEmailAddressAsIamge { get; set; }
        public virtual bool DefaultSubscribeToForumTopicPosted { get; set; }
        public virtual bool NotifyMeFromMyNewFeedbackToMyCourse { get; set; }
        public virtual bool NotifyMeFromSystemNotificationsAsPopup { get; set; }
        public virtual bool NotifyMeFromPrivateMessageNotificationAsPopup { get; set; }
        public virtual bool ReciveMessageJustFormFriend { get; set; }
        public virtual bool NotifyMeFromSystemNotificationAsEmail { get; set; }
        public virtual bool NotifyMeFromPrivateMessageNotificationAsEmail { get; set; }
        public virtual bool SubscribeToNewsLetters { get; set; }
        public virtual bool ReciveWeeklyNewPosts { get; set; }
        public virtual bool ReciveWeeklyNewNewsItems { get; set; }
        public virtual bool ReciveWeeklyNewPolling { get; set; }
        public virtual bool ReciveWeeklyNewCourses { get; set; }
        public virtual bool NotifyMeFromMyForumTopics { get; set; }
        public virtual bool NotifyMeFromNewCommentToMyBlogPost { get; set; }
        public virtual bool NotifyMeFromNewReplyToMyComments { get; set; }
        public virtual int TotalReputation { get; set; }
        public virtual int TotalWarningsByAdmin { get; set; }
        public virtual bool IsChangedPermissions { get; set; }
        public virtual string DirectPermissions { get; set; }
        public XElement XmlDirectPermissions
        {
            get { return XElement.Parse(DirectPermissions); }
            set { DirectPermissions = value.ToString(); }
        }
        /// <summary>
        /// gets or sets connectionIds of current user but don't map to db
        /// </summary>
        public HashSet<string> ConnectionIds { get; set; }
        public virtual byte[] RowVersion { get; set; }

        #endregion

        #region NavigationProperties
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; }
        #endregion
    }
}
