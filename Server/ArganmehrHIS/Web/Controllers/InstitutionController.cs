using Common.Controller;
using Common.Filters;
using Utility;
using IocConfig.Controllers;
using ServiceLayer.Contracts.AM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ViewModel.AdminArea.Arganmehr;
using ViewModel.AdminArea.Arganmehr.Institution;
using ViewModel.AdminArea.User;

namespace Web.Controllers
{
    [RoutePrefix("Institution")]
    [Route("{action}")]
    public partial class InstitutionController : PublicControllerBase
    {
        private readonly IInstitutionService _institution;
        public InstitutionController(IInstitutionService institution)
        {
            _institution = institution;
        }


        public virtual ActionResult Index()
        {
            return null;
        }


        [HttpGet]
        public virtual ActionResult List()
        {
            return View();
        }


        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public virtual ActionResult ListAjax(InstituionSearchRequest search)
        {
            int total;
            var institutions = _institution.GetPageList(out total, search);
            search.Total = total;
            var viewModel = new InstitutionListViewModel
            {
                Institutions = institutions,
                SearchRequest = search
            };
            return PartialView(MVC.Administrator.User.Views.ViewNames._ListAjax, viewModel);
        }


        [HttpGet]
        public virtual ActionResult AddInstitution()
        {
            return View(MVC.Institution.Views._AddInstitution);
        }


        [HttpPost]
        public virtual ActionResult AddInstitution(InstitutionModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(MVC.Institution.Views._AddInstitution, viewModel);
            }
            _institution.InsertInstitution(viewModel);

            this.NotySuccess("عملیات ثبت  موسسه جدید با موفقیت انجام شد");
            return RedirectToAction(MVC.Institution.ActionNames.List, MVC.Institution.Name);
        }

        [HttpGet]
        public virtual ActionResult EditInstitution(int InstitutionId)
        {
            if (InstitutionId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var viewModel = _institution.GetInstitution(InstitutionId);
            if (viewModel == null) return HttpNotFound();
            return View(viewModel);
        }


        [HttpPost]
        public virtual ActionResult EditInstitution(InstitutionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dbInstitution = _institution.GetInstitution(model.Id);
            if (dbInstitution == null) return HttpNotFound();

            try
            {
                _institution.EditInstitution(model);
            }
            catch (Exception)
            {
                this.NotyWarning("هنگام درج موسسه جدید خطایی رخ داده، لطفا اطلاعات وارد شده را چک کنید");

                return View(model);
            }

            this.NotySuccess("عملیات  ویرایش موسسه با موفقیت انجام شد");
            return RedirectToAction(MVC.Institution.ActionNames.List, MVC.Institution.Name);

        }

        #region "Remote Validation"


        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult ExistInstitutionCode(string InstitutionCode)
        {
            return _institution.ExistInstitutionCode(InstitutionCode) ? Json(false) : Json(true);
        }

        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult ExistInstitutionTitle(string InstitutionTitle)
        {
            return _institution.ExistInstitution(InstitutionTitle) ? Json(false) : Json(true);
        }


        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual JsonResult ExistEmailAddress(string EmailAddress)
        {
            return _institution.ExistEmailAddress(EmailAddress) ? Json(false) : Json(true);
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
        public virtual JsonResult IsUserNameExist(string userName)
        {
            return _institution.ExistUserName(userName) ? Json(false) : Json(true);

        }

        #endregion

    }
}
