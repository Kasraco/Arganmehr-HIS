using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AdminArea.RoleAccess
{


    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }

    public class EditUserRoleViewModel
    {
        [Required]
        public string UserId { get; set; }

        public string UserName { get; set; }

        [Required]
        public IEnumerable<string> SelectedRoles { get; set; }

        public IEnumerable<Role.RoleViewModel> Roles { get; set; }
    }





}
