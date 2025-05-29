
using IocConfig.Modeling;
using ServiceLayer.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig.Filters
{
    public class JsonNetAttribute : ActionFilterAttribute
    {
        public IDateTimeHelper DateTimeHelper { get; set; }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //if (!DataSettings.DatabaseIsInstalled())
            //    return;

            DateTimeHelper = ProjectObjectFactory.Container.GetInstance<IDateTimeHelper>();

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // handle JsonResult only
            if (filterContext.Result.GetType() != typeof(JsonResult))
                return;

            var jsonResult = filterContext.Result as JsonResult;

            filterContext.Result = new JsonNetResult(DateTimeHelper)
            {
                Data = jsonResult.Data,
                ContentType = jsonResult.ContentType,
                ContentEncoding = jsonResult.ContentEncoding,
                JsonRequestBehavior = jsonResult.JsonRequestBehavior
            };
        }
    }
}
