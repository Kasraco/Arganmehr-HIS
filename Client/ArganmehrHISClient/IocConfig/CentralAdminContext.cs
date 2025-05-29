using DomainClasses.Entities.AM;
using DomainClasses.Entities.AM.Extentions;

using ServiceLayer.Context;
using ServiceLayer.Contracts.AM.CA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Helpers.Extentions;
using IocConfig.Extensions;
using ServiceLayer;

namespace IocConfig
{
    public class CentralAdminContext : ICAContext
    {

        internal const string OverriddenCentralAdminIdKey = "OverriddenCentralAdminId";

        private readonly ICentralAdminService _CentralAdminService;
        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        private CentralAdmin _currentCentralAdmin;

        public CentralAdminContext(ICentralAdminService CentralAdminService, IWebHelper webHelper, HttpContextBase httpContext)
        {
            this._CentralAdminService = CentralAdminService;
            this._httpContext = httpContext;
            this._webHelper = webHelper;
        }

        public void SetRequestCA(int? centralManagementId)
        {
            try
            {
                var dataTokens = _httpContext.Request.RequestContext.RouteData.DataTokens;
                if (centralManagementId.GetValueOrDefault() > 0)
                {
                    dataTokens[OverriddenCentralAdminIdKey] = centralManagementId.Value;
                }
                else if (dataTokens.ContainsKey(OverriddenCentralAdminIdKey))
                {
                    dataTokens.Remove(OverriddenCentralAdminIdKey);
                }

                _currentCentralAdmin = null;
            }
            catch { }
        }

        public void SetPreviewCA(int? centralManagementId)
        {
            try
            {
                _httpContext.SetPreviewModeValue(OverriddenCentralAdminIdKey, centralManagementId.HasValue ? centralManagementId.Value.ToString() : null);
                _currentCentralAdmin = null;
            }
            catch { }
        }

        public int? GetRequestCA()
        {
            try
            {
                var value = _httpContext.Request.RequestContext.RouteData.DataTokens[OverriddenCentralAdminIdKey];
                if (value != null)
                {
                    return (int)value;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public int? GetPreviewCA()
        {
            try
            {
                var cookie = _httpContext.GetPreviewModeCookie(false);
                if (cookie != null)
                {
                    var value = cookie.Values[OverriddenCentralAdminIdKey];
                    if (value.HasValue())
                    {
                        return value.ToInt();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public CentralAdmin CurrentCA
        {
            get
            {
                if (_currentCentralAdmin == null)
                {
                    int? CentralAdminOverride = GetRequestCA() ?? GetPreviewCA();
                    if (CentralAdminOverride.HasValue)
                    {
                        // the Institiution to be used can be overwritten on request basis (e.g. for theme preview, editing etc.)
                        _currentCentralAdmin = _CentralAdminService.GetCentralAdminById(CentralAdminOverride.Value);
                    }
                    else
                    {
                        // ty to determine the current Institiution by HTTP_HOST
                        var host = _webHelper.ServerVariables("HTTP_HOST");
                        var allCentralAdmins = _CentralAdminService.GetAllCentralAdmins();
                        var centralmanagement = allCentralAdmins.FirstOrDefault(s => s.ContainsHostValue(host));

                        if (centralmanagement == null)
                        {
                            //load the first found Institiution
                            centralmanagement = allCentralAdmins.FirstOrDefault();
                        }

                        if (centralmanagement == null)
                        {
                            //throw new Exception("No CentralAdmin could be loaded");
                        }

                        _currentCentralAdmin = centralmanagement;
                    }
                }

                return _currentCentralAdmin;
            }
        }

        public int CurrentCAIdIfMultiCAMode
        {
            get
            {
                return _CentralAdminService.IsSingleCentralAdminMode() ? 0 : CurrentCA.Id;
            }
        }

      
    }
}
