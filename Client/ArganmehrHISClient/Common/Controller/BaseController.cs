using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Controller;
using Common.Controller.NotyHelper;
using System.Configuration;
using System.Web;
using System.Threading;
using System.Globalization;

namespace Common.Controller
{
    public class BaseController : System.Web.Mvc.Controller
    {
        private const string LanguageCookieName = "MyLanguageCookieName";
        //protected override void ExecuteCore()
        //{
        //    //var cookie = HttpContext.Request.Cookies[LanguageCookieName];
        //    //string lang;
        //    //if (cookie != null)
        //    //{
        //    //    lang = cookie.Value;
        //    //}
        //    //else
        //    //{
        //    //    lang = ConfigurationManager.AppSettings["DefaultCulture"] ?? "fa-IR";
        //    //    var httpCookie = new HttpCookie(LanguageCookieName, lang) { Expires = DateTime.Now.AddYears(1) };
        //    //    HttpContext.Response.SetCookie(httpCookie);
        //    //}
        //    //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
        //    base.ExecuteCore();
        //}

        //protected override bool DisableAsyncSupport
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        #region Validation
        [ChildActionOnly]
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        #endregion

    }
}

