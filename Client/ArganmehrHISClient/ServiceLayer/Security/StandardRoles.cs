using System;
using System.Collections.Generic;
using Web;

namespace ServiceLayer.Security
{
    public static class StandardRoles
    {
        #region Fields

        private static Lazy<IEnumerable<PermissionRecord>> _rolesWithPermissionsLazy =
            new Lazy<IEnumerable<PermissionRecord>>();
        private static Lazy<IEnumerable<string>> _rolesLazy = new Lazy<IEnumerable<string>>();
        #endregion

        #region Properties
        public static IEnumerable<string> SystemRoles
        {
            get
            {
                if (_rolesLazy.IsValueCreated)
                    return _rolesLazy.Value;
                _rolesLazy = new Lazy<IEnumerable<string>>(GetSysmteRoles);
                return _rolesLazy.Value;
            }
        }

        public static IEnumerable<PermissionRecord> SystemRolesWithPermissions
        {
            get
            {
                if (_rolesWithPermissionsLazy.IsValueCreated)
                    return _rolesWithPermissionsLazy.Value;
                _rolesWithPermissionsLazy = new Lazy<IEnumerable<PermissionRecord>>(GetDefaultRolesWithPermissions);
                return _rolesWithPermissionsLazy.Value;
            }
        }
        #endregion

        #region DefaultRoles
        public const string SuperAdministrators = "مدیران ارشد";
        public const string Writers = "نویسندگان سایت";
        public const string ForumAdmins = "مدیران انجمن";
        public const string RegisterdUsers = "کاربران عضو شده";
        public const string Moderators = "مدیران";
        public const string BlogAdmins = "مدیران وبلاگ";
        public const string NewsAdmins = "مدیران اخبار";
        public const string PollsAdmins = "مدیران نظرسنجی ها";
        #endregion

        #region GetSysmteRoles
        private static IEnumerable<string> GetSysmteRoles()
        {
            return new List<string>
            {
                SuperAdministrators,
                Writers,
                ForumAdmins,
                RegisterdUsers,
                Moderators,
                BlogAdmins,
                NewsAdmins,
                PollsAdmins

            };
        }
        #endregion

        #region GetDefaultRolesWithPermissions
        private static IEnumerable<PermissionRecord> GetDefaultRolesWithPermissions()
        {
            return new List<PermissionRecord>
            {
                new PermissionRecord
                {
                    RoleName = SuperAdministrators,
                    Permissions = AssignableToRolePermissions.Permissions
                }
            };
        }
        #endregion
       
    }
}
