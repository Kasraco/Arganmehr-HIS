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
    public interface ICommonServices
    {
      

        ICacheManagerKRB Cache
        {
            get;
        }



        IWebHelper WebHelper
        {
            get;
        }

        IWorkContext WorkContext
        {
            get;
        }

     

        ILocalizationService Localization
        {
            get;
        }

      

        ISettingServices Settings
        {
            get;
        }

        IPermissionService Permissions
        {
            get;
        }

       

       
    }
}
