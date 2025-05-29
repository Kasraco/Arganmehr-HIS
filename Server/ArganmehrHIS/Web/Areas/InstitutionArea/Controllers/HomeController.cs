using Common.Controller;
using MvcSiteMapProvider;
using ServiceLayer.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.InstitutionArea.Controllers
{

    [Description("مدیریت موسسه")]
    [CustomAuthorizeAttribute]
    public partial class HomeController : BaseController
    {




        [DisplayName("مشاهده پنل مدیریت موسسه")]
        [MvcSiteMapNode(ParentKey = "Home_Index", Title = "پنل مدیریت موسسه", Key = "InstitutionArea_Home")]
        public virtual ActionResult Index()
        {


            return View();
        }
    }
}
