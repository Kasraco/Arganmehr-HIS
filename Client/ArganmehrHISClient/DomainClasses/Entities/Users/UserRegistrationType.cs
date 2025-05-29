using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Users
{
    public enum UserRegistrationType : int
    {
        /// <summary>
        /// Standard account creation
        /// </summary>
        [Display(Name = "استاندارد")]
        Standard = 1,
        /// <summary>
        /// Email validation is required after registration
        /// </summary>
        [Display(Name = "فعال سازی با ایمیل")]
        EmailValidation = 2,
        /// <summary>
        /// A customer should be approved by administrator
        /// </summary>
        [Display(Name = "با تآیید مدیر")]
        AdminApproval = 3,
        /// <summary>
        /// Registration is disabled
        /// </summary>
        [Display(Name = "غیر فعال")]
        Disabled = 4,
    }
}
