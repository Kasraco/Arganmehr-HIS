using Common.Caching;
using DomainClasses.Configurations;
using ServiceLayer.Context;
using ServiceLayer.Contracts;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class CommonServices : ICommonServices
    {
        private readonly Lazy<ICacheManagerKRB> _cache;
        private readonly Lazy<IWorkContext> _workContext;
        private readonly Lazy<ILocalizationService> _localization;
        private readonly Lazy<ISettingServices> _settings;
        private readonly Lazy<IPermissionService> _permissions;
        private readonly Lazy<IWebHelper> _webHelper;

        public CommonServices(
         
            Lazy<ICacheManagerKRB> cache,         
            Lazy<IWorkContext> workContext,        
            Lazy<ILocalizationService> localization,         
            Lazy<IPermissionService> permissions,
            Lazy<ISettingServices> settings,
            Lazy<IWebHelper> webHelper)
        
        {
          
            this._cache = cache;         
            this._workContext = workContext;        
            this._localization = localization;          
            this._permissions = permissions;
            this._settings = settings;
            this._webHelper = webHelper;
         
        }

      

        public ICacheManagerKRB Cache
        {
            get
            {
                return _cache.Value;
            }
        }

        public IWebHelper WebHelper
        {
            get
            {
                return _webHelper.Value;
            }
        }
		
      
        public IWorkContext WorkContext
        {
            get
            {
                return _workContext.Value;
            }
        }

 
        public ILocalizationService Localization
        {
            get
            {
                return _localization.Value;
            }
        }

   
        public IPermissionService Permissions
        {
            get
            {
                return _permissions.Value;
            }
        }

        public ISettingServices Settings
        {
            get
            {
                return _settings.Value;
            }
        }


       
    }
}
