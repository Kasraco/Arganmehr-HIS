using ServiceLayer.EFServiecs.AM.CA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModel.Install;

namespace Web.Controllers
{
   
    public partial class InstallController : Controller
    {
        private readonly CentralAdminService _cas;
        public InstallController(CentralAdminService CAS)
        {
            _cas = CAS;

        }


        public virtual ActionResult Index()
        {
            return View(MVC.Install.Views.Index);
        }

        [HttpGet]
        public virtual ActionResult CreateCentralAdmin()
        {
            return PartialView(MVC.Install.Views._CreateCA);
        }

        [HttpPost]
        public virtual ActionResult CreateCentralAdmin(CentralAdminModel model)
        {
            return RedirectToAction(MVC.Home.ActionNames.Index,MVC.Home.Name);
        }
    }
}
