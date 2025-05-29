using System.Web.Mvc;
using Common.Filters;
using NWebsec.Mvc.HttpHeaders;
using NWebsec.Mvc.HttpHeaders.Csp;

namespace Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            //logg action errors
            //filters.Add(new ElmahHandledErrorLoggerFilter());
            //logg xss attacks 
            // filters.Add(new ElmahRequestValidationErrorFilter());
            filters.Add(new ForceWww("http://localhost:25890/"));

            //with this filter you cann't user inline script or css on page
            //filters.Add(new SetNoCacheHttpHeadersAttribute());
            //filters.Add(new XRobotsTagAttribute() { NoIndex = true, NoFollow = true });
            //filters.Add(new XContentTypeOptionsAttribute());
            //filters.Add(new XDownloadOptionsAttribute());
            //filters.Add(new XFrameOptionsAttribute());
            //filters.Add(new XXssProtectionAttribute());
            ////CSP
            //filters.Add(new CspAttribute());
            //filters.Add(new CspDefaultSrcAttribute { Self = true });
            //filters.Add(new CspScriptSrcAttribute { Self = true });
            ////CSPReportOnly
            //filters.Add(new CspReportOnlyAttribute());
            //filters.Add(new CspScriptSrcReportOnlyAttribute { None = true });
            //filters.Add(new RemoveServerHeaderFilterAttribute());
        }
    }
}
