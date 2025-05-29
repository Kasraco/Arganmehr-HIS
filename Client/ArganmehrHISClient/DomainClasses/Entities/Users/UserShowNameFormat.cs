using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Users
{
    /// <summary>
    /// Represents the customer name fortatting enumeration
    /// </summary>
    public enum UserShowNameFormat : int
    {
        /// <summary>
        /// Show emails
        /// </summary>
        [Display(Name = "نمایش ایمیل")]
        ShowEmail = 1,
        /// <summary>
        /// Show usernames
        /// </summary>
        [Display(Name = "نایش نام کاربری")]
        ShowUsername = 2,
        /// <summary>
        /// Show first name
        /// </summary>
        [Display(Name = "نمایش نام")]
        ShowName = 3
    }

}
