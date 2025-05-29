using ServiceLayer.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IocConfig.Localization
{
    public class SetWorkingCultureAttribute : FilterAttribute, IAuthorizationFilter
    {
        public IWorkContext WorkContext { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //if (!DataSettings.DatabaseIsInstalled())
            //    return;
            WorkContext = ProjectObjectFactory.Container.GetInstance<IWorkContext>();
            var workContext = WorkContext;
            var workingLanguage = workContext.WorkingLanguage;

            CultureInfo culture;
         
                culture = new CultureInfo("fa-IR");
            

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

    }
}
