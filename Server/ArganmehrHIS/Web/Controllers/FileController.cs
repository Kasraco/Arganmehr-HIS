using System.ComponentModel;
using System.Web.Mvc;
using Common.Controller;
using Common.Filters;
using ServiceLayer.Security;

namespace Web.Controllers
{
    [Description("مدیریت فایل")]
    [CustomAuthorizeAttribute]
    public partial class FileController : BaseController
    {
        [DisplayName("دسترسی به تصاویر")] 
        public virtual ActionResult Image(string name)
        {
            return View();
        }
        [DisplayName("دسترسی به فایل های ارسالی")] 
        public virtual ActionResult UserFile(string name)
        {
            return View();
        }
        [DisplayName("دسترسی به تصاویر کاربران")] 
        public virtual ActionResult Avatar(string name)
        {
            return View();
        }
    }
}
