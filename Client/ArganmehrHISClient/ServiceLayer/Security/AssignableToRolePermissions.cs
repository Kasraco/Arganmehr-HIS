using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Web;

namespace ServiceLayer.Security
{
    public static class AssignableToRolePermissions
    {
        #region Fields

        private static Lazy<IEnumerable<ControllerDescription>> _permissionsLazy =
             new Lazy<IEnumerable<ControllerDescription>>(GetPermision, LazyThreadSafetyMode.ExecutionAndPublication);
       


   
        #endregion

        #region permissionNames
        public static bool AllowSendPrivateMessage { get; set; }
        public static bool AllowSendNewsItem { get; set; }
        public static bool AllowSendBlogPostDraft { get; set; }
        public static bool AllowSendPollItem { get; set; }
        public static bool AllowSendFriendRequest { get; set; }
        public static bool CanUploadFile { get; set; }
        public static bool CanChangeProfilePicture { get; set; }
        public static bool CanModifyFirsAndLastName { get; set; }
        public const string CanEditRole = "CanEditRole";
        public const string CanDeleteRole = "CanDeleteRole";
        public const string CanViewRolesList = "CanViewRolesList";
        public const string CanCreateRole = "CanCreateRole";
        public const string CanEditUser = "CanEditUser";
        public const string CanCreateUser = "CanCreateUser";
        public const string CanDeleteUser = "CanDeleteUser";
        public const string CanSoftDeleteUser = "CanSoftDeleteUser";
        public const string CanViewUsersList = "CanViewUsersList";
        public const string CanEditUsersSetting = "CanEditUsersSetting";
        public const string CanAccessImages = "CanAccessImages";
        public const string CanViewAdminPanel = "CanViewAdminPanel";
        public const string CanAccessUsersFiles = "CanAccessUsersFiles";
        public const string CanAccessUsersAvatar = "CanAccessUsersAvatar";
        #endregion //permissions

        #region Permissions

        public static readonly PermissionModel CanEditRolePermission = new PermissionModel { Name = CanEditRole, Description = "میتوانند گروه کاربری را ویرایش کنند" };
        public static readonly PermissionModel CanDeleteRolePermission = new PermissionModel { Name = CanDeleteRole, Description = "میتوانند گروه کاربری را حذف کنند" };
        public static readonly PermissionModel CanViewRolesListPermission = new PermissionModel { Name = CanViewRolesList, Description = "میتوانند لیست گروه های کاربری را مشاهده کنند" };
        public static readonly PermissionModel CanCreateRolePermission = new PermissionModel { Name = CanCreateRole, Description = "میتوانند گروه کاربری جدید ایجاد کنند" };
        public static readonly PermissionModel CanEditUserPermission = new PermissionModel { Name = CanEditUser, Description = "میتوانند مشخصات کاربر را ویرایش کنند" };
        public static readonly PermissionModel CanCreateUserPermission = new PermissionModel { Name = CanCreateUser, Description = "میتوانند کاربر جدید ایجاد کنند" };
        public static readonly PermissionModel CanViewUsersListPermission = new PermissionModel { Name = CanViewUsersList, Description = "میتوانند لیست کاربران را مشاهده کنند" };
        public static readonly PermissionModel CanDeleteUserPermission = new PermissionModel { Name = CanDeleteUser, Description = "میتوانند کاربر را حذف کنند" };
        public static readonly PermissionModel CanSoftDeleteUserPermission = new PermissionModel { Name = CanSoftDeleteUser, Description = "میتوانند کاربر را به صورت منطقی حذف کنند" };
        public static readonly PermissionModel CanViewAdminPanelPermission = new PermissionModel { Name = CanViewAdminPanel, Description = "میتوانند پنل مدیریت را مشاهده کنند" };
        public static readonly PermissionModel CanEditUsersSettingPermission = new PermissionModel { Name = CanEditUsersSetting, Description = "میتوانند تنظیمات کاربران را ویرایش کنند" };
        public static readonly PermissionModel CanAccessImagesPermission = new PermissionModel { Name = CanAccessImages, Description = "میتوانند به تصاویر دسترسی داشته باشند" };
        public static readonly PermissionModel CanAccessUsersAvatarPermission = new PermissionModel { Name = CanAccessUsersAvatar, Description = "میتوانند به تصاویر پروفایل کاربرن دسترسی داشته باشند" };
        public static readonly PermissionModel CanAccessUsersFilesPermission = new PermissionModel { Name = CanAccessUsersFiles, Description = "میتوانند فایل های ضمیمه شده توسط کاربران را دانلود کنند" };
        #endregion

        #region Properties
        public static IEnumerable<ControllerDescription> Permissions
        {
            get
            {
                return _permissionsLazy.Value;
            }
        }
        #endregion


        #region GetAllPermisions
        private static IEnumerable<ControllerDescription> GetPermision()
        {
            if (_permissionsLazy.IsValueCreated)
                return _permissionsLazy.Value;

            _permissionsLazy = new Lazy<IEnumerable<ControllerDescription>>(() => new ControllerHelper().GetControllersNameAnDescription());
            return _permissionsLazy.Value;
        }

        #endregion

        #region GetAsSelectedList
        public static IEnumerable<SelectListItem> GetAsSelectListItems()
        {
            SelectListItem SLI;
            List<SelectListItem> Lsli = new List<SelectListItem>();
            foreach (var c in Permissions)
                foreach (var a in c.Actions)
                {
                    SLI = new SelectListItem { Value = string.Format("{0}-{1}", c.Name, a.Name), Text = string.Format("{0}-{1}", c.Description, a.Description) };
                    Lsli.Add(SLI);
                }

            return Lsli;
        }
        #endregion
    }
}
