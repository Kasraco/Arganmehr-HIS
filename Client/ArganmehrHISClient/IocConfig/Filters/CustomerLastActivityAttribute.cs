using AutoMapper;
using ServiceLayer.Context;
using ServiceLayer.EFServiecs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel.AdminArea.User;

namespace IocConfig.Filters
{
    public class CustomerLastActivityAttribute : ActionFilterAttribute
    {
        public IWorkContext WorkContext { get; set; }
        public IMappingEngine mappingEngine { get; set; }
        public ApplicationUserManager CustomerService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            WorkContext = ProjectObjectFactory.Container.GetInstance<IWorkContext>();
            CustomerService = ProjectObjectFactory.Container.GetInstance<ApplicationUserManager>();
            mappingEngine = ProjectObjectFactory.Container.GetInstance<IMappingEngine>();
             
            //if (!DataSettings.DatabaseIsInstalled())
            //    return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            // only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;
            
            var customer = WorkContext.CurrentUser;

            // update last activity date
            if (!customer.IsSystemAccount && customer.LastActivityDate.AddMinutes(1.0) < DateTime.UtcNow)
            {
                customer.LastActivityDate = DateTime.UtcNow;
                var euvm = mappingEngine.Map<EditUserViewModel>(customer);
               CustomerService.EditUser(euvm);
            }
        }
    }
}
