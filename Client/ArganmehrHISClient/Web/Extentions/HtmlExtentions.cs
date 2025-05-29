using DataLayer.Context;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Web.Extentions
{
    public static class HtmlExtentions
    {
        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, null, routeValues, (IDictionary<string, object>)new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, null, routeValues, new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return SecureActionLink(htmlHelper, linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            //var url = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);

            return CanAccess(htmlHelper, actionName, controllerName)
                ? htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes)
                : new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            return CanAccess(htmlHelper, actionName, controllerName)
                ? htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes)
                : new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return CanAccess(htmlHelper, actionName, controllerName)
                ? htmlHelper.ActionLink(linkText, actionName, controllerName, protocol, hostName, fragment, routeValues, htmlAttributes)
                : new MvcHtmlString(string.Empty);
        }
        //--------------------------------------------------
        public static void SecureRenderAction(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
                 htmlHelper.RenderAction(actionName, controllerName);
                 return;
        }

        private static bool CanAccess(HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            var dbContext = new ApplicationDbContext();
            var user = httpContext.User;

            var userId = user.Identity.GetUserId<int>();
            var roleIds = dbContext.Roles.Where(r => r.Users.Any(u => u.UserId == userId)).Select(r => r.Id);

            var roleAccess = from ra in dbContext.RoleAccesses
                             where roleIds.Contains(ra.RoleId)
                             select ra;

            if (string.IsNullOrWhiteSpace(controllerName))
                controllerName = htmlHelper.ViewContext.Controller.ToString().Split('.').Last().Replace("Controller", "");

            if (roleAccess.Any(ra =>
                ra.Controller.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase) &&
                ra.Action.Equals(actionName, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            return false;
        }
    }
}