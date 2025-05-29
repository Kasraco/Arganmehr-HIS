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
        InstitutionModel GetInstitution(int InstitutionId);
        List<InstitutionModel> GetInstitutions();
        IEnumerable<InstitutionModel> GetPageList(out int total, InstituionSearchRequest SearchModel);
       void InsertInstitution(InstitutionModel model);

       void EditInstitution(InstitutionModel model);
       void DeleteInstitution(int InstitutionId);
       void DeleteAllInstitution();

       bool ExistInstitution(string InstitutionTitle);

       string GenerateInstitutionCode();
       bool ExistInstitutionCode(string InstitutionCode);
       bool ExistEmailAddress(string EmailAddress);
       bool ExistUserName(string UserName);

    }
}
