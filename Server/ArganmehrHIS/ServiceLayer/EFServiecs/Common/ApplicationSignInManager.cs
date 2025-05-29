using DomainClasses.Entities.Users;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ServiceLayer.Contracts;
using ServiceLayer.EFServiecs.Users;

namespace ServiceLayer.EFServiecs.Common
{
    public class ApplicationSignInManager : SignInManager<User, long>, IApplicationSignInManager
    {

        #region Fields
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        #endregion

        #region Constructor

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }
        #endregion
    }
}
