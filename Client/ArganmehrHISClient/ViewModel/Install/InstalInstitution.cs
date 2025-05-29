using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ViewModel.Install
{
    public class InstalInstitution
    {
        [Required(ErrorMessage="وارد کردن آدرس مرکز مدیریت الزامیست")]
        [DisplayName("آدرس وب سرویس")]
        [Remote("ExistWebService", "Install", HttpMethod = "POST", ErrorMessage = "این وب سرویس وجود ندارد", AdditionalFields = "InstitutionCode")]
        public string CentralAdminWebServiceURL { get; set; }

        [DisplayName("نام کاربری")]
        [Required(ErrorMessage = "نام کاربری حتما باید وارد شود")]
        public string UserName { get; set; }

        [DisplayName("کلمه عبور")]
        [Required(ErrorMessage = "کلمه عبور الزامی است")]
        public string Password { get; set; }

        [DisplayName("کد موسسه")]
        [Required(ErrorMessage = "کد موسسه الزامی است")]
        public string InstitutionCode { get; set; }

        [DisplayName("آدرس وب سرویس موسسه")]
        [Required(ErrorMessage = "آدرس وب سرویس موسسه باید وارد شود")]
        [Remote("ExistInstitutionWebServiceURL", "Install", HttpMethod = "POST", ErrorMessage = "این وب سرویس وجود ندارد", AdditionalFields="CentralAdminWebServiceURL")]

        public string InstitutionWebServiceURL { get; set; }



    }
}
