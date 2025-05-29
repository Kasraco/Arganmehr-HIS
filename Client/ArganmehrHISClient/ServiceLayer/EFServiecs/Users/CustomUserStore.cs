using System.Data.Entity;
using System.Threading.Tasks;
using DataLayer.Context;
using DomainClasses.Entities.Users;
using Microsoft.AspNet.Identity.EntityFramework;
using ServiceLayer.Contracts;

namespace ServiceLayer.EFServiecs.Users
{
    public class CustomUserStore : UserStore<User, Role, long, UserLogin, UserRole, UserClaim>, ICustomUserStore
    {
        private readonly DbSet<User> _users;

        public CustomUserStore(ApplicationDbContext dbContext)
            : base(dbContext)
        {
            _users = (DbSet<User>)dbContext.Set<User>();
        }

        public override Task<User> FindByIdAsync(long userId)
        {
            return _users.FindAsync(userId);
        }
    }
}
