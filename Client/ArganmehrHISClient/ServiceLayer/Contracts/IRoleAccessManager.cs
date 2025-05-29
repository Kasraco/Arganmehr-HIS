using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Role;
using ViewModel.AdminArea.RoleAccess;

namespace ServiceLayer.Contracts
{
    public interface IRoleAccessManager
    {

        Task AddRoleAccess(AddRoleViewModel model);
        Task EditRoleAccess(EditUserRoleViewModel model);
        Task<List<UserRoleViewModel>> GetRoleAccess(int roleId);

        
    }
}
