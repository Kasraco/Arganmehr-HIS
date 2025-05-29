using AutoMapper;
using DataLayer.Context;
using DomainClasses.Entities.AM;
using ServiceLayer.Contracts.AM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Arganmehr;
using ViewModel.AdminArea.Arganmehr.Institution;
using EntityFramework.Extensions;
using Common.Helpers.Extentions;

namespace ServiceLayer.EFServiecs.AM
{
    public class InstitutionService : IInstitutionService
    {

        private readonly IUnitOfWork _uow;
        private readonly IDbSet<Institution> _Institution;
        private readonly IMappingEngine _mappingEngine;
        public InstitutionService(IUnitOfWork uow, IMappingEngine mappingEngine)
        {
            _uow = uow;
            _mappingEngine = mappingEngine;
            _Institution = _uow.Set<Institution>();

        }
        public InstitutionModel GetInstitution(int InstitutionId)
        {
            var InstList = _Institution.Find(InstitutionId);

            return _mappingEngine.Map<InstitutionModel>(InstList);
        }

        public List<InstitutionModel> GetInstitutions()
        {
            var InstList = _Institution.ToList();

            return _mappingEngine.Map<List<InstitutionModel>>(InstList);
        }

        public IEnumerable<InstitutionModel> GetPageList(out int total, InstituionSearchRequest SearchModel)
        {
            var institutions = _Institution.AsNoTracking().OrderBy(a => a.Id).AsQueryable();

            if (!string.IsNullOrEmpty(SearchModel.SearchFieldValue))
            {
                switch (SearchModel.SearchField)
                {
                    case InstituionSearchField.Title:
                        institutions = institutions.Where(x => x.InstitutionTitle.Contains(SearchModel.SearchFieldValue)).AsQueryable();
                        break;
                    case InstituionSearchField.Email:
                        institutions = institutions.Where(x => x.EmailAddress.Contains(SearchModel.SearchFieldValue)).AsQueryable();
                        break;
                    case InstituionSearchField.Address:
                        institutions = institutions.Where(x => x.Address.Contains(SearchModel.SearchFieldValue)).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }


            if (SearchModel.SortDirection == SortDirection.Ascending)
            {
                switch (SearchModel.SortBy)
                {
                    case InstituionSortBy.Title:
                        institutions = institutions.OrderBy(x => x.InstitutionTitle).AsQueryable();
                        break;
                    case InstituionSortBy.Email:
                        institutions = institutions.OrderBy(x => x.EmailAddress).AsQueryable();
                        break;
                    case InstituionSortBy.Address:
                        institutions = institutions.OrderBy(x => x.Address).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }
            else
            {
                switch (SearchModel.SortBy)
                {
                    case InstituionSortBy.Title:
                        institutions = institutions.OrderByDescending(x => x.InstitutionTitle).AsQueryable();
                        break;
                    case InstituionSortBy.Email:
                        institutions = institutions.OrderByDescending(x => x.EmailAddress).AsQueryable();
                        break;
                    case InstituionSortBy.Address:
                        institutions = institutions.OrderByDescending(x => x.Address).AsQueryable();
                        break;
                    ////todo:add other fields for sort
                }
            }

            total = institutions.FutureCount();

            var query = SearchModel.PageSize == PageSize.All ? institutions.Future().ToList() :
                institutions.OrderBy(x => x.InstitutionTitle)
                    .Skip((SearchModel.PageIndex - 1) * (int)SearchModel.PageSize).Take((int)SearchModel.PageSize)
                    .Future().ToList();
            return _mappingEngine.Map<IList<InstitutionModel>>(query);


        }

        public void InsertInstitution(InstitutionModel model)
        {
            var im = _mappingEngine.Map<Institution>(model);
            im.CreateDate = DateTime.Now;
            _Institution.Add(im);
            _uow.SaveChanges();
        }

        public void EditInstitution(InstitutionModel model)
        {
            var passwordModify = false;


            if (string.IsNullOrEmpty(model.InstitutionCode) || string.IsNullOrEmpty(model.InstitutionTitle))
                return;
            var im = _Institution.Find(model.Id);
            _uow.MarkAsDetached(im);
            _mappingEngine.Map(model, im);

            _uow.SaveChanges();

        }

        public void DeleteInstitution(int InstitutionId)
        {
            var im = _Institution.Find(InstitutionId);
            if (im != null)
                _Institution.Remove(im);
            return;
        }

        public void DeleteAllInstitution()
        {
            throw new NotImplementedException();
        }

        public bool ExistInstitution(string InstitutionTitle)
        {
            return _Institution.Any(x => x.InstitutionTitle.Contains(InstitutionTitle));
        }


        public string GenerateInstitutionCode()
        {
            bool DontGenerated = true;
            string CodeGenereted = "";
            while (DontGenerated)
            {
                CodeGenereted.GenerateCode(6);
                DontGenerated = ExistInstitutionCode(CodeGenereted);
            }
            return CodeGenereted;
        }

        public bool ExistInstitutionCode(string InstitutionCode)
        {
            return _Institution.Any(x => x.InstitutionCode.Contains(InstitutionCode));

        }


        public bool ExistEmailAddress(string EmailAddress)
        {
            return _Institution.Any(x => x.EmailAddress.Contains(EmailAddress));

        }


        public bool ExistUserName(string UserName)
        {
            return _Institution.Any(x => x.UserName == UserName);

        }

        public InstitutionModel GetInstitution()
        {
            InstitutionModel institutionModel = null;

            if (_Institution.Count() > 0)
            {
                var lst = _Institution.First();
                institutionModel = _mappingEngine.Map<InstitutionModel>(lst);
            }
        
            return institutionModel;
        }

        public InstitutionModel GetInstitution(string InstitutionCode)
        {
            InstitutionModel institutionModel = null;

            var lst = _Institution.Where(x => x.InstitutionCode.Contains(InstitutionCode));
            if (lst.Count() > 0)
            {
                var institutionFirst = lst.First();
                institutionModel = _mappingEngine.Map<InstitutionModel>(institutionFirst);

            }
            else
            {
                institutionModel = null;
            }
            return institutionModel;

        }

        public bool ExistInstitution(string Title, string InstitutionCode)
        {
            return _Institution.Any(x => x.InstitutionCode.Contains(InstitutionCode) && x.InstitutionTitle.Contains(Title));

        }

        public bool ExistInstitution(int Id, string InstitutionCode)
        {
            return _Institution.Any(x => x.InstitutionCode.Contains(InstitutionCode) && x.Id == Id);

        }


        public bool ExistInstitutionURL(string URL)
        {
            return _Institution.Any(x => x.WebServiceAddress.Contains(URL));

        }

     
    }
}
