
using Common.Caching;
using Common.UI;
using DomainClasses.Configurations;
using DomainClasses.Entities.Logg;
using DomainClasses.Entities.SEO;
using DomainClasses.Entities.Theme;
using DomainClasses.IO.WebSite;
using DomainClasses.IO.WebSite.VirtualPath;
using DomainClasses.Theme;
using IocConfig.Common;
using IocConfig.Helper;
using IocConfig.SEO;
using IocConfig.Theming;
using IocConfig.UI;
using IocConfig.DependencyInjection;
using ServiceLayer;
using ServiceLayer.Congfiguration;
using ServiceLayer.Context;
using ServiceLayer.Contracts.AM.CA;
using ServiceLayer.Contracts.AM.Directory;
using ServiceLayer.Contracts.AM;
using ServiceLayer.Contracts.Common;
using ServiceLayer.EFServiecs.AM.CA;
using ServiceLayer.EFServiecs.AM.Directory;
using ServiceLayer.EFServiecs.Common;
using ServiceLayer.Helper;
using ServiceLayer.Localization;
using ServiceLayer.Logger;
using ServiceLayer.ThemService;
using StructureMap.Configuration.DSL;
using StructureMap.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Infrastructure;

using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Threading;

using System.Web.Mvc;
using AutoMapper;
using Microsoft.Owin.Security;
using StructureMap.Pipeline;
using Common.Helpers.Fakes;
using System.Reflection;
using StructureMap.Web.Pipeline;
using ServiceLayer.EFServiecs.AM;

namespace IocConfig
{
    public class ArganmehrRegistry : Registry
    {
        public ArganmehrRegistry()
        {
           var uniquePerRequest = new UniquePerRequestLifecycle();
            For<ICache>().Use<CMKRB>();
            For<ICache>().Use<StaticCache>().Singleton();
            For<ICache>().Use<AspNetCache>().Singleton();
            For<IText>().Use<Text>();
            //For<HttpContextBase>().Use<HttpContextBaseFactory>();           
 
           
            For<ICacheManagerKRB>().Use<CacheManagerKRB<StaticCache>>().Named("static").Singleton();
            For<ICacheManagerKRB>().Use<CacheManagerKRB<AspNetCache>>().Named("aspnet").Singleton();
            For<ISettingServices>().Use<SettingService>();




            For<IWorkContext>().LifecycleIs(new HttpSessionLifecycle()).LifecycleIs(uniquePerRequest).Use<WebWorkContext>();
            For<ICAContext>().LifecycleIs(uniquePerRequest).Use<CentralAdminContext>();
            For<ICentralAdminService>().Use<CentralAdminService>();
            For<IWebHelper>().Use<WebHelper>();

            For<IThemeContext>().LifecycleIs(uniquePerRequest).Use<ThemeContext>();
            For<ISettings>().Use<SeoSettings>();
           // For<ISettings>().Use<ThemeSettings>();
            For<IWebSiteFolder>().LifecycleIs(uniquePerRequest).Use<WebSiteFolder>();
            For<IVirtualPathProvider>().LifecycleIs(uniquePerRequest).Use<DefaultVirtualPathProvider>();
            For<IThemeRegistry>().Use(() => new DefaultThemeRegistry(null, null, true)).Singleton();
            For<ITopologicSortable<string>>().Use<ThemeFolderData>();
            For<IThemeVariablesService>().LifecycleIs(uniquePerRequest).Use<ThemeVariablesService>();
            For<ILogger>().Use<DefaultLogger>();
            For<ILogger>().Use<NullLogger>();
            For<IUserAgent>().Use<UAParserUserAgent>();
            For<IMobileDeviceHelper>().LifecycleIs(uniquePerRequest).Use<MobileDeviceHelper>();
            For<IThemeFileResolver>().Use<ThemeFileResolver>().Singleton();
        
            For<ILanguageService>().Use<LanguageService>();
            For<ILocalizationService>().Use<LocalizationService>();
            For<ILocalizedEntityService>().Use<LocalizedEntityService>();
            For<IHideObjectMembers>().Use<ComponentFactory>();

           // For<IUiComponent>().Use<Component>();
            

            For<ComponentRenderer<TabStrip>>().Use<TabStripRenderer>();
            For<ComponentRenderer<Pager>>().Use<PagerRenderer>();
            For<ComponentRenderer<Window>>().Use<WindowRenderer>();
            For<IDateTimeHelper>().Use<DateTimeHelper>();
            For<ISettings>().Use<DateTimeSettings>();
            For<IGenericAttributeService>().LifecycleIs(uniquePerRequest).Use<GenericAttributeService>();
            For<ICountryService>().Use<CountryService>();
            For<IStateProvinceService>().Use<StateProvinceService>();


            For<ICommonServices>().LifecycleIs(uniquePerRequest).Use<CommonServices>();

            For<IInstitutionService>().Use<InstitutionService>();
           
        }

   

       

        static HttpContextBase HttpContextBaseFactory(ILifecycle ctx)
        {
            if (IsRequestValid())
            {
                return new HttpContextWrapper(HttpContext.Current);
            }

            // TODO: determine store url

            // register FakeHttpContext when HttpContext is not available
            return new FakeHttpContext("~/");
        }

        static bool IsRequestValid()
        {
            if (HttpContext.Current == null)
                return false;

            try
            {
                // The "Request" property throws at application startup on IIS integrated pipeline mode
                var req = HttpContext.Current.Request;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
