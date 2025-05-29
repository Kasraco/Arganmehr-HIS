using System.Collections.Generic;
using System.Security.Principal;
using StackExchange.Profiling;
using StructureMap.Web.Pipeline;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using IocConfig;
using ServiceLayer.Contracts;
using System.Web.WebPages;
using FluentValidation.Mvc;
using IocConfig.Validators;
using IocConfig.Theming;
using System.Web.Hosting;
using ServiceLayer.Context;
using Common.Helpers.Extentions;
using Common.Data;

namespace Web
{
    public class MvcApplication : HttpApplication
    {

        #region Application_Start
        protected void Application_Start()
        {
            try
            {
                var mobileDisplayMode = DisplayModeProvider.Instance.Modes.FirstOrDefault(x => x.DisplayModeId == DisplayModeProvider.MobileDisplayModeId);
                if (mobileDisplayMode != null)
                    DisplayModeProvider.Instance.Modes.Remove(mobileDisplayMode);

                AreaRegistration.RegisterAllAreas();

                DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
                ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new SmartValidatorFactory()));

                RoutingConfig.RegisterRoutes(RouteTable.Routes);

                ClientDataTypeModelValidatorProvider.ResourceClassKey = "MvcLocalization";
                DefaultModelBinder.ResourceClassKey = "MvcLocalization";
               // ErrorMessageProvider.SetResourceClassKey("MvcLocalization");

              

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                HostingEnvironment.RegisterVirtualPathProvider(new ThemingVirtualPathProvider(HostingEnvironment.VirtualPathProvider));
                BundleTable.VirtualPathProvider = HostingEnvironment.VirtualPathProvider;

                ApplicationStart.Config();
               
            }
            catch
            {
                HttpRuntime.UnloadAppDomain(); // سبب ری استارت برنامه و آغاز مجدد آن با درخواست بعدی می‌شود
                throw;
            }

        }
        #endregion

        #region Application_EndRequest
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            try
            {
                foreach (var task in ProjectObjectFactory.Container.GetAllInstances<IRunAfterEachRequest>())
                {
                    task.Execute();
                }
            }
            catch (Exception)
            {
                HttpContextLifecycle.DisposeAndClearAll();
                MiniProfiler.Stop();
            }
          
        }
        #endregion

        #region Application_BeginRequest
        private void Application_BeginRequest(object sender, EventArgs e)
        {
            foreach (var task in ProjectObjectFactory.Container.GetAllInstances<IRunOnEachRequest>())
            {
                task.Execute();
            }

            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }
        #endregion

        #region ShouldIgnoreRequest
        private bool ShouldIgnoreRequest()
        {
            string[] reservedPath =
              {
                  "/__browserLink",
                  "/Scripts",
                  "/Content"
              };

            var rawUrl = Context.Request.RawUrl;
            if (reservedPath.Any(path => rawUrl.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return BundleTable.Bundles.Select(bundle => bundle.Path.TrimStart('~'))
                      .Any(bundlePath => rawUrl.StartsWith(bundlePath, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region Private
        private void SetPermissions(IEnumerable<string> permissions)
        {
            Context.User =
                new GenericPrincipal(Context.User.Identity, permissions.ToArray());
        }
        #endregion

        #region Application_Error
        protected void Application_Error()
        {
            foreach (var task in ProjectObjectFactory.Container.GetAllInstances<IRunOnError>())
            {
                task.Execute();
            }
        }
        #endregion

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            string result = string.Empty;

           // if (DataSettings.DatabaseIsInstalled())
            //{
                custom = custom.ToLowerInvariant();

                switch (custom)
                {
                    case "theme":
                        result = ProjectObjectFactory.Container.GetInstance<IThemeContext>().CurrentTheme.ThemeName;
                        break;
                    case "store":
                        result = ProjectObjectFactory.Container.GetInstance<ICAContext>().CurrentCA.Id.ToString();
                        break;
                    case "theme_store":
                        result = "{0}-{1}".FormatInvariant(
                            ProjectObjectFactory.Container.GetInstance<IThemeContext>().CurrentTheme.ThemeName,
                            ProjectObjectFactory.Container.GetInstance<ICAContext>().CurrentCA.Id.ToString());
                        break;
                }
          //  }

            if (result.HasValue())
            {
                return result;
            }

            return base.GetVaryByCustomString(context, custom);
        }
      
    }
}
