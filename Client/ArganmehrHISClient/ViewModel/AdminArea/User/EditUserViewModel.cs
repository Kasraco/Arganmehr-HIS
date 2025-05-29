using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using DomainClasses;
using DomainClasses.Entities;
using DomainClasses.Entities.Common;
using DomainClasses.Entities.Users;

namespace ViewModel.AdminArea.User
{
    public class EditUserViewModel
    {
        public long Id { get; set; }

        [DisplayName("کاربر سیستمی")]
        public bool IsSystemAccount { get; set; }
        [DisplayName("تاریخ تولد")]
        public string BirthDay { get; set; }
        [DisplayName(" آخرین فعالیت")]
        public string LastActivityDate { get; set; }
        public ICollection<UserRole> Roles { get; set; }
        [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل را به شکل صحیح وارد کنید")]
        [DisplayName("ایمیل")]
        [StringLength(256, ErrorMessage = "حداکثر طول ایمیل 256 حرف است")]
        [Remote("IsEmailExist", "User", "Administrator", ErrorMessage = "این ایمیل قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 50 حرف باشد", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("کلمه عبور")]
        [Remote("CheckPassword", "User", "Administrator", ErrorMessage = "این کلمه عبور به راحتی قابل تشخیص است", HttpMethod = "POST", AdditionalFields = "Id")]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        [DisplayName("نام کاربری")]
        [StringLength(256, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 256 حرف باشد", MinimumLength = 5)]
        [Remote("IsUserNameExist", "User", "Administrator", ErrorMessage = "این نام کاربری قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "لطفا فقط از حروف انگلیسی و اعدد استفاده کنید")]
        public string UserName { get; set; }
        [DisplayName("قفل شود")]

        public bool IsBanned { get; set; }
        [Required(ErrorMessage = "لطفا نام نمایشی خود را وارد کنید")]
        [DisplayName("نام نمایشی")]
        [StringLength(256, ErrorMessage = "نام نمایشی نباید کمتر از 5 حرف و بیتشر از 256 حرف باشد", MinimumLength = 5)]
        [Remote("IsNameForShowExist", "User", "Administrator", ErrorMessage = "این نام نمایشی قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF,0-9\s]*$", ErrorMessage = "لطفا فقط ازاعداد و حروف  فارسی استفاده کنید")]
        public string NameForShow { get; set; }

        [DisplayName("نظر مدیر")]
        [MaxLength(1024, ErrorMessage = "تعداد حروف نظر مدیر غیر مجاز است")]
        public string AdministratorComment { get; set; }
        [DisplayName("تصویر پروفایل")]
        public string AvatarFileName { get; set; }
        [DisplayName(" آی دی گوگل پلاس")]
        [MaxLength(20, ErrorMessage = "تعداد کاراکتر های آی دی گوگل پلاس غیر مجاز است")]
        [Remote("GooglePlusIdExist", "User", "Administrator", ErrorMessage = "این آدرس قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        public string GooglePlusId { get; set; }
        [DisplayName("آی دی فیسبوک")]
        [MaxLength(20, ErrorMessage = "تعداد کاراکتر های آی دی فیسبوک غیر مجاز است")]
        [Remote("FaceBookIdExist", "User", "Administrator", ErrorMessage = "این آدرس قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        public string FaceBookId { get; set; }
        
        [DisplayName("دسترسی برای آپلود فایل")]
        public bool CanUploadFile { get; set; }
        [DisplayName("دسترسی برای تغییر آواتار")]
        public bool CanChangeProfilePicture { get; set; }
        [DisplayName("دسترسی برای تغییر نام-نام خانوادگی")]
        public bool CanModifyFirsAndLastName { get; set; }
        public long[] RoleIds { get; set; }
        [DisplayName("به صورت منطقی حذف شود")]
        public bool IsDeleted { get; set; }
        [DisplayName("تاریخ عضویت")]
        public string RegisterDate { get; set; }
        public HttpPostedFileBase AvatarImage { get; set; }
    }
}
