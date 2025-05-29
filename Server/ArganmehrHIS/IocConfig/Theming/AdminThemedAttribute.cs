using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig.Theming
{
    public class AdminThemedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                return;

            // add extra view location formats to all view results (even the partial ones)
            filterContext.RouteData.DataTokens["ExtraAreaViewLocations"] = new string[] 
			{
				"~/Administration/Views/{1}/{0}.cshtml",
				"~/Administration/Views/Shared/{0}.cshtml"
			};
        }

    }
}
