using DataLayer.Context;
using DomainClasses.Entities.Users;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ServiceLayer.Security
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ActivityLogAttribute : ActionFilterAttribute, IActionFilter
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log(filterContext);
        }


        private void Log(ActionExecutingContext ctx)
        {
             int userId=0;
            if (ctx == null)
                throw new ArgumentNullException("ActionExecutingContext");

            var user = ctx.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return;
              
            }

            var dbContext = new ApplicationDbContext();
            UserActivityLog UAL = new UserActivityLog();

            userId = user.Identity.GetUserId<int>();

            ActivityLogType ALT1 = GetUserType(dbContext, Name);
            if (ALT1 != null)
            {
                UAL.LogType = ALT1;
                UAL.LogTypeId = ALT1.Id;
                UAL.AreaName = GetAreaName(ctx.RouteData);
                UAL.URL = GetBaseUrl();
                UAL.ControllerName = ctx.ActionDescriptor.ControllerDescriptor.ControllerName;
                UAL.ActionMethod = ctx.ActionDescriptor.ActionName;
                UAL.IP = ctx.HttpContext.Request.UserHostAddress;
                UAL.CreateDate = ctx.HttpContext.Timestamp;
                UAL.UserId = userId;
                UAL.Message = string.IsNullOrEmpty(Description) ? UAL.ActionMethod : Description;
                UAL.Name = string.IsNullOrEmpty(Name) ? UAL.ControllerName : Name;

                dbContext.UserActivityLogs.Add(UAL);
                dbContext.SaveChanges();
            }

        }

        private ActivityLogType GetUserType(ApplicationDbContext dbContext, string Title)
        {
            ActivityLogType ALT;

            var qdb = dbContext.ActivityLogTypes.Where(x => Title.Contains(x.Name)).ToList();
            if (qdb.Count() > 0)
            {
                ALT = qdb.First();
                return ALT;
            }

            return null;

        }

        private string GetAreaName(RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as Route;
            if ((route2 != null) && (route2.DataTokens != null))
            {
                return (route2.DataTokens["area"] as string);
            }
            return null;
        }


        private string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/") appUrl += "/";

            var baseUrl = string.Format("{0}", request.Url.ToString());

            return baseUrl;
        }

        private string GetAreaName(RouteData routeData)
        {
            object obj2;
            if (routeData.DataTokens.TryGetValue("area", out obj2))
            {
                return (obj2 as string);
            }
            return GetAreaName(routeData.Route);
        }

    }
}
