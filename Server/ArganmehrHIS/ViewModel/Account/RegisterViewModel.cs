using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا ایمیل خود را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل را به شکل صحیح وارد کنید")]
        [DisplayName("ایمیل")]
        [StringLength(256, ErrorMessage = "حداکثر طول ایمیل 256 حرف است")]
        [Remote("IsEmailExist", "Account", "", ErrorMessage = "این ایمیل قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید")]
        [StringLength(50, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 50 حرف باشد", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("کلمه عبور")]
        [Remote("CheckPassword", "Account", "", ErrorMessage = "این کلمه عبور به راحتی قابل تشخیص است", HttpMethod = "POST")]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید")]
        [DataType(DataType.Password)]
        [DisplayName("تکرار کلمه عبور")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "کلمات عبور وارد شده مطابقت ندارند")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        [DisplayName("نام کاربری")]
        [StringLength(256, ErrorMessage = "کلمه عبور نباید کمتر از 5 حرف و بیتشر از 256 حرف باشد", MinimumLength = 5)]
        [Remote("IsUserNameExist", "Account", "", ErrorMessage = "این نام کاربری قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "لطفا فقط از حروف انگلیسی و اعدد استفاده کنید")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لطفا نام نمایشی خود را وارد کنید")]
        [DisplayName("نام نمایشی")]
        [StringLength(256, ErrorMessage = "نام نمایشی نباید کمتر از 5 حرف و بیتشر از 256 حرف باشد", MinimumLength = 5)]
        [Remote("IsNameForShowExist", "Account", "", ErrorMessage = "این نام نمایشی قبلا در سیستم ثبت شده است", HttpMethod = "POST")]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF,0-9\s]*$", ErrorMessage = "لطفا فقط ازاعداد و حروف  فارسی استفاده کنید")]
        public string NameForShow { get; set; }
    }
}