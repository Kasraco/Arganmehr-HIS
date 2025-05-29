using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AdminArea.User
{
    public class UserListViewModel
    {
        public IList<UserViewModel> Users { get; set; }
        public UserSearchRequest SearchRequest { get; set; }
    }
}
