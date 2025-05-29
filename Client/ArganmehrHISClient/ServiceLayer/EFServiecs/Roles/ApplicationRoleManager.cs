using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.Context;
using DomainClasses.EF.Filters;
using DomainClasses.Entities.Users;
using EFSecondLevelCache;
using EntityFramework.Extensions;
using Microsoft.AspNet.Identity;
using ServiceLayer.Contracts;
using ServiceLayer.Security;
using Utility;
using ViewModel.AdminArea.Role;

namespace ServiceLayer.EFServiecs.Roles
{
    public class RoleManager : RoleManager<Role, long>, IApplicationRoleManager
    {
        #region Fields
        private readonly IMappingEngine _mappingEngine;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;
        private readonly IDbSet<Role> _roles;
        private readonly IDbSet<User> _users;
        #endregion

        #region Constructor
        public RoleManager(IMappingEngine mappingEngine, IPermissionService permissionService, IUnitOfWork unitOfWork, IRoleStore<Role, long> roleStore)
            : base(roleStore)
        {
            _unitOfWork = unitOfWork;
            _roles = _unitOfWork.Set<Role>();
            _users = _unitOfWork.Set<User>();
            _permissionService = permissionService;
            _mappingEngine = mappingEngine;
            AutoCommitEnabled = true;
        }
        #endregion

        #region FindRoleByName
        public Role FindRoleByName(string roleName)
        {
            return this.FindByName(roleName); // RoleManagerExtensions
        }
        #endregion

        #region CreateRole
        public IdentityResult CreateRole(Role role)
        {
            return this.Create(role); // RoleManagerExtensions
        }
        #endregion

        #region GetUsersOfRole
        public IList<UserRole> GetUsersOfRole(string roleName)
        {
            return Roles.Where(role => role.Name == roleName)
                             .SelectMany(role => role.Users)
                             .ToList();
        }
        #endregion

        #region GetApplicationUsersInRole
        public IList<User> GetApplicationUsersInRole(string roleName)
        {
            //var roleUserIdsQuery = from role in Roles
            //                       where role.Name == roleName
            //                       from user in role.Users
            //                       select user.UserId;

            return null; //_userManager.GetUsersWithRoleIds(roleUserIdsQuery.ToArray());
        }
        #endregion

        #region FindUserRoles
        public IList<string> FindUserRoles(long userId)
        {
            var userRolesQuery = from role in Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return userRolesQuery.OrderBy(x => x.Name).Select(a => a.Name).ToList();
        }

        public IEnumerable<long> FindUserRoleIds(long userId)
        {
            var userRolesQuery = from role in Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return userRolesQuery.Select(a => a.Id).ToList();
        }

        public async Task<IList<long>> FindUserRoleIdsAsync(long userId)
        {
            var userRolesQuery = from role in Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return await userRolesQuery.Select(a => a.Id).ToListAsync();
        }


        public async Task<IList<string>> FindUserPermissions(long userId)
        {
            var u = _users.Find(userId);
            var URoles = u.Roles.Select(x => x.RoleId).AsQueryable();
            var Roles = _roles.Where(x => URoles.Contains(x.Id)).AsQueryable();
            var qRA = await (from r in Roles
                       from ra in r.RoleAccesses.AsQueryable()
                       where ra.RoleId == r.Id
                       select ( ra.Controller + "-" + ra.Action)
                       ).Distinct().AsNoTracking().ToListAsync();

            return _permissionService.GetUserPermissionsAsList(qRA);
        }
        #endregion

        #region GetRolesForUser
        public string[] GetRolesForUser(long userId)
        {
            var roles = FindUserRoles(userId);
            if (roles == null || !roles.Any())
            {
                return new string[] { };
            }

            return roles.ToArray();
        }

        #endregion

        #region IsUserInRole
        public bool IsUserInRole(long userId, string roleName)
        {
            var userRolesQuery = from role in Roles
                                 where role.Name == roleName
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;
            var userRole = userRolesQuery.FirstOrDefault();
            return userRole != null;
        }

        #endregion

        #region GetAllRolesAsync
        public async Task<IList<Role>> GetAllRolesAsync()
        {
            return await Roles.ToListAsync();
        }

        #endregion

        #region SeedDatabase
        /// <summary>
        /// for instal permissions with roles
        /// </summary>
        public void SeedDatabase()
        {
            var standardRoles = StandardRoles.SystemRolesWithPermissions;

            foreach (var role in standardRoles.Select(record => this.FindByName(record.RoleName) ?? new Role
            {
                Name = record.RoleName,
                IsSystemRole = true,
                RoleAccesses =
                    _permissionService.GetPermissions(record.Permissions)
            }).Where(role => role.Id == 0))
            {
                _roles.Add(role);
            }

            _unitOfWork.SaveChanges();
        }

        #endregion

        #region DeleteAll
        public async Task RemoteAll()
        {
            await Roles.DeleteAsync();
        }
        #endregion

        #region GetList


        public async Task<IEnumerable<RoleViewModel>> GetList()
        {
            return await _roles.Project(_mappingEngine).To<RoleViewModel>().ToListAsync();
        }
        #endregion

