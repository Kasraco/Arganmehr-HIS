using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace ViewModel.AdminArea.Arganmehr
{
    public class InstitutionModel
    {

        public virtual int Id { get; set; }

          [Required(ErrorMessage = "لطفا کد موسسه خود را وارد کنید")]
        [Remote("ExistInstitutionCode", "Institution", "", ErrorMessage = "این کد قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        public string InstitutionCode { get; set; }


          [Required(ErrorMessage = "لطفا نام موسسه خود را وارد کنید")]
        [Remote("ExistInstitutionTitle", "Institution", "", ErrorMessage = "این موسسه قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        public string InstitutionTitle { get; set; }

        public string CountryDivitionCode { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل را به شکل صحیح وارد کنید")]
        [DisplayName("ایمیل")]
        [StringLength(256, ErrorMessage = "حداکثر طول ایمیل 256 حرف است")]
        [Remote("ExistEmailAddress", "Institution", "", ErrorMessage = "این ایمیل قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        public string EmailAddress { get; set; }


        public string WebSiteAddress { get; set; }
        public string WebServiceAddress { get; set; }

        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        [DisplayName("نام کاربری")]
        [StringLength(256, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 256 حرف باشد", MinimumLength = 5)]
        [Remote("IsUserNameExist", "Institution", "", ErrorMessage = "این نام کاربری قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "لطفا فقط از حروف انگلیسی و اعدد استفاده کنید")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید")]
        [StringLength(50, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 50 حرف باشد", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("کلمه عبور")]
        [Remote("CheckPassword", "Institution", "", ErrorMessage = "این کلمه عبور به راحتی قابل تشخیص است", HttpMethod = "POST")]
        public string Password { get; set; }
        public string CreateDate { get; set; }

    }

}
