using DomainClasses.Localization;
using IocConfig.Filters;
using IocConfig.Localization;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Helpers.Extentions;
using DomainClasses.Entities.Logg;
using ServiceLayer.Logger;


namespace IocConfig.Controllers
{
    [SetWorkingCulture]

    [JsonNet]
    public abstract partial class SmartController : Controller
    {
        protected SmartController()
        {
            this.Logger = NullLogger.Instance;
            this.T = NullLocalizer.Instance;
            this.Services = ProjectObjectFactory.Container.GetInstance<ICommonServices>();
        }

        public ILogger Logger
        {
            get;
            set;
        }

        public Localizer T
        {
            get;
            set;
        }

        public ICommonServices Services
        {
            get;
            set;
        }

        //protected virtual void NotifyInfo(string message, bool durable = true)
        //{
        //    Services.Notifier.Information(message, durable);
        //}


        //protected virtual void NotifyWarning(string message, bool durable = true)
        //{
        //    Services.Notifier.Warning(message, durable);
        //}


        //protected virtual void NotifySuccess(string message, bool durable = true)
        //{
        //    Services.Notifier.Success(message, durable);
        //}


        //protected virtual void NotifyError(string message, bool durable = true)
        //{
        //    Services.Notifier.Error(message, durable);
        //}


        protected virtual void NotifyError(Exception exception, bool durable = true, bool logException = true)
        {
            if (logException)
            {
                LogException(exception);
            }

       //     Services.Notifier.Error(HttpUtility.HtmlEncode(exception.ToAllMessages()), durable);
        }


        protected virtual void NotifyAccessDenied(bool durable = true, bool log = true)
        {
            var message = T("Admin.AccessDenied.Description");

            if (log)
            {
                Logger.Error(message, null, Services.WorkContext.CurrentUser);
            }

          //  Services.Notifier.Error(message, durable);
        }

        protected virtual ActionResult RedirectToReferrer()
        {
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().HasValue())
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return RedirectToRoute("HomePage");
        }

        protected virtual ActionResult RedirectToHomePageWithError(string reason, bool durable = true)
        {
            string message = T("Common.RequestProcessingFailed", this.RouteData.Values["controller"], this.RouteData.Values["action"], reason.NaIfEmpty());

          //  Services.Notifier.Error(message, durable);

            return RedirectToRoute("HomePage");
        }


        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                LogException(filterContext.Exception);
            }

            base.OnException(filterContext);
        }


        private void LogException(Exception exc)
        {
            var customer = Services.WorkContext.CurrentUser;
            Logger.Error(exc.Message, exc, customer);
        }

        
    }
}
