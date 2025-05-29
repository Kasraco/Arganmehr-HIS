using System.Web.Mvc;
using Common.Controller;

namespace Web.Controllers
{
    [RoutePrefix("Error")]
    [Route("{action}")]
    public partial class ErrorController : BaseController
    {
        public virtual ActionResult Error403()
        {
            return View();
        }

        public virtual ActionResult Error404()
        {
            return View();
        }
        public virtual ActionResult Error500()
        {
            return View();
        }


    }
}
