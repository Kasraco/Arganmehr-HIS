using DomainClasses.Security;
using ServiceLayer.Context;
using Common.Helpers.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Helpers.security;
namespace IocConfig.Filters
{

      [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsByConfigAttribute : FilterAttribute, IAuthorizationFilter
    {
        public RequireHttpsByConfigAttribute(SslRequirement sslRequirement)
        {
            this.SslRequirement = sslRequirement;
        }

        public Lazy<SecuritySettings> SecuritySettings { get; set; }
        public Lazy<ICAContext> CAContext { get; set; }
   

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

           

            var currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection();

            var securitySettings = SecuritySettings.Value;
            if (securitySettings.ForceSslForAllPages)
            {
                // all pages are forced to be SSL no matter of the specified value
                this.SslRequirement = SslRequirement.Yes;
            }

            switch (this.SslRequirement)
            {
                case SslRequirement.Yes:
                    {
                        if (!currentConnectionSecured)
                        {
                            var storeContext = CAContext.Value;
                            var currentStore = storeContext.CurrentCA;

                            if (currentStore != null )
                            {
                                // redirect to HTTPS version of page
                                 string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                               // var webHelper = WebHelper.Value;
                               // string url = webHelper.GetThisPageUrl(true, true);
                                filterContext.Result = new RedirectResult(url, true);
                            }
                        }
                    }
                    break;
                case SslRequirement.No:
                    {
                        if (currentConnectionSecured)
                        {
                           // var webHelper = WebHelper.Value;

                            // redirect to HTTP version of page
                             string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                           // string url = webHelper.GetThisPageUrl(true, false);
                            filterContext.Result = new RedirectResult(url, true);
                        }
                    }
                    break;
                case SslRequirement.Retain:
                    {
                        //do nothing
                    }
                    break;
                default:
                    throw new Exception("Unsupported SslRequirement parameter");
            }
        }

        public SslRequirement SslRequirement { get; set; }
    }
}
