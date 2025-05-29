//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using System.Web.Mvc;

//namespace Common.Filters
//{
//    [AttributeUsage(AttributeTargets.Method)]
//    public class ActivityLogAttribute : ActionFilterAttribute, IActionFilter
//    {
//        public string Name { get; set; }
//        public string Description { get; set; }

//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            Log("OnActionExecuting", filterContext);
//        }

//        public override void OnActionExecuted(ActionExecutedContext filterContext)
//        {
//            Log("OnActionExecuted", filterContext);
//        }

//        public override void OnResultExecuting(ResultExecutingContext filterContext)
//        {
//            Log("OnResultExecuting", filterContext);
//        }

//        public override void OnResultExecuted(ResultExecutedContext filterContext)
//        {
//            Log("OnResultExecuted", filterContext);
//        }

//        private void Log(string stage, ControllerContext ctx)
//        {
//            ctx.HttpContext.Response.Write(
//                string.Format("{0}:{1} - {2} < br/> ",
//                ctx.RouteData.Values["controller"], ctx.RouteData.Values["action"], stage));
//        }

//    }
//}
