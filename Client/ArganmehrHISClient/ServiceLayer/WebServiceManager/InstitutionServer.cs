using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Arganmehr;

namespace ServiceLayer.WebServiceManager
{
    public class InstitutionServer : WebClientWrapperBase
    {
        public InstitutionServer(string baseUrl)
            : base(baseUrl + "/api/Institution/")
        {

        }

        public async Task<bool> ExistInstitution(string InstitutionCode)
        {
            return await Execute<bool>("GetExistInstitution?Code=" + InstitutionCode);
        }

        public async Task<bool> ExistInstitutionURL(string URL)
        {
            return await Execute<bool>("GetExistInstitutionURL?URL=" + URL);
        }

        public async Task<InstitutionModel> GetInstitution(string InstitutionCode)
        {
            return await Execute<InstitutionModel>("GetInstitution?Code=" + InstitutionCode);

        }
    } 
}
