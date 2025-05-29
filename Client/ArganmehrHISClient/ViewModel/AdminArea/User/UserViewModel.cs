using DomainClasses;
using DomainClasses.Entities.Common;
using DomainClasses.Entities.Users;

namespace ViewModel.AdminArea.User
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsSystemAccount { get; set; }
        public string Name { get; set; }
        public string RegisterDate { get; set; }
        public string LastActivityDate { get; set; }
        public string Roles { get; set; }
        


    }
}
