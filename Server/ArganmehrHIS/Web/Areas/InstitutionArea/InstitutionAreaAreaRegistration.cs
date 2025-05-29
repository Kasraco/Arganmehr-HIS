using System.Web.Mvc;

namespace Web.Areas.InstitutionArea
{
    public class InstitutionAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InstitutionArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
           

            context.MapRoute(
               "InstitutionArea_default",
               "InstitutionArea/{controller}/{action}/{id}",
               new { controller = MVC.Administrator.Home.Name, action = MVC.InstitutionArea.Home.ActionNames.Index, id = UrlParameter.Optional },
               namespaces: new[] { string.Format("{0}.Controllers", typeof(InstitutionAreaAreaRegistration).Namespace) }
               );
        }
    }
}