using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using RefactorThis.GraphDiff;

using System.Web;
using AutoMapper;
using Common.Caching;
using Common.Controller;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.EF.Filters;
using DomainClasses.Entities.Users;

using ServiceLayer.Contracts;
using ServiceLayer.CustomAspNetIdentity;
using ServiceLayer.QueryExtensions;
using ServiceLayer.Security;
using Utility;
using ViewModel.AdminArea.User;

namespace ServiceLayer.EFServiecs.Users
{
    public class ApplicationUserManager : UserManager<User, long>, IApplicationUserManager
    {

        #region Fields

        private readonly IIdentity _identity;
        private User _user;
        private readonly HttpContextBase _contextBase;
        private readonly IPermissionService _permissionService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbSet<User> _users;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IMappingEngine _mappingEngine;

        #endregion

        #region Constructor

        public ApplicationUserManager(IIdentity identity, HttpContextBase contextBase, IPermissionService permissionService, IUserStore<User, long> userStore, IApplicationRoleManager roleManager, IUnitOfWork unitOfWork,
            IMappingEngine mappingEngine, IDataProtectionProvider dataProtectionProvider,
             IIdentityMessageService smsService,
            IIdentityMessageService emailService)
            : base(userStore)
        {
            _permissionService = permissionService;
            AutoCommitEnabled = true;
            _dataProtectionProvider = dataProtectionProvider;
            _mappingEngine = mappingEngine;
            EmailService = emailService;
            SmsService = smsService;
            _unitOfWork = unitOfWork;
            _users = _unitOfWork.Set<User>();
            _roleManager = roleManager;
            _contextBase = contextBase;
            _identity = identity;

            CreateApplicationUserManager();


        }

        #endregion

        #region GenerateUserIdentityAsync
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(User user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            return userIdentity;
        }
        #endregion

        #region HasPassword

        public async Task<bool> HasPassword(long userId)
        {
            var user = await FindByIdAsync(userId);
            return user != null && user.PasswordHash != null;
        }

        #endregion

        #region HasPhoneNumber
        public async Task<bool> HasPhoneNumber(long userId)
        {
            var user = await FindByIdAsync(userId);
            return user != null && user.PhoneNumber != null;
        }
        #endregion

        #region OnValidateIdentity
        public Func<CookieValidateIdentityContext, Task> OnValidateIdentity()
        {
            return CustomSecurityStampValidator.OnValidateIdentity(
                         validateInterval: TimeSpan.FromMinutes(0),
                         regenerateIdentityCallback: GenerateUserIdentityAsync,
                         getUserIdCallback: identity => identity.GetUserId<long>());
        }

        #endregion

        #region SeedDatabase

        public void SeedDatabase()
        {
            const string systemAdminUserName = "SystemAdmin";
            const string systemAdminEmail = "Admin@gmail.com";
            const string systemAdminPassword = "Admin1234@gmail.com";
            const string systemAdminNameforShow = "مدیر سیستم";
            var newUser = this.FindByName(systemAdminUserName);
            if (newUser == null)
            {
                newUser = new User
                {
                    UserName = systemAdminUserName.ToLower(),
                    EmailConfirmed = true,
                    IsSystemAccount = true,
                    PhoneNumberConfirmed = true,
                    Email = systemAdminEmail.FixGmailDots(),
                    RegisterDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    AvatarFileName = "avatar.jpg",
                    NameForShow = systemAdminNameforShow
                };
                this.Create(newUser, systemAdminPassword);
                this.SetLockoutEnabled(newUser.Id, false);
            }
            var userRoles = _roleManager.FindUserRoles(newUser.Id);
            if (userRoles.Any(a => a == StandardRoles.SuperAdministrators)) return;
            this.AddToRole(newUser.Id, StandardRoles.SuperAdministrators);
        }

        #endregion

        #region GenerateUserIdentityAsync
        private static async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager, User user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            return userIdentity;
        }
        #endregion

        #region GetAllUsersAsync
        public Task<List<User>> GetAllUsersAsync()
        {
            return Users.ToListAsync();
        }
        #endregion

        #region CreateApplicationUserManager

