using DomainClasses.Entities.Users;
using Microsoft.AspNet.Identity;
using ServiceLayer.Contracts;

namespace ServiceLayer.EFServiecs.Roles
{
    public class CustomRoleStore : ICustomRoleStore
    {
        private readonly IRoleStore<Role, long> _roleStore;

        public CustomRoleStore(IRoleStore<Role, long> roleStore)
        {
            _roleStore = roleStore;
        }
    }
}