        #region AddRole
        public Task<bool> AddRole(AddRoleViewModel viewModel)
        {
            //if (viewModel.PermissionNames == null || viewModel.PermissionNames.Length < 1)
            //    return Task.FromResult(false);
            var role = _mappingEngine.Map<Role>(viewModel);
            role.RoleAccesses = new List<RoleAccess>();

            role.RoleAccesses = _permissionService.GetPermissions(viewModel.PermissionNames);

            //foreach (var controller in viewModel.SelectedControllers)
            //{
            //    foreach (var action in controller.Actions)
            //    {
            //        role.RoleAccesses.Add(new RoleAccess { Controller = controller.Name, Action = action.Name });
            //    }
            //}

            _roles.Add(role);
         

            return Task.FromResult(true);
        }
        #endregion

        #region GetRoleByPermissions
        public async Task<EditRoleViewModel> GetRoleByPermissionsAsync(long id)
        {
            var role = await _roles.Include(r => r.RoleAccesses).FirstOrDefaultAsync(r => r.Id == id);
            var viewModel = _mappingEngine.Map<EditRoleViewModel>(role);
            viewModel.PermissionNames = _permissionService.GetUserPermissionsAsList(role.RoleAccesses).ToArray();

            return viewModel;
        }
     
        #endregion

        #region EditRole

        public async Task<bool> EditRole(EditRoleViewModel viewModel)
        {
            if (viewModel.PermissionNames == null || viewModel.PermissionNames.Length < 1)
                return false;
            var role = await ((DbSet<Role>)_roles).FindAsync(viewModel.Id);
            _mappingEngine.Map(viewModel, role);
            role.RoleAccesses = _permissionService.GetPermissions(viewModel.PermissionNames);
            return true;
        }
        #endregion

        #region AddRange
        public void AddRange(IEnumerable<Role> roles)
        {
            _unitOfWork.AddThisRange(roles);
        }
        #endregion

        #region AnyAsync
        public Task<bool> AnyAsync()
        {
            return _roles.AnyAsync();
        }
        public bool Any()
        {
            return _roles.Any();
        }
        #endregion

        #region AutoCommitEnabled
        public bool AutoCommitEnabled { get; set; }
        #endregion

        #region ChechForExisByName
        public bool ChechForExisByName(string name, long? id)
        {
            var roles = _roles.ToList();
            return id == null ? roles.Any(a => a.Name.GetFriendlyPersianName() == name.GetFriendlyPersianName())
                : roles.Any(a => a.Name.GetFriendlyPersianName() == name.GetFriendlyPersianName() && id.Value != a.Id);
        }
        #endregion

        #region GetPageList
        public IEnumerable<RoleViewModel> GetPageList(out int total, string term, int page, int count = 10)
        {
            var roles = _roles.AsNoTracking().OrderBy(a => a.Id).AsQueryable();
            if (!string.IsNullOrEmpty(term))
                roles = roles.Where(a => a.Name.Contains(term));
            total = roles.FutureCount();
            var result =
                roles.Skip((page - 1) * count).Take(count).Project(_mappingEngine).To<RoleViewModel>().Future().ToList();

            return result;
        }
        #endregion

        #region RemoveById
        public async Task RemoveById(long id)
        {
            await _roles.Where(a => a.Id == id).DeleteAsync();
        }

        #endregion

        #region CheckRoleIsSystemRoleAsync
        public async Task<bool> CheckRoleIsSystemRoleAsync(long id)
        {
            return await _roles.AnyAsync(a => a.Id == id && a.IsSystemRole);
        }
        #endregion

        #region SetRoleAsRegistrationDefaultRoleAsync
        public async Task SetRoleAsRegistrationDefaultRoleAsync(long id)
        {
            _unitOfWork.EnableFiltering("IsDefaultRegisteRole");
            var role = await _roles.FirstOrDefaultAsync();
            role.IsBanned = true;
            _unitOfWork.DisableFiltering("IsDefaultRegisteRole");
            await _roles.Where(a => a.Id == id).UpdateAsync(a => new Role { IsDefaultForRegister = true });
        }

        #endregion

        #region GetAllAsSelectList
        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectList()
        {
            _unitOfWork.EnableFiltering(RoleFilters.ActiveList);
            var roles = await _roles.AsNoTracking().Project(_mappingEngine).To<SelectListItem>().Cacheable().ToListAsync();
            return roles;
        }
        #endregion

        #region GetDefaultRoleForRegister
        public Task<string> GetDefaultRoleForRegister()
        {
            return _roles.Where(a => a.IsDefaultForRegister).Select(a => a.Name).FirstOrDefaultAsync();
        }
        #endregion





        public Task<bool> GetAccessRoleAsync(long userId, string areaName, string controllerName, string actionName)
        {
            throw new System.NotImplementedException();
        }

        public bool GetAccessRole(long userId, string areaName, string controllerName, string actionName)
        {
            throw new System.NotImplementedException();
        }
    }
}