        private void CreateApplicationUserManager()
        {
            ClaimsIdentityFactory = new CustomClaimsIdentityFactory();

            UserValidator = new CustomUserValidator<User, long>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            PasswordValidator = new CustomPasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            //RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser, int>
            //{
            //    MessageFormat = "کد فعال سازی شما {0} است"
            //});
            //RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser, int>
            //{
            //    Subject = "کد فعال سازی",
            //    BodyFormat = "کد فعال سازی شما {0} است"
            //});


            if (_dataProtectionProvider == null) return;

            var dataProtector = _dataProtectionProvider.Create("Asp.net Identity");
            UserTokenProvider = new DataProtectorTokenProvider<User, long>(dataProtector);

        }
        #endregion

        #region DeleteAll
        public async Task RemoveAll()
        {
            await Users.DeleteAsync();
        }
        #endregion

        #region GetUsersWithRoleIds
        public IList<User> GetUsersWithRoleIds(params long[] ids)
        {
            return Users.Where(applicationUser => ids.Contains(applicationUser.Id))
                .ToList();
        }
        #endregion

        #region AutoCommitEnabled
        public bool AutoCommitEnabled { get; set; }

        #endregion

        #region Any
        public bool Any()
        {
            return _users.Any();
        }
        #endregion

        #region AddRange
        public void AddRange(IEnumerable<User> users)
        {
            _unitOfWork.AddThisRange(users);
        }
        #endregion

