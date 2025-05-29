using Common.Filters;
using DataLayer.Context;
using IocConfig.Controllers;
using Newtonsoft.Json;
using ServiceLayer.Contracts.AM;
using ServiceLayer.Contracts.AM.CA;
using ServiceLayer.EFServiecs.AM.CA;
using ServiceLayer.WebServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel.AdminArea.Arganmehr;
using ViewModel.Install;


namespace Web.Controllers
{

    public partial class InstallController : PublicControllerBase
    {
        private readonly ICentralAdminService _cas;
        private readonly IInstitutionService _institution;
        private readonly IUnitOfWork _uow;
        public InstallController(IUnitOfWork uow,ICentralAdminService cas, IInstitutionService institution)
        {
            _uow = uow;
            _cas = cas;
            _institution = institution;
        }

        #region "ActionResult Method"
        public virtual ActionResult Index()
        {
            return View(MVC.Install.Views.Index);
        }

        [HttpGet]
        public virtual  ActionResult CreateCentralAdmin()
        {
             
            return PartialView(MVC.Install.Views._CreateCA);
        }

        [HttpPost]
        public virtual async Task<ActionResult> CreateCentralAdmin(InstalInstitution model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(MVC.Install.Views._CreateCA, model);
            }
            var CA1 = _cas.GetFirstCentralAdmin();
            var CA = _cas.GetCentralAdminById(CA1.Id);
            CA.SecureUrl = model.UserName;
            CA.Url = model.CentralAdminWebServiceURL;
            CA.PWSLinkServer = model.Password;
            _cas.UpdateCentralAdmin(CA);
            _uow.SaveAllChanges();
            var InsModelFromServer = await GetInstitutionFromServer(model.CentralAdminWebServiceURL, model.InstitutionCode);
            if (_institution.ExistInstitutionCode(model.InstitutionCode))
            {
                var inscm = _institution.GetInstitution(model.InstitutionCode);

                inscm.Address = InsModelFromServer.Address;
                inscm.CountryDivitionCode = InsModelFromServer.CountryDivitionCode;
                inscm.CreateDate = InsModelFromServer.CreateDate;
                inscm.EmailAddress = InsModelFromServer.EmailAddress;
                inscm.FaxNumber = InsModelFromServer.FaxNumber;
                inscm.InstitutionCode = InsModelFromServer.InstitutionCode;
                inscm.InstitutionTitle = InsModelFromServer.InstitutionTitle;
                inscm.MobileNumber = InsModelFromServer.MobileNumber;
                inscm.Password = InsModelFromServer.Password;
                inscm.PostalCode = InsModelFromServer.PostalCode;
                inscm.TelephoneNumber = InsModelFromServer.TelephoneNumber;
                inscm.UserName = InsModelFromServer.UserName;
                inscm.WebServiceAddress = InsModelFromServer.WebServiceAddress;
                inscm.WebSiteAddress = InsModelFromServer.WebSiteAddress;

                _institution.EditInstitution(inscm);

                return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);

            }

            InstitutionModel INSMClientModel = new InstitutionModel
            {
                Address = InsModelFromServer.Address,
                CountryDivitionCode = InsModelFromServer.CountryDivitionCode,
                CreateDate = InsModelFromServer.CreateDate,
                EmailAddress = InsModelFromServer.EmailAddress,
                FaxNumber = InsModelFromServer.FaxNumber,
                InstitutionCode = InsModelFromServer.InstitutionCode,
                InstitutionTitle = InsModelFromServer.InstitutionTitle,
                MobileNumber = InsModelFromServer.MobileNumber,
                Password = InsModelFromServer.Password,
                PostalCode = InsModelFromServer.PostalCode,
                TelephoneNumber = InsModelFromServer.TelephoneNumber,
                UserName = InsModelFromServer.UserName,
                WebServiceAddress = InsModelFromServer.WebServiceAddress,
                WebSiteAddress = InsModelFromServer.WebSiteAddress
            };

            _institution.InsertInstitution(INSMClientModel);

            _uow.SaveAllChanges();
 

            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
        }
        #endregion

        #region "Remote Validation Method"


        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual async Task<JsonResult> ExistWebService(string CentralAdminWebServiceURL, string InstitutionCode)
        {
            InstitutionServer IS = new InstitutionServer(CentralAdminWebServiceURL);

            var result = await IS.ExistInstitution(InstitutionCode);

            return result ? Json(true) : Json(false);
        }

        [HttpPost]
        [AjaxOnly]
        // [CheckReferrer]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public virtual async Task<JsonResult> ExistInstitutionWebServiceURL(string InstitutionWebServiceURL, string CentralAdminWebServiceURL)
        {
            InstitutionServer IS = new InstitutionServer(CentralAdminWebServiceURL);

            var result = await IS.ExistInstitutionURL(InstitutionWebServiceURL);

            return result ? Json(true) : Json(false);
        }


        #endregion

        #region "Private Method"


        private async Task<InstitutionModel> GetInstitutionFromServer(string CentralAdminWebServiceURL, string institutionCode)
        {
            
            var ca = _cas.GetFirstCentralAdmin();
            InstitutionServer IS = new InstitutionServer(CentralAdminWebServiceURL);
            var insM = await IS.GetInstitution(institutionCode);
            return insM;
        }


        #endregion


       


    }
}
