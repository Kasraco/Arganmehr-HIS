
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ViewModel.AdminArea.Role
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "لطفا نام گروه را وارد کنید")]
        [DisplayName("نام گروه")]
        [StringLength(50, ErrorMessage = "تعداد کاراکتر های نام گروه نباید کمتر از 5 و بیشتر از 50 باشد", MinimumLength = 5)]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF,0-9\s]*$", ErrorMessage = "لطفا فقط ازاعداد و حروف  فارسی استفاده کنید")]
        [Remote("RoleNameExist", "Role", "Administrator", ErrorMessage = "این گروه قبلا در سیستم ثبت شده است", HttpMethod = "POST", AdditionalFields = "Id")]
        public string Name { get; set; }
        [Required(ErrorMessage = "لطفا توضیحاتی برای گروه وارد کنید")]
        [StringLength(256, ErrorMessage = "تعداد کاراکتر های توضیحات گروه نباید کمتر از 5 و بیشتر از 256 باشد", MinimumLength = 5)]
        [RegularExpression(@"^[\u0600-\u06FF,\u0590-\u05FF,0-9\s]*$", ErrorMessage = "لطفا فقط ازاعداد و حروف  فارسی استفاده کنید")]
        [DisplayName("توضیحات")]
        public string Description { get; set; }
        [DisplayName("گروه پیشفرض ثبت نام است؟")]
        public bool IsDefaultForRegister { get; set; }
        [DisplayName("فعال باشد؟")]
        public bool IsActive { get; set; }
        [DisplayName("گروه سیستمی است؟")]
        public bool IsSystemRole { get; set; }
        public string[] PermissionNames { get; set; }
    }
}
