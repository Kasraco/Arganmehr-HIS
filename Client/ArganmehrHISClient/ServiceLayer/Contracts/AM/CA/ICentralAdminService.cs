
using DomainClasses.Entities.AM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Contracts.AM.CA
{
    public interface ICentralAdminService
    {

        void DeleteCentralAdmin(CentralAdmin centralAdmin);


        IList<CentralAdmin> GetAllCentralAdmins();
        bool ExistCentralAdmin();
       
        CentralAdmin GetCentralAdminById(int CentralAdminId);

        CentralAdmin GetFirstCentralAdmin();
        CentralAdmin GetCentralAdminByTitle(string CMTitle);
        CentralAdmin GetCentralAdminByName(string Name);

        void InsertCentralAdmin(CentralAdmin centralAdmin);

      
        void UpdateCentralAdmin(CentralAdmin centralAdmin);

      
        bool IsSingleCentralAdminMode();


        bool IsCentralAdminDataValid(CentralAdmin centralAdmin);

        

        void SeedDatabase();

    }
}
