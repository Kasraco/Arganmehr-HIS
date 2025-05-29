using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Arganmehr;
using ViewModel.AdminArea.Arganmehr.Institution;

namespace ServiceLayer.Contracts.AM
{
    public interface IInstitutionService
    {
        InstitutionModel GetInstitution();
        InstitutionModel GetInstitution(int InstitutionId);
        InstitutionModel GetInstitution(string InstitutionCode);
        List<InstitutionModel> GetInstitutions();
        IEnumerable<InstitutionModel> GetPageList(out int total, InstituionSearchRequest SearchModel);
        void InsertInstitution(InstitutionModel model);

        void EditInstitution(InstitutionModel model);
        void DeleteInstitution(int InstitutionId);

        bool ExistInstitution(string InstitutionTitle);

        string GenerateInstitutionCode();
        bool ExistInstitutionCode(string InstitutionCode);
        bool ExistInstitutionURL(string URL);
        bool ExistInstitution(string Title, string InstitutionCode);
        bool ExistInstitution(int Id, string InstitutionCode);
        bool ExistEmailAddress(string EmailAddress);
        bool ExistUserName(string UserName);

    }
}
