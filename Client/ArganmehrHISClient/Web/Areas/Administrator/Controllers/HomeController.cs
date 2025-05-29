using System.ComponentModel;
using System.Web.Mvc;
using Common.Filters;
using ServiceLayer.Security;

using DataLayer.Context;
using Common.Controller;
using MvcSiteMapProvider;
using IocConfig.Controllers;

namespace Web.Areas.Administrator.Controllers
{
    [Description("پنل مدیریت")]
    [CustomAuthorizeAttribute]
    public partial class HomeController : AdminControllerBase
    {
       

    
       
        [DisplayName("مشاهده پنل مدیریت")]
        [MvcSiteMapNode(ParentKey = "Home_Index", Title = "پنل مدیریت" , Key="Administrator_Home")]
        public virtual ActionResult Index()
        {
            

            return View();
        }
    }
}