using DataLayer.Context;
using DomainClasses.Entities.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;


namespace ServiceLayer.Security
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute,  IAuthorizationFilter
    {
        private string _requestControllerName;
        private string _requestedActionName;


        public CustomAuthorizeAttribute()
        {
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _requestControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            _requestedActionName = filterContext.ActionDescriptor.ActionName;

            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            var dbContext = new ApplicationDbContext();

            var userId = user.Identity.GetUserId<int>();
            var roleIds = dbContext.Roles.Where(r => r.Users.Any(u => u.UserId == userId)).Select(r => r.Id).ToList();
            
            var roleAccess = from ra in dbContext.RoleAccesses                            
                             where roleIds.Contains(ra.RoleId)
                             select ra;

            if (roleAccess.Any(ra =>
                ra.Controller.Equals(_requestControllerName, StringComparison.InvariantCultureIgnoreCase) &&
                ra.Action.Equals(_requestedActionName, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(403);
                return;
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
