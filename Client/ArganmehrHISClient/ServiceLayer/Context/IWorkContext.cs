using DomainClasses.Entities.Localization;
using DomainClasses.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.AdminArea.Arganmehr;

namespace ServiceLayer.Context
{
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
        Language WorkingLanguage { get; set; }
        //List<Language> WorkingLanguages { get; }
        IList<Language> WorkingLanguages { get; }
        bool IsPublishedLanguage(string seoCode, int PortalId = 0);

        string GetDefaultLanguageSeoCode(int PortalId = 0);

        InstitutionModel CurrentInstitution{ get; set; }


        bool IsAdmin { get; set; }


    }
}
