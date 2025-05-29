using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Common.Controller;
using Common.Filters;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.Entities.Users;
using ServiceLayer.Contracts;
using ServiceLayer.Security;
using Utility;
using ViewModel.AdminArea.User;
using Web.Extentions;
using WebGrease.Css.Extensions;
using MvcSiteMapProvider;
using IocConfig.Controllers;

namespace Web.Areas.Administrator.Controllers
{
    [Description("مدیریت کاربران")]
    [CustomAuthorizeAttribute]
    public partial class UserController : AdminControllerBase
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationRoleManager _roleManager;

        #endregion

        #region Constructor

        public UserController(IUnitOfWork unitOfWork, IPermissionService permissionService, IApplicationRoleManager roleManager,
            IApplicationUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
        }

        #endregion

        #region List,ListAjax
        [HttpGet]
        [DisplayName("مشاهده لیست کاربران")]
        [ActivityLog(Name = "ViewUsers", Description = "مشاهده کاربران")]
        [MvcSiteMapNode(ParentKey = "Administrator_Home", Title="مدیریت کاربران")]
        public virtual async Task<ActionResult> List()
        {
            await PopulateRoles();
            return View();
        }

        //[CheckReferrer]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual ActionResult ListAjax(UserSearchRequest search)
        {
            int total;
            var users = _userManager.GetPageList(out total, search);
            search.Total = total;
            var viewModel = new UserListViewModel
            {
                Users = users,
                SearchRequest = search
            };
            return PartialView(MVC.Administrator.User.Views.ViewNames._ListAjax, viewModel);
        }
        #endregion

