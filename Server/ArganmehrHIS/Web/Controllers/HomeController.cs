using System.Web.Mvc;
using System.ComponentModel;
using ServiceLayer.Security;
using Common.Controller;
using MvcSiteMapProvider;
using ServiceLayer.EFServiecs.AM.CA;
using Common.Security;

namespace Web.Controllers
{

    [Description("خانه")]
   
    public partial class HomeController : BaseController
    {

        #region Fields
        private const int ADay = 86400;
        private readonly CentralAdminService _cas;
        #endregion

        #region Ctor

        public HomeController(CentralAdminService CAS)
        {
            _cas = CAS;
           
        }
       
        #endregion
         [ActivityLog(Name="ViewHome",Description="Home")]
        public virtual ActionResult Index()
        {
            if (!_cas.ExistCentralAdmin())
               return  RedirectToAction(MVC.Install.ActionNames.Index, MVC.Install.Name);
            return View();
        }

        //for waterMark site images 
        //[OutputCache(VaryByParam = "fileName", Duration = ADay)]
        //public ActionResult Image(string fileName)
        //{
        //    fileName = Path.GetFileName(fileName); 
        //    var rootPath = Server.MapPath("~/Images");
        //    var path = Path.Combine(rootPath, fileName);
        //    if (!System.IO.File.Exists(path))
        //    {
        //        const string notFoundImage = "notFound.png";
        //        path = Path.Combine(rootPath, notFoundImage);
        //        return File(path, MediaTypeNames.Image.Gif, notFoundImage);
        //    }

        //    if (!this.IsEmbeddedIntoAnotherDomain()) return File(path, MediaTypeNames.Image.Gif, fileName);

        //    var text = Url.Action(actionName: "Index", controllerName: "Home", routeValues: null, protocol: "http");
        //    var content = ImageManage.AddWaterMark(path, text);
        //    return File(content, MediaTypeNames.Image.Gif, fileName);
        //}
        public virtual ActionResult About()
        {
            return RedirectToAction("Index");
        }
        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
