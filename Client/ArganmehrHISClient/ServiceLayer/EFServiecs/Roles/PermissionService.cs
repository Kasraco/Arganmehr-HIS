using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ServiceLayer.Contracts;
using DomainClasses.Entities.Users;
using System.Threading.Tasks;
using DataLayer.Context;
using System.Data.Entity;
using ServiceLayer.Security;

namespace ServiceLayer.EFServiecs.Roles
{
    public class PermissionService : IPermissionService
    {
        #region Fields

        private const string PermissionsElement = "Permissions";
        private const string PermissionElement = "Permission";

        #endregion

        #region Ctor

        public PermissionService()
        {
        }
        #endregion

        #region GetPermissionsAsXml
        public System.Xml.Linq.XElement GetPermissionsAsXml(params string[] permissionNames)
        {
            var permissionsAsXml = new XElement(PermissionsElement);
            foreach (var permissionName in permissionNames)
            {
                permissionsAsXml.Add(new XElement(PermissionElement, permissionName));
            }
            return permissionsAsXml;
        }

        public ICollection<RoleAccess> GetPermissions(IEnumerable<Web.ControllerDescription> SelectedControllers)
        {
            ICollection<RoleAccess> lstRA = new List<RoleAccess>();
            RoleAccess RA;
            foreach (var controller in SelectedControllers)
            {
                foreach (var action in controller.Actions)
                {
                    RA=new RoleAccess { Controller = controller.Name, Action = action.Name };
                    lstRA.Add(RA);
                }
            }

            return lstRA;
        }


        #endregion

        #region GetUserPermissionsAsList
        public IList<string> GetUserPermissionsAsList(XElement permissionsAsXml)
        {
            return permissionsAsXml.Elements(PermissionElement).Select(a => a.Value).ToList();
        }

        #endregion


        public IList<string> GetUserPermissionsAsList(IList<XElement> permissionsAsXmls)
        {
            var permissions = new List<string>();
            foreach (var permissionsAsXml in permissionsAsXmls.Where(permissionsAsXml => permissionsAsXml != null))
            {
                permissions.AddRange(permissionsAsXml.Elements(PermissionElement).Select(a => a.Value).ToList());
            }
            return permissions;
        }


        public IList<string> GetUserPermissionsAsList(IEnumerable<string> RoleAccesses)
        {
            return RoleAccesses.ToList();
        }


        public IList<string> GetUserPermissionsAsList(ICollection<RoleAccess> RoleAccesses)
        {

            var q = from p in RoleAccesses
                    select new
                    {
                        Permissions = p.Controller + "-" + p.Action
                    };
            return q.Select(x => x.Permissions).ToList();
        }


        public ICollection<RoleAccess> GetPermissions(params string[] permissionNames)
        {
            var PermisstionControllers = AssignableToRolePermissions.Permissions;
            var RoleAccesses = GetPermissions(PermisstionControllers);


            List<RoleAccess> lstRoleAccesses=new List<RoleAccess>();
            RoleAccess RA;
            string[] strsplit = { "-" };
            string[] strPermissions;
            foreach(var p in permissionNames)
            {
                strPermissions = p.Split(strsplit, System.StringSplitOptions.None);
                var query = RoleAccesses.Where(x => x.Controller == strPermissions[0] && 
                                                   x.Action == strPermissions[1]).ToList();
                foreach(var t in query)
                {
                    lstRoleAccesses.Add(t);
                }
            }
            return lstRoleAccesses.Distinct().ToList();
        }

    }
}