        #region Edit
        [Route("Edit/{id}")]
        [HttpGet]
        [DisplayName("ویرایش کاربر")]
        [ActivityLog(Name = "EditUser", Description = "ویرایش کاربر")]
        public virtual async Task<ActionResult> Edit(long? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var viewModel = await _userManager.GetUserByRolesAsync(id.Value);
            if (viewModel == null) return HttpNotFound();
            await PopulateRoles(viewModel.Roles.Select(a => a.RoleId).ToArray());
            return View(viewModel);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        //[CheckReferrer]
        [AllowUploadSpecialFilesOnly(".jpg,.png,.gif", true)]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(EditUserViewModel viewModel)
        {
            #region Validation
            if (_userManager.CheckUserNameExist(viewModel.UserName, viewModel.Id))
                this.AddErrors("UserName", "این نام کاربری قبلا در سیستم ثبت شده است");
            if (_userManager.CheckNameForShowExist(viewModel.NameForShow, viewModel.Id))
                this.AddErrors("NameForShow", "این نام نمایشی قبلا  در سیستم ثبت شده است");
            if (viewModel.Password.IsNotEmpty() && !viewModel.Password.IsSafePasword())
                this.AddErrors("Password", "این کلمه عبور به راحتی قابل تشخیص است");
            if (_userManager.CheckEmailExist(viewModel.Email, viewModel.Id))
                this.AddErrors("Email", "این ایمیل قبلا در سیستم ثبت شده است");
            if (_userManager.CheckFacebookIdExist(viewModel.FaceBookId, viewModel.Id))
                this.AddErrors("FaceBookId", "این آد دی قبلا در سیستم ثبت شده است");
            if (_userManager.CheckFacebookIdExist(viewModel.GooglePlusId, viewModel.Id))
                this.AddErrors("GooglePlusId", "این آد دی قبلا در سیستم ثبت شده است");

            #endregion

            if (!ModelState.IsValid)
            {
                await PopulateRoles(viewModel.RoleIds);
                return View(viewModel);
            }

            var dbUser = await _userManager.FindByIdAsync(viewModel.Id);
            if (dbUser == null) return HttpNotFound();

            var avatarName = "avatar.jpg";
            if (viewModel.AvatarImage != null && viewModel.AvatarImage.ContentLength > 0)
            {
                avatarName = this.UploadFile(viewModel.AvatarImage);
            }
            viewModel.AvatarFileName = avatarName;

            if (!await _userManager.EditUser(viewModel))
            {
                this.NotyWarning("لطفا برای کاربر مورد نظر ، گروه کاربری تعیین کنید");
                await PopulateRoles();
                return View(viewModel);
            }

            this.NotySuccess("عملیات  ویرایش کاربر با موفقیت انجام شد");
            return RedirectToAction(MVC.Administrator.User.ActionNames.List, MVC.Administrator.User.Name);
        }

        #endregion

        #region Create

        [HttpGet]
        [DisplayName("ثبت کاربر جدید")]
        [ActivityLog(Name = "AddUser", Description = "درج کاربر جدید")]
        public virtual async Task<ActionResult> Create()
        {
            await PopulateRoles();
            var viewModel = new AddUserViewModel
            {
                CanUploadFile = true,
                CanModifyFirsAndLastName = true,
                CanChangeProfilePicture = true
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        [AllowUploadSpecialFilesOnly(".jpg,.png,.gif", true)]
        //[CheckReferrer]
        public virtual async Task<ActionResult> Create(AddUserViewModel viewModel)
        {
            #region Validation
            if (_userManager.CheckUserNameExist(viewModel.UserName, null))
                this.AddErrors("UserName", "این نام کاربری قبلا در سیستم ثبت شده است");
            if (_userManager.CheckNameForShowExist(viewModel.NameForShow, null))
                this.AddErrors("NameForShow", "این نام نمایشی قبلا  در سیستم ثبت شده است");
            if (!viewModel.Password.IsSafePasword())
                this.AddErrors("Password", "این کلمه عبور به راحتی قابل تشخیص است");
            if (_userManager.CheckEmailExist(viewModel.Email, null))
                this.AddErrors("Email", "این ایمیل قبلا در سیستم ثبت شده است");
            if (_userManager.CheckFacebookIdExist(viewModel.FaceBookId, null))
                this.AddErrors("FaceBookId", "این آد دی قبلا در سیستم ثبت شده است");
            if (_userManager.CheckFacebookIdExist(viewModel.GooglePlusId, null))
                this.AddErrors("GooglePlusId", "این آد دی قبلا در سیستم ثبت شده است");
            #endregion

            if (!ModelState.IsValid)
            {
                await PopulateRoles(viewModel.RoleIds);
                return View(viewModel);
            }
            if (viewModel.RoleIds == null || viewModel.RoleIds.Length < 1)
            {
                this.NotyWarning("لطفا برای  کاربر مورد نظر ، گروه کاربری تعیین کنید");
                await PopulateRoles();
                return View(viewModel);
            }

            var avatarName = "avatar.jpg";
            if (viewModel.AvatarImage != null && viewModel.AvatarImage.ContentLength > 0)
            {
                avatarName = this.UploadFile(viewModel.AvatarImage);
            }
            viewModel.AvatarFileName = avatarName;
            await _userManager.AddUser(viewModel);
            this.NotySuccess("عملیات ثبت  کاربر جدید با موفقیت انجام شد");
            return RedirectToAction(MVC.Administrator.User.ActionNames.List, MVC.Administrator.User.Name);
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CheckReferrer] 
        [DisplayName("حذف کاربر")]
        [ActivityLog(Name = "DeleteUser", Description = "عملیات حذف کاربر")]
        public virtual async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!await _userManager.LogicalRemove(id.Value))
            {
                this.NotyWarning("این  کاربر ، کاربر سیستمی است و حذف آن باعث اختلال در سیستم خواهد شد");
                return RedirectToAction(MVC.Administrator.User.ActionNames.List, MVC.Administrator.User.Name);
            }
            this.NotySuccess("کاربر مورد نظر با موفقیت حذف شد");
            return RedirectToAction(MVC.Administrator.User.ActionNames.List, MVC.Administrator.User.Name);

        }

        #endregion

        #region RemoteValidations



        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult PhoneNumberExist(string phoneNumber, int? id)
        {
            return _userManager.CheckPhoneNumberExist(phoneNumber, id) ? Json(false) : Json(true);
        }


        [HttpPost]
        [AjaxOnly]
        //[CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult GooglePlusIdExist(string googlePlusId, int? id)
        {
            return _userManager.CheckGooglePlusIdExist(googlePlusId, id) ? Json(false) : Json(true);
        }


        [HttpPost]
        [AjaxOnly]
        //[CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult FaceBookIdExist(string faceBookId, int? id)
        {
            return _userManager.CheckFacebookIdExist(faceBookId, id) ? Json(false) : Json(true);
        }
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult IsEmailAvailable(string email)
        {
            return _userManager.IsEmailAvailableForConfirm(email) ? Json(true) : Json(false);
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult CheckPassword(string password)
        {
            return password.IsSafePasword() ? Json(true) : Json(false);
        }
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult IsNameForShowExist(string nameForShow, int? id)
        {
            return _userManager.CheckNameForShowExist(nameForShow, id) ? Json(false) : Json(true);
        }
        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult IsEmailExist(string email, int? id)
        {
            var check = _userManager.CheckEmailExist(email, id);
            return check ? Json(false) : Json(true);
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult IsUserNameExist(string userName, int? id)
        {
            return _userManager.CheckUserNameExist(userName, id) ? Json(false) : Json(true);
        }
        #endregion

        #region Private
        [NonAction]
        private async Task PopulateRoles(params long[] selectedIds)
        {
            var roles = await _roleManager.GetAllAsSelectList();

            if (selectedIds != null)
            {
                roles.ForEach(a => a.Selected = selectedIds.Any(b => long.Parse(a.Value) == b));
            }

            ViewBag.Roles = roles;
        }
        
        #endregion
    }
}