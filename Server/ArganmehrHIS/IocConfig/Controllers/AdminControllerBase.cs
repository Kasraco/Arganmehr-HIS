using IocConfig.Localization;
using IocConfig.Theming;
using ServiceLayer.Context;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace IocConfig.Controllers
{

   [AdminThemed]
    public abstract class AdminControllerBase : SmartController
    {
     
        //protected override void Initialize(RequestContext requestContext)
        //{
        //    var routeData = requestContext.RouteData;
        //    if (routeData != null && !routeData.DataTokens.ContainsKey("ParentActionViewContext"))
        //    {
        //        ProjectObjectFactory.Container.GetInstance<IWorkContext>().IsAdmin = true;
        //    }
        //    base.Initialize(requestContext);
        //}

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            AddLocales(languageService, locales, null);
        }

        /// <summary>
        /// Add locales for localizable entities
        /// </summary>
        /// <typeparam name="TLocalizedModelLocal">Localizable model</typeparam>
        /// <param name="languageService">Language service</param>
        /// <param name="locales">Locales</param>
        /// <param name="configure">Configure action</param>
        protected virtual void AddLocales<TLocalizedModelLocal>(ILanguageService languageService, IList<TLocalizedModelLocal> locales, Action<TLocalizedModelLocal, int> configure) where TLocalizedModelLocal : ILocalizedModelLocal
        {
            foreach (var language in languageService.GetAllLanguages(true))
            {
                var locale = Activator.CreateInstance<TLocalizedModelLocal>();
                locale.LanguageId = language.Id;
                if (configure != null)
                {
                    configure.Invoke(locale, locale.LanguageId);
                }
                locales.Add(locale);
            }
        }

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        [SuppressMessage("ReSharper", "Mvc.AreaNotResolved")]
        protected ActionResult AccessDeniedView()
        {
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl, area = "Administrator" });
        }

        /// <summary>
        /// Renders default access denied view as a partial
        /// </summary>
        protected ActionResult AccessDeniedPartialView()
        {
            return PartialView("~/Administrator/Views/Security/AccessDenied.cshtml");
        }
    }
}