        #region GetPageList
        public IList<UserViewModel> GetPageList(out int total, UserSearchRequest search)
        {
            var users = _users.AsNoTracking().OrderBy(a => a.Id).AsQueryable();
            if (search.RoleIds != null && search.RoleIds.Length > 0)
            {
                users =
                    users.Include(a => a.Roles)
                        .Where(a => a.Roles.Select(r => r.RoleId).Intersect(search.RoleIds).Any())
                        .AsQueryable();
            }


            if (!string.IsNullOrEmpty(search.SearchFieldValue))
            {
                switch (search.SearchField)
                {
                    case UserSearchField.UserName:
                        users = users.Where(user => user.UserName.Contains(search.SearchFieldValue)).AsQueryable();
                        break;
                    case UserSearchField.Email:
                        users = users.Where(user => user.Email.Contains(search.SearchFieldValue)).AsQueryable();
                        break;
                    case UserSearchField.LastLoginIp:
                        users = users.Where(user => user.LastIp.Contains(search.SearchFieldValue)).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }


            if (search.SortDirection == SortDirection.Ascending)
            {
                switch (search.SortBy)
                {
                    case UserSortBy.UserName:
                        users = users.OrderBy(user => user.UserName).AsQueryable();
                        break;
                    case UserSortBy.LastLogingDate:
                        users = users.OrderBy(user => user.LastLoginDate).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }
            else
            {
                switch (search.SortBy)
                {
                    case UserSortBy.UserName:
                        users = users.OrderByDescending(user => user.UserName).AsQueryable();
                        break;
                    case UserSortBy.LastLogingDate:
                        users = users.OrderByDescending(user => user.LastLoginDate).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }

            total = users.FutureCount();

            var query = search.PageSize == PageSize.All ? users.Future().ToList() :
                users.OrderByUserName()
                    .SkipAndTake(search.PageIndex - 1, (int)search.PageSize)
                    .Future().ToList();
            return _mappingEngine.Map<IList<UserViewModel>>(query);
        }
        #endregion

        #region GetUserByRoles
        public async Task<EditUserViewModel> GetUserByRolesAsync(long id)
        {
            var userWithRoles = await
                 _users.AsNoTracking()
                     .Include(a => a.Roles)
                     .FirstOrDefaultAsync(a => a.Id == id);
            return _mappingEngine.Map<EditUserViewModel>(userWithRoles);
        }

        #endregion

        #region EditUser
        public async Task<bool> EditUser(EditUserViewModel viewModel)
        {
            var passwordModify = false;

            if (viewModel.RoleIds == null || viewModel.RoleIds.Length < 1) return await Task.FromResult(false);

            var user = _users.Find(viewModel.Id);
            _unitOfWork.MarkAsDetached(user);

            if (viewModel.IsBanned)
                viewModel.IsBanned = !user.IsSystemAccount;
            if (viewModel.IsDeleted)
                viewModel.IsDeleted = !user.IsSystemAccount;

            viewModel.IsSystemAccount = user.IsSystemAccount;
            _mappingEngine.Map(viewModel, user);
            var emailModify = viewModel.Email != user.Email;

            if (emailModify)
            {
                user.EmailConfirmed = false;
            }

            if (viewModel.Password.IsNotEmpty())
            {
                passwordModify = true;
                user.PasswordHash = PasswordHasher.HashPassword(viewModel.Password);
            }

            viewModel.RoleIds.ToList().ForEach(roleId => user.Roles.Add(new UserRole { RoleId = roleId, UserId = user.Id }));
            _unitOfWork.Update(user, a => a.AssociatedCollection(u => u.Roles));

            if (user.IsDeleted || user.IsBanned || passwordModify || emailModify)
                this.UpdateSecurityStamp(user.Id);
            else
                await _unitOfWork.SaveAllChangesAsync();

            return await Task.FromResult(true);
        }
        #endregion

        #region SetRolesToUser
        public void SetRolesToUser(User user, IEnumerable<Role> roles)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region AddUser
        public async Task AddUser(AddUserViewModel viewModel)
        {
            var user = _mappingEngine.Map<User>(viewModel);
            viewModel.RoleIds.ToList().ForEach(roleId => user.Roles.Add(new UserRole { RoleId = roleId }));
            await CreateAsync(user, viewModel.Password);

        }

        public User InsertGeustUser()
        {
            AddUserViewModel AUVM = new AddUserViewModel
            {
                Email = "UserGuest@Arganmehr.ir",
                UserName = "UserGuest1",
                Password = "353152119",
                NameForShow = "Guest User",
                AvatarFileName = "null",
                IsBanned = false     
            };
           
            var user = _mappingEngine.Map<User>(AUVM);
            user.Roles.Add(new UserRole { RoleId = 6 });
            user.IsDeleted = false;
            var u = _users.Add(user);
            _unitOfWork.SaveChanges();
            return u;

        }
        #endregion

        #region LogicalRemove
        public async Task<bool> LogicalRemove(long id)
        {
            var key = id.ToString(CultureInfo.InvariantCulture) + "_roles";
            _contextBase.InvalidateCache(key);
            _unitOfWork.EnableFiltering(UserFilters.NotSystemAccountList);
            var result = await _users.Where(a => a.Id == id).UpdateAsync(a => new User { IsDeleted = true });
            return result > 0;
        }
        #endregion

        #region Validations

        public bool CheckUserNameExist(string userName, long? id)
        {
            return id == null
                ? _users.Any(a => a.UserName == userName.ToLower())
                : _users.Any(a => a.UserName == userName.ToLower() && a.Id != id.Value);
        }

        public bool CheckEmailExist(string email, long? id)
        {
            email = email.FixGmailDots();
            return id == null
               ? _users.Any(a => a.Email == email.ToLower())
               : _users.Any(a => a.Email == email.ToLower() && a.Id != id.Value);
        }

        public bool CheckNameForShowExist(string nameForShow, long? id)
        {
            var namesForShow = _users.Select(a => new { Name = a.NameForShow, a.Id }).ToList();
            nameForShow = nameForShow.GetFriendlyPersianName();
            return id == null
             ? namesForShow.Any(a => a.Name.GetFriendlyPersianName() == nameForShow)
             : namesForShow.Any(a => a.Name.GetFriendlyPersianName() == nameForShow && a.Id != id.Value);
        }

        public bool CheckGooglePlusIdExist(string googlePlusId, long? id)
        {
            return id == null
                    ? _users.Any(a => a.GooglePlusId == googlePlusId)
                    : _users.Any(a => a.GooglePlusId == googlePlusId && a.Id != id.Value);
        }

        public bool CheckFacebookIdExist(string faceBookId, long? id)
        {
            return id == null
              ? _users.Any(a => a.FaceBookId == faceBookId)
              : _users.Any(a => a.FaceBookId == faceBookId && a.Id != id.Value);
        }

        public bool CheckPhoneNumberExist(string phoneNumber, long? id)
        {
            return id == null
               ? _users.Any(a => a.PhoneNumber == phoneNumber)
               : _users.Any(a => a.PhoneNumber == phoneNumber && a.Id != id.Value);
        }
        #endregion

        #region override GetRolesAsync
        public async override Task<IList<string>> GetRolesAsync(long userId)
        {

            var userPermissions = await _roleManager.FindUserPermissions(userId);
            ////todo: any permission form other sections
            return userPermissions;
        }

        public bool ShouldRefreshPerissions(long userId)
        {
            var user = _users.Find(userId);
            return user.IsChangedPermissions || user.IsBanned || user.IsDeleted;
        }
        #endregion

        #region CreateAsync
        public async Task<long> CreateAsync(ViewModel.Account.RegisterViewModel viewModel)
        {
            var user = _mappingEngine.Map<User>(viewModel);
            await CreateAsync(user, viewModel.Password);
            var defultRoleName = await _roleManager.GetDefaultRoleForRegister();
            if (defultRoleName.IsNotEmpty())
                await AddToRoleAsync(user.Id, defultRoleName);
            return user.Id;
        }
        #endregion

        #region CustomValidatePasswordAsync
        public async Task<string> CustomValidatePasswordAsync(string pass)
        {
            var result = await PasswordValidator.ValidateAsync(pass);
            return result.Errors.GetUserManagerErros();
        }
        #endregion

        #region ChechIsBanneduser
        public bool CheckIsUserBannedOrDelete(long id)
        {
            return _users.Any(a => a.Id == id && (a.IsBanned || a.IsDeleted));
        }
        public bool CheckIsUserBanned(long id)
        {
            _unitOfWork.EnableFiltering(UserFilters.BannedList);
            var result = _users.Any(a => a.Id == id);
            _unitOfWork.DisableFiltering(UserFilters.BannedList);
            return result;
        }

        public bool CheckIsUserBannedOrDelete(string userName)
        {
            return _users.Any(a => a.UserName == userName.ToLower() && (a.IsBanned || a.IsDeleted));
        }
        public bool CheckIsUserBannedOrDeleteByEmail(string email)
        {
            return _users.Any(a => a.Email == email.FixGmailDots() && (a.IsBanned || a.IsDeleted));
        }
        public bool CheckIsUserBannedByEmail(string email)
        {
            _unitOfWork.EnableFiltering(UserFilters.BannedList);
            email = email.FixGmailDots();
            var result = _users.Any(a => a.Email == email);
            _unitOfWork.DisableFiltering(UserFilters.BannedList);
            return result;
        }
        public bool CheckIsUserBannedByUserName(string userName)
        {
            _unitOfWork.EnableFiltering(UserFilters.BannedList);
            userName = userName.ToLower();
            var result = _users.Any(a => a.UserName == userName);
            _unitOfWork.DisableFiltering(UserFilters.BannedList);
            return result;
        }
        #endregion

        #region GetEmailStore
        public IUserEmailStore<User, long> GetEmailStore()
        {
            var cast = Store as IUserEmailStore<User, long>;
            if (cast == null)
            {
                throw new NotSupportedException("not support");
            }
            return cast;
        }

        #endregion

        #region Private
        //private static string[] SplitString(string dependencies)
        //{
        //    if (dependencies == null) return new string[0];
        //    var split = from dependency in dependencies.Split(',')
        //                let lowerDependency = dependency.ToLower()
        //                where !string.IsNullOrEmpty(lowerDependency)
        //                select lowerDependency;
        //    return split.ToArray();
        //}
        #endregion

        #region IsEmailConfirmedByUserNameAsync
        public bool IsEmailConfirmedByUserNameAsync(string userName)
        {
            _unitOfWork.EnableFiltering(UserFilters.EmailConfirmedList);
            var result = _users.Any(a => a.UserName == userName.ToLower());
            _unitOfWork.DisableFiltering(UserFilters.EmailConfirmedList);
            return result;
        }

        #endregion

        #region IsEmailAvailableForConfirm
        public bool IsEmailAvailableForConfirm(string email)
        {
            email = email.FixGmailDots();
            return _users.Any(a => a.Email == email);
        }
        #endregion

        #region EditSecurityStamp
        public void EditSecurityStamp(long userId)
        {
            this.UpdateSecurityStamp(userId);
        }
        #endregion

        #region FindUserById
        public User FindUserById(long id)
        {
            return this.FindById(id);
        }
        #endregion

        #region CurrentUser
        public User GetCurrentUser()
        {
            return _user ?? (_user = this.FindById(GetCurrentUserId()));
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return _user ?? (_user = await FindByIdAsync(GetCurrentUserId()));
        }

        public long GetCurrentUserId()
        {
            return _identity.GetUserId<long>();
        }
        #endregion



        public bool CanAccess(int userId, string areaName, string controllerName, string actionName, string DependencyActionNames)
        {
           
            throw new NotImplementedException();
        }
    }
}
