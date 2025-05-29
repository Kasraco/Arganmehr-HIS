using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;
using IocConfig.Theming;
using Common.Data;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Web.RazorGeneratorMvcStart), "Start")]

namespace Web
{
    public static class RazorGeneratorMvcStart
    {
        public static void Start()
        {

           

           
            //var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly)
            //{
            //    UsePhysicalViewsIfNewer = HttpContext.Current.Request.IsLocal
            //};

           // ViewEngines.Engines.Clear();
           ///// ViewEngines.Engines.Add(engine);
          //  ViewEngines.Engines.Add(new RazorViewEngine());
        // //   VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);

             ViewEngines.Engines.Clear();
                // ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
                // ViewEngines.Engines.Add(new RazorViewEngine());

                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
          
        }
    }
}
